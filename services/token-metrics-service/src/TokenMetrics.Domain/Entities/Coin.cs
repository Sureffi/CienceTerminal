using System;

namespace TokenMetrics.Domain.Entities;

/// <summary>
/// Represents a cryptocurrency token with enriched metadata from external sources.
/// This is a slowly-changing dimension table that aggregates data from Jupiter, Helius, and mention tracking.
/// </summary>
public class Coin
{
    /// <summary>
    /// Unique identifier for the coin record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Solana contract address (mint address). Must be 44 characters.
    /// </summary>
    public string CoinMintAddress { get; set; } = string.Empty;

    /// <summary>
    /// Token symbol (e.g., "BONK", "SOL"). Fetched from Jupiter API.
    /// </summary>
    public string? CoinSymbol { get; set; }

    /// <summary>
    /// Token image/logo URL. Fetched from Jupiter API.
    /// </summary>
    public string? CoinImage { get; set; }

    /// <summary>
    /// Number of mentions in the last 24 hours. Denormalized from MentionAggregates for quick access.
    /// </summary>
    public int MentionCount24h { get; set; }

    /// <summary>
    /// Total number of token holders. Fetched from Helius RPC API.
    /// </summary>
    public int? HolderCount { get; set; }

    /// <summary>
    /// Total liquidity across all pools. Fetched from Jupiter API.
    /// </summary>
    public double? Liquidity { get; set; }

    /// <summary>
    /// 24-hour trading volume. Fetched from Jupiter API.
    /// </summary>
    public double? Volume24h { get; set; }

    /// <summary>
    /// Market capitalization. Fetched from Jupiter API.
    /// </summary>
    public double? MarketCap { get; set; }

    public double? PriceChange24H { get; set; }

    /// <summary>
    /// Percentage of supply held by top holders. Indicates centralization risk.
    /// </summary>
    public double? TopHoldersPercentage { get; set; }

    /// <summary>
    /// Timestamp when the first liquidity pool was created (token launch date).
    /// </summary>
    public DateTime? FirstPoolCreatedAt { get; set; }

    /// <summary>
    /// Timestamp when metadata was last refreshed from external APIs.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Indicates if the token is actively being tracked (has recent mentions).
    /// Can be set to false to stop tracking without deleting historical data.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Manual blacklist flag for scam tokens or tokens that should not be alerted on.
    /// </summary>
    public bool IsBlacklisted { get; set; }

    public void UpdateFromJupiterData(JupiterTokenData jupiterData)
    {
        Volume24h = jupiterData?.Stats24h?.BuyVolume + jupiterData?.Stats24h?.SellVolume;
        CoinSymbol = jupiterData?.Symbol;
        CoinImage = jupiterData?.Icon;
        MarketCap = jupiterData?.MarketCap;
        PriceChange24H = jupiterData?.Stats24h?.PriceChange;
        Liquidity = jupiterData?.Liquidity;
        HolderCount = jupiterData?.HolderCount;
        TopHoldersPercentage = jupiterData?.Audit?.TopHoldersPercentage;
        FirstPoolCreatedAt = jupiterData?.FirstPool?.CreatedAt;
        LastUpdated = DateTime.UtcNow;
    }
}
