using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TokenMetrics.Application.Trending;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Infrastructure.Data;

namespace TokenMetrics.Infrastructure.Services;

/// <summary>
/// Background service that periodically:
/// 1. Queries recent CA mentions from database
/// 2. Calculates trending scores and rolling aggregates
/// 3. Updates MentionAggregates table
/// 4. Publishes MentionAggregatesUpdatedEvent to notify Alert Service
/// </summary>
public class TrendingAggregationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TrendingAggregationService> _logger;
    private readonly TimeSpan _aggregationInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _lookbackWindow = TimeSpan.FromHours(24);

    public TrendingAggregationService(
        IServiceScopeFactory scopeFactory,
        ILogger<TrendingAggregationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Trending aggregation service started (using MentionAggregates)");

        // Initial delay to allow services to stabilize
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        // Agregation job loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                await AggregateAndPublishAsync(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in trending aggregation cycle");
            }

            await Task.Delay(_aggregationInterval, stoppingToken);
        }
    }

    private async Task AggregateAndPublishAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var dbContext = serviceProvider.GetRequiredService<TokenMetricsDbContext>();
        var eventProducer = serviceProvider.GetRequiredService<IEventProducer>();
        var awsOptions = serviceProvider.GetRequiredService<IOptions<AwsOptions>>().Value;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var cutoff = DateTime.UtcNow.Subtract(_lookbackWindow);

        // BATCH LOAD: Get all mentions in one query (no N+1 problem)
        var allMentions = await dbContext.CaMentionRecords
            .Where(m => m.Timestamp >= cutoff)
            .OrderBy(m => m.CoinMintAddress)
            .ThenBy(m => m.Timestamp)
            .AsNoTracking() // Read-only optimization
            .ToListAsync(cancellationToken);

        // Group mentions by CA address
        var mentionsByCA = allMentions
            .GroupBy(m => m.CoinMintAddress)
            .ToDictionary(g => g.Key, g => g.ToList());

        var allCAs = mentionsByCA.Keys.ToList();

        // BATCH LOAD: Get coins to check blacklist status (no N+1 problem)
        var coins = await dbContext.Coins
            .Where(c => allCAs.Contains(c.CoinMintAddress))
            .AsNoTracking()
            .ToDictionaryAsync(c => c.CoinMintAddress, cancellationToken);

        // Filter out blacklisted coins
        var blacklistedCAs = coins
            .Where(kvp => kvp.Value.IsBlacklisted)
            .Select(kvp => kvp.Key)
            .ToList();

        var activeCAs = allCAs.Except(blacklistedCAs).ToList();

        if (blacklistedCAs.Any())
        {
            _logger.LogInformation("Filtered out {Count} blacklisted CAs from aggregation", blacklistedCAs.Count);
        }

        _logger.LogInformation("Aggregating trends for {Count} active CAs", activeCAs.Count);

        // BATCH LOAD: Get all existing aggregates in one query (no N+1 problem)
        var existingAggregates = await dbContext.MentionAggregates
            .Where(a => activeCAs.Contains(a.CoinMintAddress))
            .ToDictionaryAsync(a => a.CoinMintAddress, cancellationToken);

        var aggregates = new List<MentionAggregate>();

        // Track previous top 25 for detecting changes
        var previousTop25 = existingAggregates.Values
            .Where(a => a.Rank.HasValue)
            .Select(a => a.CoinMintAddress)
            .ToHashSet();

        foreach (var ca in activeCAs)
        {
            // Get mentions from dictionary (already loaded)
            var mentions = mentionsByCA[ca];

            // Get existing aggregate from dictionary (already loaded) to retrieve previous EMA
            existingAggregates.TryGetValue(ca, out var existing);

            // Calculate trending state
            // Note: We no longer persist EMA values, they're computed fresh each time
            var trendingState = TrendingCalculator.CalculateTrend(mentions, previousEmaJson: null);

            var aggregate = new MentionAggregate
            {
                Id = existing?.Id ?? Guid.NewGuid(), // Preserve existing ID or create new
                CoinMintAddress = ca,
                MentionCount5m = trendingState.MentionCount5m,
                MentionCount1h = trendingState.MentionCount1h,
                MentionCount6h = trendingState.MentionCount6h,
                MentionCount24h = trendingState.MentionCount24h,
                TrendingScore = trendingState.TrendScore,
                LastMentioned = mentions.Max(m => m.Timestamp),
                LastCalculated = DateTime.UtcNow,
                Rank = null // Will be assigned below for top 25
            };

            aggregates.Add(aggregate);
        }

        // Sort by trend score and assign ranks to top 25
        var top25 = aggregates
            .OrderByDescending(a => a.TrendingScore)
            .Take(25)
            .ToList();

        // Create a set for O(1) lookup when clearing ranks
        var top25Addresses = new HashSet<string>(top25.Select(a => a.CoinMintAddress));

        // Assign ranks to top 25
        for (int i = 0; i < top25.Count; i++)
        {
            top25[i].Rank = i + 1;
        }

        // Update database (upsert) - reuse existingAggregates dictionary (no N+1!)
        foreach (var aggregate in aggregates)
        {
            if (existingAggregates.TryGetValue(aggregate.CoinMintAddress, out var existing))
            {
                // Update existing record in-place
                dbContext.Entry(existing).CurrentValues.SetValues(aggregate);
            }
            else
            {
                // Insert new record
                await dbContext.MentionAggregates.AddAsync(aggregate, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Upserted {Count} mention aggregates", aggregates.Count);

        // Delete mention aggregates for blacklisted coins
        if (blacklistedCAs.Any())
        {
            var blacklistedAggregates = await dbContext.MentionAggregates
                .Where(a => blacklistedCAs.Contains(a.CoinMintAddress))
                .ToListAsync(cancellationToken);

            if (blacklistedAggregates.Any())
            {
                dbContext.MentionAggregates.RemoveRange(blacklistedAggregates);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted {Count} mention aggregates for blacklisted coins", blacklistedAggregates.Count);
            }
        }

        // STEP 5: Create/update Coin records with denormalized MentionCount24h
        var existingCoins = await dbContext.Coins
            .Where(c => activeCAs.Contains(c.CoinMintAddress))
            .ToDictionaryAsync(c => c.CoinMintAddress, cancellationToken);

        foreach (var aggregate in aggregates)
        {
            if (existingCoins.TryGetValue(aggregate.CoinMintAddress, out var existingCoin))
            {
                // Update existing coin's mention count
                existingCoin.MentionCount24h = (int)Math.Round(aggregate.MentionCount24h);
                existingCoin.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                // Create new coin record with minimal data
                var newCoin = new Coin
                {
                    Id = Guid.NewGuid(),
                    CoinMintAddress = aggregate.CoinMintAddress,
                    MentionCount24h = (int)Math.Round(aggregate.MentionCount24h),
                    LastUpdated = DateTime.UtcNow,
                    IsActive = false,
                    IsBlacklisted = false
                };

                await dbContext.Coins.AddAsync(newCoin, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        stopwatch.Stop();
        _logger.LogInformation(
            "Aggregation complete: {Count} CAs processed, {CoinsUpdated} coins updated in {ElapsedMs}ms",
            aggregates.Count, aggregates.Count, stopwatch.ElapsedMilliseconds);

        // Publish lightweight notification event - Alert Service will query the database
        var aggregateEvent = new MentionAggregatesUpdatedEvent
        {
            CalculatedAt = DateTime.UtcNow
        };

        await eventProducer.PublishAsync(
            awsOptions.SNS.MentionAggregatesUpdatedTopicArn,
            aggregateEvent,
            cancellationToken);

        _logger.LogInformation("Published MentionAggregatesUpdatedEvent to SNS");
    }
}
