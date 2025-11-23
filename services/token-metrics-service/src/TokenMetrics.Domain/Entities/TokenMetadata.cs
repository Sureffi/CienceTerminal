namespace TokenMetrics.Domain.Entities;

/// <summary>
/// Complete token metadata entity combining data from multiple sources (Jupiter, DexScreener)
/// </summary>
public class TokenMetadata
{
    public string ContractAddress { get; init; } = string.Empty;

    // Basic Token Info
    public string Symbol { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
    public int Decimals { get; set; }

    // Jupiter API Data
    public decimal? MarketCap { get; set; }
    public int? HolderCount { get; set; }
    public decimal? Liquidity { get; set; }
    public decimal? Top10HoldersPercent { get; set; }
    public decimal? DevHoldingPercent { get; set; }
    public decimal? UsdPrice { get; set; }
    public bool? IsMintDisabled { get; set; }
    public bool? IsFreezeDisabled { get; set; }
    public bool? IsVerified { get; set; }
    public string? Launchpad { get; set; }
    public DateTime? FirstPoolCreatedAt { get; set; }

    // DexScreener API Data
    public decimal? Volume24h { get; set; }
    public decimal? PriceChange24h { get; set; }
    public List<decimal>? PriceHistory { get; set; }  // 24 hourly price points for sparklines

    // Social Links
    public string? TwitterUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    // Cache Management
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }

    // Cache tier determines update frequency
    public CacheTier CacheTier { get; set; } = CacheTier.Cold;

    public TokenMetadata(string contractAddress)
    {
        ContractAddress = contractAddress;
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }
}

/// <summary>
/// Cache tier determines how frequently token metadata is refreshed
/// </summary>
public enum CacheTier
{
    /// <summary>
    /// Top 25 trending tokens - updated every 1-2 minutes
    /// </summary>
    Hot,

    /// <summary>
    /// Recently searched or mentioned tokens - updated every 15 minutes
    /// </summary>
    Warm,

    /// <summary>
    /// Historical tokens - updated only on demand
    /// </summary>
    Cold
}
