using CienceTerminal.Contracts.Models;

namespace TokenMetrics.Application.Trending;

/// <summary>
/// Calculates trending scores for CA mentions using EMA-based velocity tracking.
/// Ported from Twitter Scanner's CaMentionTracking logic.
/// </summary>
public class TrendingCalculator
{
    public static TrendingState CalculateTrend(
        List<CaMentionRecord> mentions,
        string? previousEmaJson = null)
    {
        var now = DateTime.UtcNow;

        // Initialize EMA tracker
        var trend = new TokenTrend(previousEmaJson);

        // Create time bucket counters
        var counter5m = new TimeBucketCounter(5);
        var counter1h = new TimeBucketCounter(60);
        var counter6h = new TimeBucketCounter(360);
        var counter24h = new TimeBucketCounter(1440);

        // Process all mentions
        foreach (var mention in mentions)
        {
            counter5m.AddMention(mention.Timestamp);
            counter1h.AddMention(mention.Timestamp);
            counter6h.AddMention(mention.Timestamp);
            counter24h.AddMention(mention.Timestamp);
        }

        // Get rolling counts
        double count5m = counter5m.GetRollingCount(now);
        double count1h = counter1h.GetRollingCount(now);
        double count6h = counter6h.GetRollingCount(now);
        double count24h = counter24h.GetRollingCount(now);

        // Update EMAs
        trend.UpdateMentions(TimeSpan.FromMinutes(5), count5m);
        trend.UpdateMentions(TimeSpan.FromHours(1), count1h);
        trend.UpdateMentions(TimeSpan.FromHours(6), count6h);
        trend.UpdateMentions(TimeSpan.FromHours(24), count24h);

        double trendScore = trend.ComputeTrendScore(count24h);

        return new TrendingState
        {
            MentionCount5m = count5m,
            MentionCount1h = count1h,
            MentionCount6h = count6h,
            MentionCount24h = count24h,
            TrendScore = trendScore,
            EmaValuesJson = trend.SerializeEmas()
        };
    }
}

public class TrendingState
{
    public double MentionCount5m { get; set; }
    public double MentionCount1h { get; set; }
    public double MentionCount6h { get; set; }
    public double MentionCount24h { get; set; }
    public double TrendScore { get; set; }
    public string EmaValuesJson { get; set; } = "{}";
}
