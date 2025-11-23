using AlertService.Domain.Entities;
using CienceTerminal.Contracts.Enums;

namespace AlertService.Application.Alerts;

public class CaMentionAlert : Alert
{
    // Token identification
    public string CoinAddress { get; set; } // Needed for dex, axiom links etc.
    public string CoinSymbol { get; set; } // Needed for display
    public string CoinImageUrl { get; set; } = string.Empty;

    // Trending metrics
    public int? Rank { get; set; }
    public double TrendScore { get; set; }
    public double MentionCount24Hour { get; set; }
    public DateTime? LastMentioned { get; set; }

    // Social proof
    public List<string> TopMentionerImageUrls { get; set; } = new();

    // Token financial metrics
    public double? MarketCap { get; set; }
    public double? Liquidity { get; set; }
    public double? Volume24h { get; set; }
    public double? PriceChange24H { get; set; }
    public int? HolderCount { get; set; }
    public double? TopHoldersPercentage { get; set; }
    public DateTime? FirstPoolCreatedAt { get; set; }

    public override AlertType Type { get; set; } = AlertType.CaMention;

    // Change these in the future for notifications
    public override string Title => Rank.HasValue ? $"#{Rank} Trending Token" : "Trending Token";
    public override string Message => $"{CoinSymbol} is trending with {MentionCount24Hour:F0} mentions in 24h";

    public CaMentionAlert(string coinAddress, string coinSymbol = "")
    {
        CoinAddress = coinAddress;
        CoinSymbol = coinSymbol;
    }

    public void UpdateCoinMetrics(Coin coin)
    {
        MentionCount24Hour = coin.MentionCount24h;
        MarketCap = coin.MarketCap;
        Liquidity = coin.Liquidity;
        Volume24h = coin.Volume24h;
        PriceChange24H = coin.PriceChange24H;
        HolderCount = coin.HolderCount;
        TopHoldersPercentage = coin.TopHoldersPercentage;
        FirstPoolCreatedAt = coin.FirstPoolCreatedAt;

        // Only udpate if not null (Preserve previous values)
        if (coin.CoinImage is not null)
            CoinImageUrl = coin.CoinImage;
        if (coin.CoinSymbol is not null)
            CoinSymbol = coin.CoinSymbol;
    }
}
