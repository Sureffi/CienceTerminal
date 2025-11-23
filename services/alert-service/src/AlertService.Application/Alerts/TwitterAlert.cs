using AlertService.Domain.Entities;
using CienceTerminal.Contracts.Enums;

namespace AlertService.Application.Alerts;

// TODO: Figure out if this structure is good 
public class TwitterAlert : Alert
{
    public override AlertType Type { get; set; }

    public override string Title { get; } = "Twitter Alert";

    public override string Message { get; } = "Placeholder";

    public string TweetLink { get; set; } = string.Empty;

    // Author info
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorProfilePicture { get; set; } = string.Empty;
    public int AuthorFollowers { get; set; }
    public bool AuthorIsBlueVerified { get; set; }
    public bool AuthorIsGoldVerified { get; set; }

    public string TweetContent { get; set; } = string.Empty;

    // Coin info
    public string CoinName { get; set; } = string.Empty;
    public string CoinSymbol { get; set; } = string.Empty;
    public string CoinMint { get; set; } = string.Empty;
    public string? CoinImageUrl { get; set; }
    public string? Launchpad { get; set; }

    public DateTime? CoinFirstPoolCreatedAt { get; set; }
    public double? CoinMarketCap { get; set; }
    public int? CoinHolderCount { get; set; }
    public double? CoinLiquidity { get; set; }
    public double? CoinVolume24h { get; set; }
    public int? CoinMentionCount24h { get; set; }
    public double? CoinPriceChange24H { get; set; }
    public double? CoinTop10Holders { get; set; }
    public double? CoinDevHolding { get; set; }
    public bool? CoinIsFreezeDisabled { get; set; }
    public bool? CoinIsMintDisabled { get; set; }

    public string? CoinAge
    {
        get
        {
            if (CoinFirstPoolCreatedAt == null) return null;

            var age = DateTime.UtcNow - CoinFirstPoolCreatedAt.Value;

            if (age.TotalDays >= 1)
                return $"{(int)age.TotalDays}d";
            if (age.TotalHours >= 1)
                return $"{(int)age.TotalHours}h";
            if (age.TotalMinutes >= 1)
                return $"{(int)age.TotalMinutes}m";

            return "<1m";
        }
    }

    public void UpdateCoinMetrics(Coin coin)
    {
        CoinHolderCount = coin.HolderCount;
        CoinMarketCap = coin.MarketCap;
        CoinLiquidity = coin.Liquidity;
        CoinVolume24h = coin.Volume24h;
        CoinMentionCount24h = coin.MentionCount24h;
        CoinPriceChange24H = coin.PriceChange24H;
        CoinTop10Holders = coin.TopHoldersPercentage;
        CoinFirstPoolCreatedAt = coin.FirstPoolCreatedAt;

        // Only update if not null (Preserve old values)
        if (coin.CoinSymbol is not null)
            CoinSymbol = coin.CoinSymbol;
        if (coin.CoinImage is not null)
            CoinImageUrl = coin.CoinImage;
    }
}


