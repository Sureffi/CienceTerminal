using AlertService.Application.Interfaces;
using AlertService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlertService.Application.Messaging.Commands;

/// <summary>
/// Handles coin metrics updates from Token Metrics Service.
/// Queries fresh coin data from database and updates active alerts via AlertManager.
/// </summary>
public class UpdateCoinMetricsCommandHandler : IRequestHandler<UpdateCoinMetricsCommand>
{
    private readonly ICoinRepository _coinRepository;
    private readonly IAlertManager _alertManager;
    private readonly ILogger<UpdateCoinMetricsCommandHandler> _logger;

    public UpdateCoinMetricsCommandHandler(
        ICoinRepository coinRepository,
        IAlertManager alertManager,
        ILogger<UpdateCoinMetricsCommandHandler> logger)
    {
        _coinRepository = coinRepository;
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task Handle(UpdateCoinMetricsCommand request, CancellationToken cancellationToken)
    {
        var @event = request.Event;
        var coinMintAddress = @event.CoinMintAddress;

        _logger.LogDebug(
            "Processing metrics update for {Symbol} ({CoinMint})",
            @event.Symbol, coinMintAddress);

        // Query fresh coin data from database (already updated by Token Metrics Service)
        var coin = await _coinRepository.GetByMintAddressAsync(coinMintAddress, cancellationToken);

        if (coin == null)
        {
            _logger.LogWarning("Coin {CoinMint} not found in database after metrics update", coinMintAddress);
            return;
        }

        // Get all active alerts
        var activeAlerts = await _alertManager.GetActiveAlertsAsync();

        // Find alerts for this coin
        var alertsForCoin = activeAlerts
            .Where(a =>
            {
                return a switch
                {
                    Alerts.CaMentionAlert caMentionAlert => caMentionAlert.CoinAddress == coinMintAddress,
                    Alerts.TwitterAlert twitterAlert => twitterAlert.CoinMint == coinMintAddress,
                    _ => false
                };
            })
            .ToList();

        if (alertsForCoin.Count == 0)
        {
            _logger.LogDebug("No active alerts found for coin {CoinMint}", coinMintAddress);
            return;
        }

        _logger.LogInformation(
            "Updating {Count} alerts with fresh metrics for {Symbol} ({CoinMint})",
            alertsForCoin.Count, coin.CoinSymbol, coinMintAddress);

        // Update each alert with fresh coin data
        foreach (var alert in alertsForCoin)
        {
            if (alert is Alerts.CaMentionAlert caMentionAlert)
            {
                caMentionAlert.UpdateCoinMetrics(coin);
            }
            else if (alert is Alerts.TwitterAlert twitterAlert)
            {
                twitterAlert.UpdateCoinMetrics(coin);
            }

            // Use AlertManager to notify (this will push via SignalR)
            await _alertManager.AddOrUpdateAlertAsync(alert);
        }

        _logger.LogDebug(
            "Successfully updated {Count} alerts for {Symbol}",
            alertsForCoin.Count, coin.CoinSymbol);
    }
}
