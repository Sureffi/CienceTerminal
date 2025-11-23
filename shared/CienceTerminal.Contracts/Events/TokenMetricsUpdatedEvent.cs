namespace CienceTerminal.Contracts.Events;

/// <summary>
/// Published by Token Metrics Service when token metadata is refreshed
/// Consumed by Alert Service to update CaMentionAlert with latest metrics
/// </summary>
public class TokenMetricsUpdatedEvent
{
    public string CoinMintAddress { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    // Basic Token Info
    public string Symbol { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? IconUrl { get; set; }

    // Jupiter API Data - Financial Metrics
    public double? MarketCap { get; set; }
    public double? Liquidity { get; set; }
    public double? UsdPrice { get; set; }

    // Jupiter API Data - Holder Information
    public int? HolderCount { get; set; }
    public double? Top10HoldersPercent { get; set; }
    public double? DevHoldingPercent { get; set; }

    // Jupiter API Data - Security
    public bool? IsMintDisabled { get; set; }
    public bool? IsFreezeDisabled { get; set; }

    // Jupiter API Data - Token Info
    public bool? IsVerified { get; set; }
    public string? Launchpad { get; set; }
    public DateTime? FirstPoolCreatedAt { get; set; }

    // Volume data (future - from DexScreener)
    public double? Volume24h { get; set; }
    public double? PriceChange24h { get; set; }
}
