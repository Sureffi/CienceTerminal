using System;

namespace CienceTerminal.Contracts.Events;

/// <summary>
/// Lightweight notification published by Token Metrics Service after mention aggregates are recalculated.
/// Alert Service consumes this event as a trigger to query the database and determine alert conditions.
/// Published every ~1 minute after trending calculation completes.
/// </summary>
public class MentionAggregatesUpdatedEvent
{
    /// <summary>
    /// Timestamp when the aggregation calculation was completed.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}
