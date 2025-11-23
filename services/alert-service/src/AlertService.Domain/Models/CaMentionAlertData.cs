using CienceTerminal.Contracts.Enums;

namespace AlertService.Domain.Models;

/// <summary>
/// Data transfer object containing all information needed to create a CA mention alert.
/// Combines trending data from MentionAggregates with enriched token metadata from Coins table.
/// </summary>
public class CaMentionAlertData
{
    // Alert metadata
    public Guid AlertId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public AlertSeverity Severity { get; set; }
    public int? Rank { get; set; }

    // Token identification
    public string CoinMintAddress { get; set; } = string.Empty;
    public string CoinSymbol { get; set; } = string.Empty;
    public string CoinImageUrl { get; set; } = string.Empty;

    // Trending metrics (from MentionAggregates)
    public double TrendScore { get; set; }
    public double MentionCount24h { get; set; }
    public DateTime LastMentioned { get; set; }

    // Top mentioners (for social proof)
    public List<string> TopMentionerImageUrls { get; set; } = new();

    // Token financial metrics (from Coins table via Jupiter API)
    public double? MarketCap { get; set; }
    public double? Liquidity { get; set; }
    public double? Volume24h { get; set; }
    public double? PriceChange24H { get; set; }
    public int? HolderCount { get; set; }
    public double? TopHoldersPercentage { get; set; }
    public DateTime? FirstPoolCreatedAt { get; set; }

    // Validation flags
    public bool IsBlacklisted { get; set; }
}
