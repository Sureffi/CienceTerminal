using AlertService.Application.Interfaces;
using AlertService.Domain.Interfaces;
using AlertService.Domain.Models;
using CienceTerminal.Contracts.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlertService.Application.Messaging.Commands;

/// <summary>
/// Handles mention aggregates update events.
/// Queries the database for trending tokens, enriches with coin metadata, and creates alerts.
/// </summary>
public class ProcessMentionAggregatesUpdateCommandHandler : IRequestHandler<ProcessMentionAggregatesUpdateCommand>
{
    private readonly IMentionAggregateRepository _mentionAggregateRepository;
    private readonly ICoinRepository _coinRepository;
    private readonly ICaMentionRecordRepository _caMentionRecordRepository;
    private readonly IAlertManager _alertManager;
    private readonly ILogger<ProcessMentionAggregatesUpdateCommandHandler> _logger;

    public ProcessMentionAggregatesUpdateCommandHandler(
        IMentionAggregateRepository mentionAggregateRepository,
        ICoinRepository coinRepository,
        ICaMentionRecordRepository caMentionRecordRepository,
        IAlertManager alertManager,
        ILogger<ProcessMentionAggregatesUpdateCommandHandler> logger)
    {
        _mentionAggregateRepository = mentionAggregateRepository;
        _coinRepository = coinRepository;
        _caMentionRecordRepository = caMentionRecordRepository;
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task Handle(ProcessMentionAggregatesUpdateCommand request, CancellationToken cancellationToken)
    {
        var calculatedAt = request.Event.CalculatedAt;
        _logger.LogInformation("Processing mention aggregates update calculated at {CalculatedAt}", calculatedAt);

        // Get top 25 trending tokens
        // TODO: Make this use appsettings.json limit
        var topTrending = await _mentionAggregateRepository.GetTopTrendingAsync(20, cancellationToken);
        _logger.LogInformation("Found {Count} tokens in top trending list", topTrending.Count);

        if (topTrending.Count == 0)
        {
            _logger.LogInformation("No trending tokens found, skipping alert processing");
            return;
        }

        // Fetch coin data for all trending tokens in a single batch query
        var coinMintAddresses = topTrending.Select(a => a.CoinMintAddress).ToList();
        var coinDataMap = await _coinRepository.GetByMintAddressesAsync(coinMintAddresses, cancellationToken);

        _logger.LogInformation("Fetched coin data for {Count}/{Total} trending tokens",
            coinDataMap.Count, topTrending.Count);

        // Fetch top 3 mentioner profile pictures for all trending tokens in a single batch query
        var topMentionersMap = await _caMentionRecordRepository.GetTopMentionerProfilePicturesBatchAsync(
            coinMintAddresses, 3, cancellationToken);

        _logger.LogInformation("Fetched top mentioners for {Count}/{Total} trending tokens",
            topMentionersMap.Count, topTrending.Count);

        // Get existing alerts to determine if we should create new or update existing
        var existingAlerts = await _alertManager.GetActiveAlertsAsync();
        var caMentionAlertsList = existingAlerts
            .OfType<AlertService.Application.Alerts.CaMentionAlert>()
            .ToList();

        _logger.LogDebug("Found {Count} existing CA mention alerts", caMentionAlertsList.Count);

        // Check for duplicates by CoinAddress
        var duplicates = caMentionAlertsList
            .GroupBy(a => a.CoinAddress)
            .Where(g => g.Count() > 1)
            .ToList();

        Dictionary<string, AlertService.Application.Alerts.CaMentionAlert> existingCaMentionAlerts;

        if (duplicates.Any())
        {
            _logger.LogError(
                "DUPLICATE ALERTS DETECTED: {Count} coins have multiple CA mention alerts",
                duplicates.Count);

            foreach (var dup in duplicates)
            {
                _logger.LogError(
                    "Coin {CoinAddress} ({Symbol}) has {Count} alerts: {Details}",
                    dup.Key,
                    dup.First().CoinSymbol,
                    dup.Count(),
                    string.Join(", ", dup.Select(a => $"ID={a.Id} Rank={a.Rank} TS={a.Timestamp:s}")));
            }

            // Use most recent alert for each coin to recover
            existingCaMentionAlerts = caMentionAlertsList
                .GroupBy(a => a.CoinAddress)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Timestamp).First());

            // Remove the duplicate alerts from AlertManager to clean up the state
            foreach (var dup in duplicates)
            {
                // Keep most recent, remove all others
                var mostRecent = dup.OrderByDescending(a => a.Timestamp).First();
                var duplicateAlertsToRemove = dup.Where(a => a.Id != mostRecent.Id).ToList();

                foreach (var alert in duplicateAlertsToRemove)
                {
                    _logger.LogWarning("Removing duplicate alert {AlertId} for coin {CoinAddress}", alert.Id, alert.CoinAddress);
                    await _alertManager.RemoveAlertAsync(alert.Id);
                }
            }

            _logger.LogWarning("Cleaned up {Count} duplicate alerts, using most recent alert for each coin",
                duplicates.Sum(d => d.Count() - 1));
        }
        else
        {
            existingCaMentionAlerts = caMentionAlertsList.ToDictionary(a => a.CoinAddress, a => a);
        }

