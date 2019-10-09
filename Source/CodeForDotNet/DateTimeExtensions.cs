using System;

namespace CodeForDotNet
{
	/// <summary>
	/// Extends the <see cref="DateTime"/> class.
	/// </summary>
	public static class DateTimeExtensions
	{
		#region Public Methods

		/// <summary>
		/// Gets the end of the day, accurate to the millisecond.
		/// </summary>
		public static DateTime GetEndOfDay(this DateTime value)
		{
			return value.AddDays(1).AddMilliseconds(-1);
		}

		/// <summary>
		/// Gets the end of the day, accurate to the millisecond.
		/// </summary>
		public static DateTimeOffset GetEndOfDay(this DateTimeOffset value)
		{
			return value.AddDays(1).AddMilliseconds(-1);
		}

		/// <summary>
		/// Gets the end of the month, accurate to the millisecond.
		/// </summary>
		public static DateTime GetEndOfMonth(this DateTime value)
		{
			return value.Date.AddDays(-value.Day + 1).AddMonths(1).AddMilliseconds(-1);
		}

		/// <summary>
		/// Gets the end of the month, accurate to the millisecond.
		/// </summary>
		public static DateTimeOffset GetEndOfMonth(this DateTimeOffset value)
		{
			return value.Date.AddDays(-value.Day + 1).AddMonths(1).AddMilliseconds(-1);
		}

		/// <summary>
		/// Gets the start of the month.
		/// </summary>
		public static DateTime GetStartOfMonth(this DateTime value)
		{
			return value.Date.AddDays(-value.Day + 1);
		}

		/// <summary>
		/// Gets the start of the month.
		/// </summary>
		public static DateTimeOffset GetStartOfMonth(this DateTimeOffset value)
		{
			return value.Date.AddDays(-value.Day + 1);
		}

		/// <summary>
		/// Sets the <see cref="DateTime.Date"/> value.
		/// </summary>
		public static DateTime SetDate(this DateTime value, DateTime date)
		{
			return value.Add(date.Date.Subtract(value.Date));
		}

		/// <summary>
		/// Sets the <see cref="DateTimeOffset.Date"/> value.
		/// </summary>
		public static DateTimeOffset SetDate(this DateTimeOffset value, DateTimeOffset date)
		{
			return value.Add(date.Date.Subtract(value.Date));
		}

		/// <summary>
		/// Sets the <see cref="DateTime.TimeOfDay"/> value.
		/// </summary>
		public static DateTime SetTime(this DateTime value, TimeSpan time)
		{
			return value.Add(time.Subtract(value.TimeOfDay));
		}

		/// <summary>
		/// Sets the <see cref="DateTimeOffset.TimeOfDay"/> value.
		/// </summary>
		public static DateTimeOffset SetTime(this DateTimeOffset value, TimeSpan time)
		{
			return value.Add(time.Subtract(value.TimeOfDay));
		}

		/// <summary>
		/// Converts a nullable <see cref="DateTime"/> to local time, if it is not null, otherwise does nothing. Used to eliminate
		/// <see cref="Nullable&lt;T&gt;.HasValue"/> checking code and avoid exceptions when null.
		/// </summary>
		public static DateTime? ToLocalTime(this DateTime? value)
		{
			return !value.HasValue ? (DateTime?)null : value.Value.ToLocalTime();
		}

		/// <summary>
		/// Converts a nullable <see cref="DateTimeOffset"/> to local time, if it is not null, otherwise does nothing. Used to eliminate
		/// <see cref="Nullable&lt;T&gt;.HasValue"/> checking code and avoid exceptions when null.
		/// </summary>
		public static DateTimeOffset? ToLocalTime(this DateTimeOffset? value)
		{
			return !value.HasValue ? (DateTimeOffset?)null : value.Value.ToLocalTime();
		}

		/// <summary>
		/// Converts a nullable <see cref="DateTime"/> to universal time, if it is not null, otherwise does nothing. Used to eliminate
		/// <see cref="Nullable&lt;T&gt;.HasValue"/> checking code and avoid exceptions when null.
		/// </summary>
		public static DateTime? ToUniversalTime(this DateTime? value)
		{
			return !value.HasValue ? (DateTime?)null : value.Value.ToUniversalTime();
		}

		/// <summary>
		/// Converts a nullable <see cref="DateTimeOffset"/> to universal time, if it is not null, otherwise does nothing. Used to eliminate
		/// <see cref="Nullable&lt;T&gt;.HasValue"/> checking code and avoid exceptions when null.
		/// </summary>
		public static DateTimeOffset? ToUniversalTime(this DateTimeOffset? value)
		{
			return !value.HasValue ? (DateTimeOffset?)null : value.Value.ToUniversalTime();
		}

		/// <summary>
		/// Trims to a specific accuracy. Use TimeSpan.TicksPer... constants, e.g. <see cref="TimeSpan.TicksPerSecond"/> to round to the nearest second.
		/// </summary>
		public static DateTime Trim(this DateTime date, long roundTicks)
		{
			return new DateTime(date.Ticks - date.Ticks % roundTicks);
		}

		/// <summary>
		/// Trims to a specific accuracy. Use TimeSpan.TicksPer... constants, e.g. <see cref="TimeSpan.TicksPerSecond"/> to round to the nearest second.
		/// </summary>
		public static DateTimeOffset Trim(this DateTimeOffset date, long roundTicks)
		{
			return new DateTimeOffset(date.Ticks - date.Ticks % roundTicks, TimeZoneInfo.Local.BaseUtcOffset);
		}

		/// <summary>
		/// Truncates the accuracy of a <see cref="DateTimeOffset"/> from the default 100 nanosecond accuracy to the more common lesser accuracy of milliseconds,
		/// e.g. JSON, JavaScript and UNIX compatible.
		/// </summary>
		public static DateTimeOffset TruncateToMilliseconds(this DateTimeOffset value)
		{
			return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, new TimeSpan(0, value.Offset.Minutes, 0));
		}

		/// <summary>
		/// Truncates the accuracy of a <see cref="DateTime"/> from the default 100 nanosecond accuracy to the more common lesser accuracy of milliseconds, e.g.
		/// JSON, JavaScript and UNIX compatible.
		/// </summary>
		public static DateTime TruncateToMilliseconds(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		#endregion Public Methods
	}
}
