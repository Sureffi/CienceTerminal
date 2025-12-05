using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Application.Messaging.Commands;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Domain.Interfaces;
using TokenMetrics.Infrastructure.Extensions;

namespace TokenMetrics.Infrastructure.Services;

/// <summary>
/// Background service that periodically updates coin metrics for coins with active alerts.
/// Queries alerts table to find active coins, fetches fresh data from Jupiter/Helius,
/// updates coins table, and publishes TokenMetricsUpdatedEvent.
/// </summary>
public class CoinMetricsUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CoinMetricsUpdateService> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1); // Update every 60 seconds

    public CoinMetricsUpdateService(
        IServiceScopeFactory scopeFactory,
        ILogger<CoinMetricsUpdateService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CoinMetricsUpdateService started");

        // Wait 10 seconds before first run to let other services initialize
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateCoinMetricsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coin metrics");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }

        _logger.LogInformation("CoinMetricsUpdateService stopped");
    }

    private async Task UpdateCoinMetricsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
        var coinRepository = scope.ServiceProvider.GetRequiredService<ICoinRepository>();
        var jupiterClient = scope.ServiceProvider.GetRequiredService<IJupiterClient>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var eventProducer = scope.ServiceProvider.GetRequiredService<IEventProducer>();
        var awsOptions = scope.ServiceProvider.GetRequiredService<IOptions<AwsOptions>>().Value;

        // Get coins with active alerts
        var activeCoins = await alertRepository.GetActiveCoinsAsync(cancellationToken);

        if (activeCoins.Count == 0)
        {
            _logger.LogDebug("No active coins to update");
            return;
        }

        _logger.LogInformation("Updating metrics for {Count} active coins", activeCoins.Count);

        // Fetch all token data in a single batch request
        var jupiterDataMap = await jupiterClient.GetBatchTokenMetadataAsync(activeCoins, cancellationToken);

        var updateCount = 0;
        var errorCount = 0;

        foreach (var coinMintAddress in activeCoins)
        {
            try
            {
                // Check if we have data for this coin from the batch fetch
                if (!jupiterDataMap.TryGetValue(coinMintAddress, out var jupiterData))
                {
                    _logger.LogWarning("No Jupiter data found for coin {CoinMint} in batch response", coinMintAddress);
                    errorCount++;
                    continue;
                }

                // Get existing coin or create new entry via command
                Coin? coin = await coinRepository.GetByMintAddressAsync(coinMintAddress, cancellationToken);

                if (coin == null)
                {
                    // Create new coin entry via CreateCoinCommand
                    coin = await mediator.Send(new CreateCoinCommand(coinMintAddress), cancellationToken);
                    await coinRepository.AddAsync(coin, cancellationToken);
                    _logger.LogInformation("Created new coin entry for {Symbol} ({CoinMint})",
                        coin.CoinSymbol, coinMintAddress);
                }
                else
                {
                    // Update existing coin with fresh Jupiter data
                    coin.UpdateFromJupiterData(jupiterData);

                    await coinRepository.UpdateAsync(coin, cancellationToken);
                    _logger.LogDebug("Updated metrics for {Symbol} ({CoinMint})",
                        jupiterData.Symbol, coinMintAddress);
                }

                await coinRepository.SaveChangesAsync(cancellationToken);

                // var metricsEvent = coin.ToTokenMetricsUpdatedEvent();
                updateCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metrics for coin {CoinMint}", coinMintAddress);
                errorCount++;
            }
        }

        // TODO: Notify once after all coins updated
        // Publish event to notify Alert Service
        await eventProducer.PublishAsync(awsOptions.SNS.TokenMetricsUpdatedTopicArn, new TokenMetricsUpdatedEvent(), cancellationToken);

        _logger.LogInformation(
            "Coin metrics update completed: {Updated} updated, {Errors} errors out of {Total} coins",
            updateCount, errorCount, activeCoins.Count);
    }
}