        // Create/update enriched alerts for each trending token
        var createdCount = 0;
        var updatedCount = 0;
        var skippedBlacklisted = 0;
        var skippedMissingData = 0;

        foreach (var aggregate in topTrending)
        {
            // Try to get enriched coin data
            coinDataMap.TryGetValue(aggregate.CoinMintAddress, out var coin);

            // If coin not in database yet, create alert with partial data
            // Token Metrics enrichment service will eventually populate the coin record
            if (coin == null)
            {
                _logger.LogWarning(
                    "Coin {CoinMint} not found in database, creating alert with partial data (will be enriched later)",
                    aggregate.CoinMintAddress);
                skippedMissingData++;
            }

            // Get top mentioner profile pictures
            topMentionersMap.TryGetValue(aggregate.CoinMintAddress, out var topMentioners);

            // Reuse existing alert ID if alert already exists for this coin
            var alertId = existingCaMentionAlerts.TryGetValue(aggregate.CoinMintAddress, out var existingAlert)
                ? existingAlert.Id
                : Guid.NewGuid();

            var alertData = new CaMentionAlertData
            {
                AlertId = alertId,
                Timestamp = DateTime.UtcNow,
                Severity = DetermineSeverity(aggregate.Rank, aggregate.TrendingScore),
                Rank = aggregate.Rank,

                // From aggregate (always available)
                CoinMintAddress = aggregate.CoinMintAddress,
                TrendScore = aggregate.TrendingScore,
                MentionCount24h = aggregate.MentionCount24h,
                LastMentioned = aggregate.LastMentioned,

                // From coin (enriched data) - fallback to defaults if coin not found
                CoinSymbol = coin?.CoinSymbol ?? aggregate.CoinMintAddress.Substring(0, Math.Min(6, aggregate.CoinMintAddress.Length)),
                CoinImageUrl = coin?.CoinImage ?? "",
                MarketCap = coin?.MarketCap,
                Liquidity = coin?.Liquidity,
                Volume24h = coin?.Volume24h,
                PriceChange24H = coin?.PriceChange24H,
                HolderCount = coin?.HolderCount,
                TopHoldersPercentage = coin?.TopHoldersPercentage,
                FirstPoolCreatedAt = coin?.FirstPoolCreatedAt,
                IsBlacklisted = coin?.IsBlacklisted ?? false,

                // Top mentioners profile pictures (from CA mention records)
                TopMentionerImageUrls = topMentioners ?? new List<string>()
            };

            await _alertManager.AddCaMentionAlertAsync(alertData);

            // Track statistics for logging
            if (alertData.IsBlacklisted)
            {
                skippedBlacklisted++;
            }
            else if (existingAlert != null)
            {
                updatedCount++;
                _logger.LogDebug(
                    "Updated trending alert for {Symbol} ({CoinMint}) at rank {Rank} with score {TrendingScore:F2}",
                    alertData.CoinSymbol, aggregate.CoinMintAddress, aggregate.Rank, aggregate.TrendingScore);
            }
            else
            {
                createdCount++;
                _logger.LogInformation(
                    "Created trending alert for {Symbol} ({CoinMint}) at rank {Rank} with score {TrendingScore:F2}",
                    alertData.CoinSymbol, aggregate.CoinMintAddress, aggregate.Rank, aggregate.TrendingScore);
            }
        }

        // Remove alerts for coins that are no longer in top 25 trending
        var trendingCoinAddresses = topTrending.Select(a => a.CoinMintAddress).ToHashSet();
        var alertsToRemove = existingCaMentionAlerts.Values
            .Where(alert => !trendingCoinAddresses.Contains(alert.CoinAddress))
            .ToList();

        var removedCount = 0;
        foreach (var alertToRemove in alertsToRemove)
        {
            await _alertManager.RemoveAlertAsync(alertToRemove.Id);
            removedCount++;
            _logger.LogInformation(
                "Removed alert for {Symbol} ({CoinMint}) - no longer trending",
                alertToRemove.CoinSymbol, alertToRemove.CoinAddress);
        }

        _logger.LogInformation(
            "Processed trending alerts: {Created} created, {Updated} updated, {Removed} removed (not trending), {Blacklisted} blacklisted, {MissingData} missing coin data",
            createdCount, updatedCount, removedCount, skippedBlacklisted, skippedMissingData);
    }

    private static AlertSeverity DetermineSeverity(int? rank, double trendingScore)
    {
        // Top 10 = High, 11-25 = Medium
        return rank switch
        {
            <= 10 => AlertSeverity.High,
            _ => AlertSeverity.Medium
        };
    }
}
