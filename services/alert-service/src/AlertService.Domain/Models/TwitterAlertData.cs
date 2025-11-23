using CienceTerminal.Contracts.Enums;

namespace AlertService.Domain.Models;

/// <summary>
/// Data transfer object containing all information needed to create a Twitter alert.
/// Combines Twitter event data with enriched token metadata from Coins table.
/// </summary>
public class TwitterAlertData
{
    // Alert metadata
    public Guid AlertId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }

    // Tweet information
    public string TweetLink { get; set; } = string.Empty;
    public string TweetContent { get; set; } = string.Empty;

    // Author information
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorProfilePicture { get; set; } = string.Empty;
    public int AuthorFollowers { get; set; }
    public bool AuthorIsBlueVerified { get; set; }
    public bool AuthorIsGoldVerified { get; set; }

    // Token identification
    public string CoinMintAddress { get; set; } = string.Empty;
    public string CoinSymbol { get; set; } = string.Empty;
    public string? CoinImageUrl { get; set; }

    // Token financial metrics (from Coins table via Jupiter API)
    public double? MarketCap { get; set; }
    public double? Liquidity { get; set; }
    public double? Volume24h { get; set; }
    public int? HolderCount { get; set; }
    public double? TopHoldersPercentage { get; set; }
    public DateTime? FirstPoolCreatedAt { get; set; }

    // Validation flags
    public bool IsBlacklisted { get; set; }
}
