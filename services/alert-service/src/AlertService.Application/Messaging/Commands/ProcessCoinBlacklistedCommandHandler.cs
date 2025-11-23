using AlertService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlertService.Application.Messaging.Commands;

public class ProcessCoinBlacklistedCommandHandler : IRequestHandler<ProcessCoinBlacklistedCommand>
{
    private readonly IAlertManager _alertManager;
    private readonly ILogger<ProcessCoinBlacklistedCommandHandler> _logger;

    public ProcessCoinBlacklistedCommandHandler(
        IAlertManager alertManager,
        ILogger<ProcessCoinBlacklistedCommandHandler> logger)
    {
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task Handle(ProcessCoinBlacklistedCommand request, CancellationToken cancellationToken)
    {
        var blacklistEvent = request.Event;

        _logger.LogWarning(
            "Coin {CoinMint} has been blacklisted. Reason: {Reason}. Removing all active alerts.",
            blacklistEvent.CoinMintAddress,
            blacklistEvent.Reason);

        await _alertManager.RemoveAlertsByCoinAsync(blacklistEvent.CoinMintAddress);

        _logger.LogInformation(
            "Completed processing blacklist event for coin {CoinMint}",
            blacklistEvent.CoinMintAddress);
    }
}
