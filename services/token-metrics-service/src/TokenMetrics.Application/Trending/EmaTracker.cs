namespace TokenMetrics.Application.Trending;

/// <summary>
/// Exponential Moving Average tracker for smoothing mention counts over time.
/// </summary>
public class EmaTracker
{
    public double Alpha { get; }
    public double Value { get; private set; }
    public bool Initialized { get; private set; }

    public EmaTracker(double alpha, double? initialValue = null)
    {
        Alpha = alpha;
        if (initialValue.HasValue)
        {
            Value = initialValue.Value;
            Initialized = true;
        }
    }

    public double Update(double newValue)
    {
        if (!Initialized)
        {
            Value = newValue;
            Initialized = true;
        }
        else
        {
            Value = (Alpha * newValue) + ((1 - Alpha) * Value);
        }
        return Value;
    }
}
