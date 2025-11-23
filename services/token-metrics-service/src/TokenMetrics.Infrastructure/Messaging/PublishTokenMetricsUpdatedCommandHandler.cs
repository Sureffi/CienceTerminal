using CienceTerminal.AWS.Abstractions;
using CienceTerminal.AWS.Configuration;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TokenMetrics.Application.Interfaces;
using TokenMetrics.Application.Messaging.Commands;

namespace TokenMetrics.Infrastructure.Messaging;

/// <summary>
/// Handles publishing TokenMetricsUpdatedEvent to SNS.
/// Fetches fresh coin data and publishes event to notify Alert Service.
/// </summary>
public class PublishTokenMetricsUpdatedCommandHandler : IRequestHandler<PublishTokenMetricsUpdatedCommand>
{
    private readonly ICoinRepository _coinRepository;
    private readonly IEventProducer _eventProducer;
    private readonly ILogger<PublishTokenMetricsUpdatedCommandHandler> _logger;
    private readonly AwsOptions _awsOptions;

    public PublishTokenMetricsUpdatedCommandHandler(
        ICoinRepository coinRepository,
        IEventProducer eventProducer,
        ILogger<PublishTokenMetricsUpdatedCommandHandler> logger,
        IOptions<AwsOptions> awsOptions)
    {
        _coinRepository = coinRepository;
        _eventProducer = eventProducer;
        _logger = logger;
        _awsOptions = awsOptions.Value;
    }

    public async Task Handle(PublishTokenMetricsUpdatedCommand request, CancellationToken cancellationToken)
    {
        var coinMintAddress = request.CoinMintAddress;

        // Fetch fresh coin data
        var coin = await _coinRepository.GetByMintAddressAsync(coinMintAddress, cancellationToken);

        if (coin == null)
        {
            _logger.LogWarning(
                "Cannot publish metrics update for {CoinMint} - coin not found",
                coinMintAddress);
            return;
        }

        // Publish TokenMetricsUpdatedEvent to notify Alert Service
        var metricsEvent = new TokenMetricsUpdatedEvent
        {
            CoinMintAddress = coinMintAddress,
            UpdatedAt = DateTime.UtcNow,
            Symbol = coin.CoinSymbol ?? "",
            IconUrl = coin.CoinImage,
            MarketCap = coin.MarketCap.HasValue ? (double)coin.MarketCap.Value : null,
            Liquidity = coin.Liquidity.HasValue ? (double)coin.Liquidity.Value : null,
            Volume24h = coin.Volume24h.HasValue ? (double)coin.Volume24h.Value : null,
            HolderCount = coin.HolderCount,
            Top10HoldersPercent = coin.TopHoldersPercentage.HasValue ? (double)coin.TopHoldersPercentage.Value : null,
            FirstPoolCreatedAt = coin.FirstPoolCreatedAt
        };

        await _eventProducer.PublishAsync(
            _awsOptions.SNS.TokenMetricsUpdatedTopicArn,
            metricsEvent,
            cancellationToken);

        _logger.LogInformation(
            "Published instant metrics update for {CoinMint} - MentionCount24h: {Count}",
            coinMintAddress, coin.MentionCount24h);
    }
}
