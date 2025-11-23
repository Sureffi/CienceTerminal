using CienceTerminal.Contracts.Events;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Infrastructure.Extensions;

public static class CoinExtensions
{
    public static TokenMetricsUpdatedEvent ToTokenMetricsUpdatedEvent(this Coin coin)
    {
        return new TokenMetricsUpdatedEvent
        {
            CoinMintAddress = coin.CoinMintAddress,
            UpdatedAt = DateTime.UtcNow,
            Symbol = coin.CoinSymbol ?? "",
            IconUrl = coin.CoinImage,
            MarketCap = coin.MarketCap,
            Liquidity = coin.Liquidity,
            Volume24h = coin.Volume24h,
            HolderCount = coin.HolderCount,
            Top10HoldersPercent = coin.TopHoldersPercentage,
            FirstPoolCreatedAt = coin.FirstPoolCreatedAt
        };
    }
}
