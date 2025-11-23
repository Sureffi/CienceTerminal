namespace TokenMetrics.Domain.Entities;

/// <summary>
/// Computed aggregate state for a CA's trending metrics.
/// Updated by background job based on raw mention records.
/// </summary>
public class CaAggregate
{
    public string CaAddress { get; set; } = string.Empty;

    // Rolling mention counts
    public double MentionCount5m { get; set; }
    public double MentionCount1h { get; set; }
    public double MentionCount6h { get; set; }
    public double MentionCount24h { get; set; }

    // Trending score and state
    public double TrendScore { get; set; }
    public DateTime LastMentioned { get; set; }
    public DateTime LastCalculated { get; set; }

    // EMA values stored as JSON for flexibility
    // Format: {"5m": 1.2, "1h": 3.4, "6h": 5.6, "24h": 7.8}
    public string EmaValuesJson { get; set; } = "{}";

    // Top 25 tracking
    public int? Rank { get; set; } // null if not in top 25
    public DateTime? EnteredTop25At { get; set; }
}
