using System;
using System.Diagnostics;

namespace CodeForDotNet.Diagnostics;

/// <summary>
/// Filters trace events for a specific activity ID.
/// </summary>
public class ActivityIdTraceFilter(Guid id) : TraceFilter
{

    /// <summary>
    /// Activity ID to capture (all others are filtered).
    /// </summary>
    public Guid ActivityId { get; set; } = id;

    /// <summary>
    /// Filters events.
    /// </summary>
    public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
    {
        return Trace.CorrelationManager.ActivityId == ActivityId;
    }
}
