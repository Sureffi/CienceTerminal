namespace AlertService.Domain.Entities;

/// <summary>
/// Read-only view of coin data from the shared cienceterminal database.
/// This entity mirrors the coins table owned and managed by the Token Metrics Service.
/// Alert Service has read-only access for validating coins and enriching alerts.
/// </summary>
public class Coin
{
    public Guid Id { get; set; }

    /// <summary>
    /// Solana contract address (mint address). Must be 44 characters.
    /// </summary>
    public string CoinMintAddress { get; set; } = string.Empty;

    /// <summary>
    /// Token symbol (e.g., "BONK", "SOL")
    /// </summary>
    public string? CoinSymbol { get; set; }

    /// <summary>
    /// Token image URL
    /// </summary>
    public string? CoinImage { get; set; }

    /// <summary>
    /// Number of mentions in the last 24 hours (denormalized from MentionAggregates)
    /// </summary>
    public int MentionCount24h { get; set; }

    /// <summary>
    /// Number of token holders
    /// </summary>
    public int? HolderCount { get; set; }

    /// <summary>
    /// Liquidity in USD
    /// </summary>
    public double? Liquidity { get; set; }

    /// <summary>
    /// 24-hour trading volume in USD
    /// </summary>
    public double? Volume24h { get; set; }

    /// <summary>
    /// Market capitalization in USD
    /// </summary>
    public double? MarketCap { get; set; }

    /// <summary>
    /// 24-hour price change percentage
    /// </summary>
    public double? PriceChange24H { get; set; }

    /// <summary>
    /// Percentage of supply held by top holders
    /// </summary>
    public double? TopHoldersPercentage { get; set; }

    /// <summary>
    /// When the first liquidity pool was created for this token
    /// </summary>
    public DateTime? FirstPoolCreatedAt { get; set; }

    /// <summary>
    /// When this coin's metadata was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Indicates if the token is actively being tracked (has recent mentions/alerts).
    /// Managed by Token Metrics Service based on CoinActivated/CoinDeactivated events.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Manual flag for scam tokens or tokens that should not be alerted on.
    /// Coins are blacklisted based on Jupiter API validation (low liquidity, rug indicators, etc.)
    /// </summary>
    public bool IsBlacklisted { get; set; }
}
