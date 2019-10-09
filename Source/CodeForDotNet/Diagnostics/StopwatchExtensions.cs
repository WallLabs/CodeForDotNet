using System;
using System.Diagnostics;

namespace CodeForDotNet.Diagnostics
{
	/// <summary>
	/// Extensions to the <see cref="Stopwatch"/> class.
	/// </summary>
	public static class StopwatchExtensions
	{
		#region Public Fields

		/// <summary>
		/// Number of <see cref="Stopwatch.ElapsedTicks"/> in a microsecond.
		/// </summary>
		public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / 1000000D;

		#endregion Public Fields

		#region Public Methods

		/// <summary>
		/// Gets the total elapsed time in microseconds.
		/// </summary>
		public static long ElapsedMicroseconds(this Stopwatch timer)
		{
			// Validate.
			if (timer is null) throw new ArgumentNullException(nameof(timer));

			// Calculate and return value.
			return (long)(timer.ElapsedTicks / TicksPerMicrosecond);
		}

		/// <summary>
		/// Gets the total elapsed time in microseconds.
		/// </summary>
		public static long GetTimestampInMicroseconds()
		{
			return (long)(Stopwatch.GetTimestamp() / TicksPerMicrosecond);
		}

		#endregion Public Methods
	}
}
