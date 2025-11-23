using MediatR;
using Microsoft.Extensions.Logging;
using TokenMetrics.Application.Interfaces;

namespace TokenMetrics.Application.Messaging.Commands;

/// <summary>
/// Handles CA mention detected events.
/// Delegates coin creation to CreateCoinCommand and manages coin records.
/// Triggers instant metric updates for coins with active alerts.
/// </summary>
public class ProcessCaMentionDetectedCommandHandler : IRequestHandler<ProcessCaMentionDetectedCommand>
{
    private readonly ICoinRepository _coinRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<ProcessCaMentionDetectedCommandHandler> _logger;
    private readonly IMediator _mediator;

    public ProcessCaMentionDetectedCommandHandler(
        ICoinRepository coinRepository,
        IAlertRepository alertRepository,
        ILogger<ProcessCaMentionDetectedCommandHandler> logger,
        IMediator mediator)
    {
        _coinRepository = coinRepository;
        _alertRepository = alertRepository;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(ProcessCaMentionDetectedCommand request, CancellationToken cancellationToken)
    {
        var evt = request.Event;

        _logger.LogInformation(
            "Processing CA mention detected: {CoinMint} in tweet {TweetId} by @{Username}",
            evt.CoinMintAddress, evt.TweetId, evt.Username);

        // Check if coin already exists
        var existingCoin = await _coinRepository.GetByMintAddressAsync(evt.CoinMintAddress, cancellationToken);

        if (existingCoin == null)
        {
            _logger.LogInformation("New coin detected: {CoinMint}, creating record", evt.CoinMintAddress);

            try
            {
                // Delegate coin creation to CreateCoinCommand
                var newCoin = await _mediator.Send(
                    new CreateCoinCommand(evt.CoinMintAddress),
                    cancellationToken);

                await _coinRepository.AddAsync(newCoin, cancellationToken);
                await _coinRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Created coin record for {CoinMint} (Blacklisted: {IsBlacklisted})",
                    evt.CoinMintAddress, newCoin.IsBlacklisted);
            }
            catch (InvalidOperationException ex)
            {
                // Handle rate limits and transient failures gracefully
                _logger.LogWarning(
                    "Skipping coin creation for {CoinMint}: {ErrorMessage}",
                    evt.CoinMintAddress, ex.Message);
                return; // Skip processing, will be picked up in next mention
            }
        }

        // Check if this coin has an active alert - if so, publish instant update
        await TriggerInstantUpdateIfActiveAlertAsync(evt.CoinMintAddress, cancellationToken);
    }

    /// <summary>
    /// Checks if the coin has an active alert and triggers an instant metrics update.
    /// Increments MentionCount24h immediately and sends command to publish update event.
    /// The background aggregation service will recalculate the full rolling count every 60 seconds.
    /// </summary>
    private async Task TriggerInstantUpdateIfActiveAlertAsync(string coinMintAddress, CancellationToken cancellationToken)
    {
        try
        {
            // Query Alert Service's alerts table to see if this coin has active alerts
            var activeCoins = await _alertRepository.GetActiveCoinsAsync(cancellationToken);

            if (!activeCoins.Contains(coinMintAddress))
            {
                // No active alert for this coin - no need to publish update
                return;
            }

            _logger.LogInformation(
                "Coin {CoinMint} has active alert - incrementing mention count and triggering instant update",
                coinMintAddress);

            // Fetch coin data
            var coin = await _coinRepository.GetByMintAddressAsync(coinMintAddress, cancellationToken);

            if (coin == null)
            {
                _logger.LogWarning(
                    "Cannot trigger instant update for {CoinMint} - coin not found in database",
                    coinMintAddress);
                return;
            }

            // Increment the 24h mention count immediately for instant frontend updates
            coin.MentionCount24h++;
            coin.LastUpdated = DateTime.UtcNow;

            await _coinRepository.UpdateAsync(coin, cancellationToken);
            await _coinRepository.SaveChangesAsync(cancellationToken);

            // Send command to publish metrics update event (handled in Infrastructure layer)
            await _mediator.Send(new PublishTokenMetricsUpdatedCommand(coinMintAddress), cancellationToken);

            _logger.LogDebug(
                "Triggered instant metrics update for {CoinMint} - new count: {MentionCount24h}",
                coinMintAddress, coin.MentionCount24h);
        }
        catch (Exception ex)
        {
            // Don't fail the entire handler if instant update fails
            _logger.LogError(ex, "Error triggering instant update for {CoinMint}", coinMintAddress);
        }
    }
}
