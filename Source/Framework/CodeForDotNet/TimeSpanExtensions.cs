using System;

namespace CodeForDotNet;

/// <summary>
/// Extensions to the <see cref="TimeSpan"/> class.
/// </summary>
public static class TimeSpanExtensions
{
    #region Public Fields

    /// <summary>
    /// Number of <see cref="TimeSpan.Ticks"/> in a microsecond.
    /// </summary>
    public static readonly double TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000D;

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    /// Creates a <see cref="TimeSpan"/> from an interval specified in microseconds.
    /// </summary>
    public static TimeSpan FromMicroseconds(long microseconds)
    {
        return TimeSpan.FromTicks((long)(microseconds * TicksPerMicrosecond));
    }

    /// <summary>
    /// Gets the total elapsed time in microseconds.
    /// </summary>
    public static long TotalMicroseconds(this TimeSpan span)
    {
        return (long)(span.Ticks / TicksPerMicrosecond);
    }

    #endregion Public Methods
}
