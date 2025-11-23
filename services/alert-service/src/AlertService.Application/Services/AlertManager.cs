using System.Collections.Concurrent;
using AlertService.Application.Alerts;
using AlertService.Domain.Configuration;
using AlertService.Domain.Entities;
using AlertService.Domain.Interfaces;
using AlertService.Domain.Models;
using CienceTerminal.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertService.Application.Services;

public class AlertManager : IAlertManager
{
    private readonly ConcurrentDictionary<Guid, Alert> _activeAlerts = new();
    private readonly IAlertNotificationService _notificationService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AlertOptions _alertOptions;
    private readonly ILogger<AlertManager> _logger;
    private bool _isInitialized = false;

    public AlertManager(
        IAlertNotificationService notificationService,
        IServiceScopeFactory scopeFactory,
        IOptions<AlertOptions> alertOptions,
        ILogger<AlertManager> logger)
    {
        _notificationService = notificationService;
        _scopeFactory = scopeFactory;
        _alertOptions = alertOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Initializes AlertManager by loading persisted alerts from database.
    /// Should be called once on service startup.
    /// </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            _logger.LogWarning("AlertManager already initialized, skipping");
            return;
        }

        _logger.LogInformation("Initializing AlertManager, loading alerts from database...");

        List<Alert> persistedAlerts;
        using (var scope = _scopeFactory.CreateScope())
        {
            var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
            persistedAlerts = await alertRepository.GetAllAsync(cancellationToken);
        }

        // Group alerts by type to enforce limits
        var alertsByType = persistedAlerts.GroupBy(a => a.Type);

