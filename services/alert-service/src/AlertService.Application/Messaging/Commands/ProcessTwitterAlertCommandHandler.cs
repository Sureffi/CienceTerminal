using AlertService.Application.Interfaces;
using AlertService.Domain.Interfaces;
using AlertService.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlertService.Application.Messaging.Commands;

/// <summary>
/// Handles Twitter alert events.
/// Enriches alerts with coin metadata from database before creating alert.
/// </summary>
public class ProcessTwitterAlertCommandHandler : IRequestHandler<ProcessTwitterAlertCommand>
{
    private readonly ICoinRepository _coinRepository;
    private readonly IAlertManager _alertManager;
    private readonly ILogger<ProcessTwitterAlertCommandHandler> _logger;

    public ProcessTwitterAlertCommandHandler(
        ICoinRepository coinRepository,
        IAlertManager alertManager,
        ILogger<ProcessTwitterAlertCommandHandler> logger)
    {
        _coinRepository = coinRepository;
        _alertManager = alertManager;
        _logger = logger;
    }

    public async Task Handle(ProcessTwitterAlertCommand request, CancellationToken cancellationToken)
    {
        var alertEvent = request.AlertEvent;
        var coinMintAddress = alertEvent.CoinMintAddress;

        _logger.LogInformation("Processing Twitter alert {AlertType} for tweet {TweetId} with coin {CoinMint}",
            alertEvent.Type, alertEvent.TweetId, coinMintAddress);

        // Fetch coin metadata for enrichment (if available)
        var coin = string.IsNullOrWhiteSpace(coinMintAddress)
            ? null
            : await _coinRepository.GetByMintAddressAsync(coinMintAddress, cancellationToken);

        if (coin == null && !string.IsNullOrWhiteSpace(coinMintAddress))
        {
            _logger.LogWarning(
                "Coin {CoinMint} not found in database, creating alert with partial data (will be enriched later)",
                coinMintAddress);
        }

        // Create enriched alert data
        var alertData = new TwitterAlertData
        {
            AlertId = alertEvent.AlertId,
            Timestamp = alertEvent.Timestamp,
            Type = alertEvent.Type,
            Severity = alertEvent.Severity,

            // Tweet information
            TweetLink = alertEvent.TweetLink,
            TweetContent = alertEvent.TweetContent,

            // Author information
            AuthorName = alertEvent.AuthorName,
            AuthorUsername = alertEvent.AuthorUsername,
            AuthorProfilePicture = alertEvent.AuthorProfilePicture,
            AuthorFollowers = alertEvent.AuthorFollowers,
            AuthorIsBlueVerified = alertEvent.AuthorIsBlueVerified,
            AuthorIsGoldVerified = alertEvent.AuthorIsGoldVerified,

            // Coin identification
            CoinMintAddress = coinMintAddress,

            // From coin (enriched data) - fallback to defaults if coin not found
            CoinSymbol = coin?.CoinSymbol ?? "",
            CoinImageUrl = coin?.CoinImage,
            MarketCap = coin?.MarketCap,
            Liquidity = coin?.Liquidity,
            Volume24h = coin?.Volume24h,
            HolderCount = coin?.HolderCount,
            TopHoldersPercentage = coin?.TopHoldersPercentage,
            FirstPoolCreatedAt = coin?.FirstPoolCreatedAt,
            IsBlacklisted = coin?.IsBlacklisted ?? false
        };

        await _alertManager.AddTwitterAlertAsync(alertData);

        _logger.LogInformation(
            "Processed Twitter alert for tweet {TweetId} with coin {Symbol} ({CoinMint})",
            alertEvent.TweetId,
            alertData.CoinSymbol,
            coinMintAddress);
    }
}
