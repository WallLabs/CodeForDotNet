﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Serialization;

namespace CodeForDotNet.Data
{
    /// <summary>
    /// Defines a schedule with multiple include and exclude patterns, supporting recurrence and durations.
    /// </summary>
    [XmlRoot(XmlRootName, Namespace = XmlNamespace)]
    [XmlType(XmlTypeName, Namespace = XmlNamespace)]
    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Settable reference properties which would normally be read-only must be allowed to support system XML serializer.")]
    public class Schedule
    {
        #region Constants

        /// <summary>
        /// XML root element name.
        /// </summary>
        public const string XmlRootName = nameof(Schedule);

        /// <summary>
        /// XML type name.
        /// </summary>
        public const string XmlTypeName = XmlRootName + "Type";

        /// <summary>
        /// XML namespace.
        /// </summary>
        public const string XmlNamespace = Constants.XmlRootNamespace;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public Schedule()
        {
            Includes = new ScheduleItemCollection();
            Excludes = new ScheduleItemCollection();
        }

        #endregion Lifetime

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(Schedule schedule1, Schedule schedule2)
        {
            return schedule1?.Equals(schedule2) ?? schedule2 is null;
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(Schedule schedule1, Schedule schedule2)
        {
            return !(schedule1?.Equals(schedule2) ?? schedule2 is null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object other)
        {
            // Compare nullability and type
            if (!(other is Schedule otherSchedule))
                return false;

            // Compare values
            return
                otherSchedule.Description == Description &&
                otherSchedule.Includes == Includes &&
                otherSchedule.Excludes == Excludes;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                (Description ?? string.Empty).GetHashCode() ^
                (Includes != null ? Includes.GetHashCode() : 0) ^
                (Excludes != null ? Excludes.GetHashCode() : 0);
        }

        #endregion Operators

        #region Public Properties

        /// <summary>
        /// Description, optional. The user can give a short description for the schedule.
        /// </summary>
        [XmlElement("description")]
        public string Description
        {
            get { return StringExtensions.NullWhenEmpty(_description) ?? ToString(CultureInfo.CurrentCulture); }
            set { _description = value; }
        }

        private string _description;

        /// <summary>
        /// Times at which this schedule occurs. Must be combined with the <see cref="Excludes"/> to decide if it is due at any particular time.
        /// </summary>
        [XmlArray("includes"), XmlArrayItem("scheduleItem")]
        public ScheduleItemCollection Includes { get; set; }

        /// <summary>
        /// Omits the <see cref="Includes"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool IncludesSpecified { get { return Includes != null && Includes.Count > 0; } }

        /// <summary>
        /// Times at which this schedule is must not occur. Allows special times to be excluded from the <see cref="Includes"/>.
        /// </summary>
        [XmlArray("excludes"), XmlArrayItem("scheduleItem")]
        public ScheduleItemCollection Excludes { get; set; }

        /// <summary>
        /// Omits the <see cref="Excludes"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool ExcludesSpecified { get { return Excludes != null && Excludes.Count > 0; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a string describing this schedule entry,
        /// using the <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a string describing this schedule entry,
        /// using the specified <see cref="CultureInfo"/>.
        /// </summary>
        public string ToString(CultureInfo culture)
        {
            if (Includes != null && Includes.Count > 0)
            {
                if (Excludes != null && Excludes.Count > 0)
                {
                    // Format string with includes and excludes
                    return string.Format(culture,
                        Properties.Resources.ScheduleToStringFormatIncludesAndExcludes,
                        Includes.ToString(culture), Excludes.ToString(culture));
                }

                // Format string with includes only
                return string.Format(culture,
                                     Properties.Resources.ScheduleToStringFormatIncludesOnly,
                                     Includes.ToString(culture));
            }

            // Format string with no schedule items
            return string.Format(culture, Properties.Resources.ScheduleToStringFormatNone);
        }

        /// <summary>
        /// Clears all items from the schedule.
        /// </summary>
        public void Clear()
        {
            Includes.Clear();
            Excludes.Clear();
        }

        /// <summary>
        /// Returns the next occurrence after the specified date, taking into account both <see cref="Includes"/> and <see cref="Excludes"/>.
        /// </summary>
        /// <param name="utcDate">Date to check in UTC, usually <see cref="DateTime.UtcNow"/> or the last event date when it is required to know if events are missed.</param>
        /// <param name="duration">
        /// Duration in minutes. Must be less than each include duration otherwise it will never match.
        /// </param>
        /// <returns>Next scheduled date or null when the schedule has ended.</returns>
        public DateTimeOffset? GetNext(DateTimeOffset utcDate, int duration)
        {
            DateTimeOffset? next = null;
            DateTimeOffset? last = utcDate;
            do
            {
                // Check all includes
                var current = last.Value;
                last = null;
                foreach (var include in Includes)
                {
                    // Skip items with shorter durations
                    if (include.Duration < duration)
                        continue;

                    // Get next date
                    var includeDate = include.GetNext(current);
                    if (!includeDate.HasValue)
                        continue;

                    // Store lowest date for next pass (in case fully excluded)
                    if (!last.HasValue || includeDate.Value < last.Value)
                        last = includeDate.Value.AddMinutes(include.Duration);

                    // Check all excludes
                    foreach (var exclude in Excludes)
                    {
                        // Cancel include when contained within exclude
                        Debug.Assert(includeDate != null);
                        if (exclude.Contains(includeDate.Value, duration))
                            includeDate = null;
                    }

                    // Store as next date when not excluded and lower than curent value
                    if (includeDate.HasValue && (!next.HasValue || includeDate.Value < next.Value))
                        next = includeDate;
                }
            }
            while (last.HasValue && !next.HasValue);

            // Return result
            return next;
        }

        #endregion Public Methods
    }
}