        foreach (var typeGroup in alertsByType)
        {
            var alertType = typeGroup.Key;
            var alertTypeName = alertType.ToString();
            var alerts = typeGroup.OrderByDescending(a => a.Timestamp).ToList();

            // Check if there's a limit for this type
            if (_alertOptions.MaxActiveAlertsByType.TryGetValue(alertTypeName, out var typeLimit))
            {
                // Only load the most recent alerts up to the limit
                var alertsToLoad = alerts.Take(typeLimit).ToList();
                var alertsToRemove = alerts.Skip(typeLimit).ToList();

                foreach (var alert in alertsToLoad)
                {
                    _activeAlerts.TryAdd(alert.Id, alert);
                }

                // Remove excess alerts from database
                if (alertsToRemove.Any())
                {
                    _logger.LogWarning(
                        "Removing {Count} excess {AlertType} alerts from database (limit: {Limit})",
                        alertsToRemove.Count, alertTypeName, typeLimit);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
                        foreach (var alert in alertsToRemove)
                        {
                            await alertRepository.RemoveAsync(alert.Id, cancellationToken);
                        }
                    }
                }
            }
            else
            {
                // No specific limit for this type, load all
                foreach (var alert in alerts)
                {
                    _activeAlerts.TryAdd(alert.Id, alert);
                }
            }
        }

        _isInitialized = true;
        _logger.LogInformation("AlertManager initialized with {AlertCount} alerts", _activeAlerts.Count);
    }

    public Task<List<Alert>> GetActiveAlertsAsync()
    {
        var alerts = _activeAlerts.Values
            .OrderByDescending(a => a.Timestamp)
            .ToList();

        return Task.FromResult(alerts);
    }

    public async Task AddOrUpdateAlertAsync(Alert alert)
    {
        if (alert.Id == Guid.Empty)
        {
            alert.Id = Guid.NewGuid();
        }

        var alertTypeName = alert.Type.ToString();
        if (_alertOptions.MaxActiveAlertsByType.TryGetValue(alertTypeName, out var typeLimit))
        {
            var alertsOfType = _activeAlerts.Values
                .Where(a => a.Type == alert.Type)
                .ToList();

            while (alertsOfType.Count > typeLimit)
            {
                var oldestAlert = alertsOfType
                    .OrderBy(a => a.Timestamp)
                    .FirstOrDefault();

                if (oldestAlert != null)
                {
                    await RemoveAlertAsync(oldestAlert.Id);
                    alertsOfType.Remove(oldestAlert);
                }
                else
                {
                    break;
                }
            }
        }
        else if (_activeAlerts.Count > _alertOptions.MaxActiveAlerts)
        {
            var oldestAlert = _activeAlerts.Values
                .OrderBy(a => a.Timestamp)
                .FirstOrDefault();

            if (oldestAlert != null)
            {
                await RemoveAlertAsync(oldestAlert.Id);
            }
        }

        _ = _activeAlerts.AddOrUpdate(alert.Id, alert, (key, existing) => alert);

        // Persist to database
        using (var scope = _scopeFactory.CreateScope())
        {
            var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
            await alertRepository.AddAsync(alert);
        }

        await _notificationService.NotifyAlertAddedAsync(alert);
    }

    /// <summary>
    /// RemoveAlertAsync string overload method designed to be called through an api by external services
    /// </summary>
    /// <param name="alertId"></param>
    /// <returns></returns>
    public async Task RemoveAlertAsync(string alertId)
    {
        if (Guid.TryParse(alertId, out var guid))
        {
            await RemoveAlertAsync(guid);
        }
    }

    /// <summary>
    /// RemoveAlertAsync removes the alert with guid from alert manager and sends notification to the frontend that the alert was removed
    /// </summary>
    /// <param name="alertId"></param>
    /// <returns></returns>
    public async Task RemoveAlertAsync(Guid alertId)
    {
        if (_activeAlerts.TryRemove(alertId, out var removedAlert))
        {
            // Remove from database
            using (var scope = _scopeFactory.CreateScope())
            {
                var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
                await alertRepository.RemoveAsync(alertId);
            }

            await _notificationService.NotifyAlertRemovedAsync(alertId);
        }
    }

    public async Task AddTwitterAlertAsync(TwitterAlertData alertData)
    {
        // Skip if blacklisted
        if (alertData.IsBlacklisted)
        {
            _logger.LogWarning(
                "Skipping alert creation for blacklisted coin {CoinMint}",
                alertData.CoinMintAddress);
            return;
        }

        var alert = new TwitterAlert
        {
            Id = alertData.AlertId,
            Type = alertData.Type,
            Timestamp = alertData.Timestamp,
            Severity = alertData.Severity,

            // Tweet information
            TweetLink = alertData.TweetLink,
            TweetContent = alertData.TweetContent,

            // Author information
            AuthorName = alertData.AuthorName,
            AuthorUsername = alertData.AuthorUsername,
            AuthorProfilePicture = alertData.AuthorProfilePicture,
            AuthorFollowers = alertData.AuthorFollowers,
            AuthorIsBlueVerified = alertData.AuthorIsBlueVerified,
            AuthorIsGoldVerified = alertData.AuthorIsGoldVerified,

            // Coin information (enriched from database)
            CoinMint = alertData.CoinMintAddress,
            CoinSymbol = alertData.CoinSymbol,
            CoinImageUrl = alertData.CoinImageUrl,
            CoinFirstPoolCreatedAt = alertData.FirstPoolCreatedAt,
            CoinMarketCap = alertData.MarketCap.HasValue ? (double)alertData.MarketCap.Value : null,
            CoinHolderCount = alertData.HolderCount,
            CoinLiquidity = alertData.Liquidity.HasValue ? (double)alertData.Liquidity.Value : null,
            CoinTop10Holders = alertData.TopHoldersPercentage.HasValue ? (double)alertData.TopHoldersPercentage.Value : null
        };

        await AddOrUpdateAlertAsync(alert);
    }

    public async Task AddCaMentionAlertAsync(CaMentionAlertData alertData)
    {
        // Skip if blacklisted
        if (alertData.IsBlacklisted)
        {
            _logger.LogWarning(
                "Skipping alert creation for blacklisted coin {CoinMint}",
                alertData.CoinMintAddress);
            return;
        }

        var alert = new CaMentionAlert(alertData.CoinMintAddress, alertData.CoinSymbol)
        {
            Id = alertData.AlertId,
            Timestamp = alertData.Timestamp,
            Severity = alertData.Severity,
            Rank = alertData.Rank,
            CoinImageUrl = alertData.CoinImageUrl,
            TrendScore = alertData.TrendScore,
            MentionCount24Hour = alertData.MentionCount24h,
            LastMentioned = alertData.LastMentioned,
            TopMentionerImageUrls = alertData.TopMentionerImageUrls,
            MarketCap = alertData.MarketCap,
            Liquidity = alertData.Liquidity,
            Volume24h = alertData.Volume24h,
            HolderCount = alertData.HolderCount,
            TopHoldersPercentage = alertData.TopHoldersPercentage,
            FirstPoolCreatedAt = alertData.FirstPoolCreatedAt
        };

        await AddOrUpdateAlertAsync(alert);
    }

    public async Task RemoveAlertsByCoinAsync(string coinMintAddress)
    {
        var alertsToRemove = _activeAlerts.Values
            .Where(alert =>
            {
                return alert switch
                {
                    CaMentionAlert caMentionAlert => caMentionAlert.CoinAddress == coinMintAddress,
                    TwitterAlert twitterAlert => twitterAlert.CoinMint == coinMintAddress,
                    _ => false
                };
            })
            .ToList();

        foreach (var alert in alertsToRemove)
        {
            await RemoveAlertAsync(alert.Id);
        }

        // Also remove from database (belt and suspenders)
        using (var scope = _scopeFactory.CreateScope())
        {
            var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
            await alertRepository.RemoveByCoinAsync(coinMintAddress);
        }

        _logger.LogInformation(
            "Removed {Count} alerts for blacklisted coin {CoinMint}",
            alertsToRemove.Count,
            coinMintAddress);
    }

}
