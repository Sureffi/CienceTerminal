using AlertService.Application.Interfaces;
using AlertService.Domain.Entities;
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

    // TODO: Update all coins on one notification
    public async Task Handle(UpdateCoinMetricsCommand request, CancellationToken cancellationToken)
    {
        var @event = request.Event;
        var coinMintAddress = @event.CoinMintAddress;

        _logger.LogInformation("Processing metrics update");

        // Get all active alerts
        List<Alert> activeAlerts = await _alertManager.GetActiveAlertsAsync();

        // Get unique active coin mint addresses
        List<string> uniqueCoins = activeAlerts.Select(static a =>
                {
                    return a switch
                    {
                        Alerts.CaMentionAlert caMentionAlert => caMentionAlert.CoinAddress,
                        Alerts.TwitterAlert twitterAlert => twitterAlert.CoinMint,
                        _ => throw new NotImplementedException(),
                    };
                })
            .Distinct()
            .ToList();

        // Batch get coins for active alerts
        var coins = await _coinRepository.GetByMintAddressesAsync(uniqueCoins, cancellationToken);

        // For each active alert, update coin metrics
        foreach (var alert in activeAlerts)
        {
            if (alert is Alerts.CaMentionAlert caMentionAlert)
            {
                if (coins.TryGetValue(caMentionAlert.CoinAddress, out var metrics))
                {
                    caMentionAlert.UpdateCoinMetrics(metrics);
                }
            }
            else if (alert is Alerts.TwitterAlert twitterAlert)
            {
                if (coins.TryGetValue(twitterAlert.CoinMint, out var metrics))
                {
                    twitterAlert.UpdateCoinMetrics(metrics);
                }
            }

            // Use alert manager to notify (Will push to SignalR)
            await _alertManager.AddOrUpdateAlertAsync(alert);
        }

    }
}
