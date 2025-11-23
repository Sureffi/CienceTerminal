using System.Text.Json.Serialization;

namespace TokenMetrics.Domain.Entities;

/// <summary>
/// Jupiter API response data structure
/// Maps to Jupiter Lite API v1 response format
/// </summary>
public class JupiterTokenData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;  // Mint address

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }

    [JsonPropertyName("twitter")]
    public string? Twitter { get; set; }

    [JsonPropertyName("telegram")]
    public string? Telegram { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("dev")]
    public string? Dev { get; set; }

    [JsonPropertyName("circSupply")]
    public double? CircSupply { get; set; }

    [JsonPropertyName("totalSupply")]
    public double? TotalSupply { get; set; }

    [JsonPropertyName("tokenProgram")]
    public string? TokenProgram { get; set; }

    [JsonPropertyName("launchpad")]
    public string? Launchpad { get; set; }

    [JsonPropertyName("partnerConfig")]
    public string? PartnerConfig { get; set; }

    [JsonPropertyName("graduatedPool")]
    public string? GraduatedPool { get; set; }

    [JsonPropertyName("graduatedAt")]
    public string? GraduatedAt { get; set; }

    [JsonPropertyName("holderCount")]
    public int? HolderCount { get; set; }

    [JsonPropertyName("fdv")]
    public double? Fdv { get; set; }  // Fully diluted valuation

    [JsonPropertyName("mcap")]
    public double? MarketCap { get; set; }

    [JsonPropertyName("usdPrice")]
    public double? UsdPrice { get; set; }

    [JsonPropertyName("priceBlockId")]
    public int? PriceBlockId { get; set; }

    [JsonPropertyName("liquidity")]
    public double? Liquidity { get; set; }

    [JsonPropertyName("stats5m")]
    public JupiterTokenStats? Stats5m { get; set; }

    [JsonPropertyName("stats1h")]
    public JupiterTokenStats? Stats1h { get; set; }

    [JsonPropertyName("stats6h")]
    public JupiterTokenStats? Stats6h { get; set; }

    [JsonPropertyName("stats24h")]
    public JupiterTokenStats? Stats24h { get; set; }

    [JsonPropertyName("firstPool")]
    public JupiterFirstPool? FirstPool { get; set; }

    [JsonPropertyName("audit")]
    public JupiterAuditData? Audit { get; set; }

    [JsonPropertyName("organicScore")]
    public double? OrganicScore { get; set; }

    [JsonPropertyName("organicScoreLabel")]
    public string? OrganicScoreLabel { get; set; }

    [JsonPropertyName("isVerified")]
    public bool? IsVerified { get; set; }

    [JsonPropertyName("cexes")]
    public List<string>? Cexes { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}

public class JupiterFirstPool
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}

public class JupiterTokenStats
{
    [JsonPropertyName("priceChange")]
    public double? PriceChange { get; set; }

    [JsonPropertyName("holderChange")]
    public double? HolderChange { get; set; }

    [JsonPropertyName("liquidityChange")]
    public double? LiquidityChange { get; set; }

    [JsonPropertyName("volumeChange")]
    public double? VolumeChange { get; set; }

    [JsonPropertyName("buyVolume")]
    public double? BuyVolume { get; set; }

    [JsonPropertyName("sellVolume")]
    public double? SellVolume { get; set; }

    [JsonPropertyName("buyOrganicVolume")]
    public double? BuyOrganicVolume { get; set; }

    [JsonPropertyName("sellOrganicVolume")]
    public double? SellOrganicVolume { get; set; }

    [JsonPropertyName("numBuys")]
    public int? NumBuys { get; set; }

    [JsonPropertyName("numSells")]
    public int? NumSells { get; set; }

    [JsonPropertyName("numTraders")]
    public int? NumTraders { get; set; }

    [JsonPropertyName("numOrganicBuyers")]
    public int? NumOrganicBuyers { get; set; }

    [JsonPropertyName("numNetBuyers")]
    public int? NumNetBuyers { get; set; }
}

public class JupiterAuditData
{
    [JsonPropertyName("isSus")]
    public bool? IsSus { get; set; }

    [JsonPropertyName("mintAuthorityDisabled")]
    public bool? MintAuthorityDisabled { get; set; }

    [JsonPropertyName("freezeAuthorityDisabled")]
    public bool? FreezeAuthorityDisabled { get; set; }

    [JsonPropertyName("topHoldersPercentage")]
    public double? TopHoldersPercentage { get; set; }

    [JsonPropertyName("devBalancePercentage")]
    public double? DevBalancePercentage { get; set; }

    [JsonPropertyName("blockaidRugpull")]
    public bool? BlockaidRugpull { get; set; }

    [JsonPropertyName("devMigrations")]
    public int? DevMigrations { get; set; }
}
