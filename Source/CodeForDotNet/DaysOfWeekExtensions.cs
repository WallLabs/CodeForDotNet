using CodeForDotNet.Properties;
using System;
using System.Collections.Generic;

namespace CodeForDotNet
{
    /// <summary>
    /// Extends <see cref="DaysOfWeek"/> and <see cref="DaysOfWeek"/> types to help with conversion.
    /// </summary>
    public static class DaysOfWeekExtensions
    {
        /// <summary>
        /// Converts a <see cref="DayOfWeek" /> to an <see cref="DaysOfWeek" /> flag.
        /// </summary>
        public static DaysOfWeek ToDaysOfWeek(this DayOfWeek day)
        {
            return (DaysOfWeek)(1 << (int)day);
        }

        /// <summary>
        /// Converts <see cref="DaysOfWeek"/> flags to a collection of <see cref="DayOfWeek"/>.
        /// </summary>
        public static DayOfWeek[] ToArray(this DaysOfWeek days)
        {
            var results = new List<DayOfWeek>();
            if ((days & DaysOfWeek.Monday) != 0) results.Add(DayOfWeek.Monday);
            if ((days & DaysOfWeek.Tuesday) != 0) results.Add(DayOfWeek.Tuesday);
            if ((days & DaysOfWeek.Wednesday) != 0) results.Add(DayOfWeek.Wednesday);
            if ((days & DaysOfWeek.Thursday) != 0) results.Add(DayOfWeek.Thursday);
            if ((days & DaysOfWeek.Friday) != 0) results.Add(DayOfWeek.Friday);
            if ((days & DaysOfWeek.Saturday) != 0) results.Add(DayOfWeek.Saturday);
            if ((days & DaysOfWeek.Sunday) != 0) results.Add(DayOfWeek.Sunday);
            return results.ToArray();
        }

        /// <summary>
        /// Converts <see cref="DaysOfWeek"/> flags to a collection of strings for each day of the week.
        /// </summary>
        public static string[] ToStringArray(this DaysOfWeek days)
        {
            var results = new List<string>();
            if ((days & DaysOfWeek.Monday) != 0) results.Add(Resources.DaysOfWeekMonday);
            if ((days & DaysOfWeek.Tuesday) != 0) results.Add(Resources.DaysOfWeekTuesday);
            if ((days & DaysOfWeek.Wednesday) != 0) results.Add(Resources.DaysOfWeekWednesday);
            if ((days & DaysOfWeek.Thursday) != 0) results.Add(Resources.DaysOfWeekThursday);
            if ((days & DaysOfWeek.Friday) != 0) results.Add(Resources.DaysOfWeekFriday);
            if ((days & DaysOfWeek.Saturday) != 0) results.Add(Resources.DaysOfWeekSaturday);
            if ((days & DaysOfWeek.Sunday) != 0) results.Add(Resources.DaysOfWeekSunday);
            return results.ToArray();
        }

        /// <summary>
        /// Gets the next date on any of the specified weekdays.
        /// </summary>
        public static DateTime GetNext(this DateTime date, DaysOfWeek days)
        {
            // Validate
            if (days == DaysOfWeek.None) throw new ArgumentOutOfRangeException("days");

            // Get next day
            var next = date;
            for (var dayOffset = 1; dayOffset <= 7; dayOffset++)
            {
                next = next.AddDays(1);
                if ((next.DayOfWeek.ToDaysOfWeek() & days) != 0)
                    return next;
            }
            return next;
        }

        /// <summary>
        /// Gets the next date on any of the specified weekdays.
        /// </summary>
        public static DateTimeOffset GetNext(this DateTimeOffset date, DaysOfWeek days)
        {
            // Validate
            if (days == DaysOfWeek.None) throw new ArgumentOutOfRangeException("days");

            // Get next day
            var next = date;
            for (var dayOffset = 1; dayOffset <= 7; dayOffset++)
            {
                next = next.AddDays(1);
                if ((next.DayOfWeek.ToDaysOfWeek() & days) != 0)
                    return next;
            }
            return next;
        }

        /// <summary>
        /// Gets the next specified weekday.
        /// </summary>
        public static DateTime GetNext(this DateTime date, DayOfWeek day)
        {
            return date.AddDays((int)day + 7 - (int)date.DayOfWeek);
        }

        /// <summary>
        /// Gets the next specified weekday.
        /// </summary>
        public static DateTimeOffset GetNext(this DateTimeOffset date, DayOfWeek day)
        {
            return date.AddDays((int)day + 7 - (int)date.DayOfWeek);
        }

        /// <summary>
        /// Gets the first specified weekday of the month.
        /// </summary>
        public static DateTime GetFirst(this DateTime date, DayOfWeek day)
        {
            return date.AddDays(-date.Day).GetNext(day);
        }

        /// <summary>
        /// Gets the first specified weekday of the month.
        /// </summary>
        public static DateTimeOffset GetFirst(this DateTimeOffset date, DayOfWeek day)
        {
            return date.AddDays(-date.Day).GetNext(day);
        }

        /// <summary>
        /// Gets the last specified weekday of the month.
        /// </summary>
        public static DateTime GetLast(this DateTime date, DayOfWeek day)
        {
            var third = date.GetFirst(day).GetNext(day).GetNext(day);
            var fourth = third.GetNext(day);
            if (fourth.Month == third.Month)
                return fourth;
            return third;
        }

        /// <summary>
        /// Gets the last specified weekday of the month.
        /// </summary>
        public static DateTimeOffset GetLast(this DateTimeOffset date, DayOfWeek day)
        {
            var third = date.GetFirst(day).GetNext(day).GetNext(day);
            var fourth = third.GetNext(day);
            if (fourth.Month == third.Month)
                return fourth;
            return third;
        }
    }
}