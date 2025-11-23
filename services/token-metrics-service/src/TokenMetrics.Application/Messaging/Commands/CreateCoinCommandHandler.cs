using MediatR;
using Microsoft.Extensions.Logging;
using TokenMetrics.Application.Messaging.Requests;
using TokenMetrics.Domain.Common;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Messaging.Commands;

/// <summary>
/// Handles creation of new Coin records with Jupiter metadata enrichment.
/// Implements blacklisting logic for tokens not found in Jupiter or flagged as rugpulls.
/// </summary>
public class CreateCoinCommandHandler : IRequestHandler<CreateCoinCommand, Coin>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateCoinCommandHandler> _logger;

    public CreateCoinCommandHandler(
        IMediator mediator,
        ILogger<CreateCoinCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Coin> Handle(CreateCoinCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating coin record for {CoinMint}", request.CoinMintAddress);

        // Fetch Jupiter metadata
        var result = await _mediator.Send(
            new GetJupiterTokenDataRequest(request.CoinMintAddress),
            cancellationToken);

        // Handle result based on success/failure and data content
        if (!result.IsSuccess)
        {
            // Handle different failure types
            if (result.ErrorType == ResultErrorType.NotFound)
            {
                _logger.LogInformation(
                    "Token not found in Jupiter API, blacklisting {CoinMint}",
                    request.CoinMintAddress);

                return CreateBlacklistedCoin(request.CoinMintAddress);
            }
            else if (result.ErrorType == ResultErrorType.RateLimited)
            {
                _logger.LogWarning(
                    "Jupiter API rate limit exceeded for {CoinMint}",
                    request.CoinMintAddress);

                throw new InvalidOperationException(
                    $"Rate limit exceeded when fetching Jupiter data for {request.CoinMintAddress}");
            }
            else
            {
                _logger.LogWarning(
                    "Failed to fetch Jupiter data for {CoinMint}: {ErrorType} - {ErrorMessage}",
                    request.CoinMintAddress, result.ErrorType, result.ErrorMessage);

                throw new InvalidOperationException(
                    $"Failed to fetch Jupiter data: {result.ErrorMessage}");
            }
        }
        else if (result.Data.Audit?.BlockaidRugpull == true)
        {
            // Blockaid detected rugpull - blacklist
            _logger.LogInformation(
                "Blockaid detected rugpull for {CoinMint} (symbol: {Symbol}), blacklisting",
                request.CoinMintAddress, result.Data.Symbol);

            return CreateBlacklistedCoin(request.CoinMintAddress);
        }
        else
        {
            // Valid token with metadata - create enriched coin
            JupiterTokenData tokenData = result.Data;

            _logger.LogInformation(
                "Creating enriched coin record for {Symbol} ({CoinMint})",
                tokenData.Symbol, request.CoinMintAddress);

            return new Coin
            {
                Id = Guid.NewGuid(),
                CoinMintAddress = request.CoinMintAddress,
                CoinSymbol = tokenData.Symbol,
                CoinImage = tokenData.Icon,
                FirstPoolCreatedAt = tokenData?.FirstPool?.CreatedAt,
                MarketCap = tokenData?.MarketCap,
                Liquidity = tokenData?.Liquidity,
                Volume24h = tokenData?.Stats24h?.BuyVolume + tokenData?.Stats24h?.SellVolume ?? 0,
                HolderCount = tokenData?.HolderCount,
                TopHoldersPercentage = tokenData?.Audit?.TopHoldersPercentage,
                MentionCount24h = 0, // Will be updated by aggregation service
                LastUpdated = DateTime.UtcNow,
                IsActive = false,
                IsBlacklisted = false
            };
        }
    }

    private static Coin CreateBlacklistedCoin(string coinMintAddress)
    {
        return new Coin
        {
            Id = Guid.NewGuid(),
            CoinMintAddress = coinMintAddress,
            CoinSymbol = null,
            CoinImage = null,
            MentionCount24h = 0,
            LastUpdated = DateTime.UtcNow,
            IsActive = false,
            IsBlacklisted = true
        };
    }
}
