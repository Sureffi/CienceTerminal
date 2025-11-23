using System.Collections.Concurrent;

namespace TokenMetrics.Application.Trending;

/// <summary>
/// Maintains a rolling window of mention timestamps for calculating counts over time periods.
/// </summary>
public class TimeBucketCounter
{
    private readonly int _windowMinutes;
    private readonly ConcurrentQueue<(DateTime Timestamp, int Count)> _buckets = new();

    public TimeBucketCounter(int windowMinutes)
    {
        _windowMinutes = windowMinutes;
    }

    public void AddMention(DateTime timestamp)
    {
        _buckets.Enqueue((timestamp, 1));
        Cleanup(timestamp);
    }

    public int GetRollingCount(DateTime now)
    {
        Cleanup(now);
        return _buckets.Sum(b => b.Count);
    }

    private void Cleanup(DateTime now)
    {
        while (_buckets.TryPeek(out var bucket) &&
               (now - bucket.Timestamp).TotalMinutes > _windowMinutes)
        {
            _buckets.TryDequeue(out _);
        }
    }
}
