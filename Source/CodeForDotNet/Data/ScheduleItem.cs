using System;
using System.Globalization;
using System.Xml.Serialization;

namespace CodeForDotNet.Data
{
    /// <summary>
    /// Schedule item; describes a single or recurring point in time and a duration in which the item is valid.
    /// </summary>
    [XmlRoot(XmlRootName, Namespace = XmlNamespace)]
    [XmlType(XmlTypeName, Namespace = XmlNamespace)]
    public class ScheduleItem
    {
        #region Constants

        /// <summary>
        /// XML root element name.
        /// </summary>
        public const string XmlRootName = nameof(ScheduleItem);

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
        public ScheduleItem()
        {
            TimeZoneId = TimeZoneInfo.Local.StandardName;
            _duration = 1;
            Start = DateTimeOffset.Now;   // Setter will populate End property using Duration
        }

        #endregion Lifetime

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(ScheduleItem scheduleItem1, ScheduleItem scheduleItem2)
        {
            return scheduleItem1?.Equals(scheduleItem2) ?? scheduleItem2 == null;
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(ScheduleItem scheduleItem1, ScheduleItem scheduleItem2)
        {
            return !(scheduleItem1?.Equals(scheduleItem2) ?? scheduleItem2 == null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object something)
        {
            // Compare nullability and type
            if (!(something is ScheduleItem other) || other is null)
                return false;

            // Compare values
            return
                other.TimeZoneId == TimeZoneId &&
                other.Start == Start &&
                other.Duration == Duration &&
                other.Recurrence == Recurrence &&
                other.Interval == Interval &&
                other.Days == Days;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                (TimeZoneId ?? "").GetHashCode() ^
                Start.GetHashCode() ^
                Duration.GetHashCode() ^
                Recurrence.GetHashCode() ^
                Interval.GetHashCode() ^
                Days.GetHashCode();
        }

        #endregion Operators

        #region Public Properties

        /// <summary>
        /// ID of the time zone in which this schedule item was defined.
        /// All dates, times, days, months and years expressed this schedule are expressed in this time zone (normally the user's local time).
        /// The time zone is applied to current time in order to compare whether it matches the schedule or not. This must work on different
        /// servers across the world than where the user is located, especially significant for global intranet and internet applications,
        /// e.g. an item scheduled in the UK at 23:30 on Tuesday needs to be processed at 00:30 on Wednesday by server in Germany.
        /// </summary>
        [XmlElement("timeZone")]
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Date and time of a single occurrence, or start date and time of a recurring schedule item.
        /// </summary>
        [XmlElement("start")]
        public DateTimeOffset Start
        {
            get { return _start; }
            set
            {
                // Set value
                _start = value;

                // Set related properties
                if (Days != DaysOfWeek.None) Days = value.DayOfWeek.ToDaysOfWeek();
            }
        }

        private DateTimeOffset _start;

        /// <summary>
        /// Duration in minutes.
        /// </summary>
        [XmlElement("duration")]
        public int Duration
        {
            get { return _duration; }
            set
            {
                // Validate
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Set value
                _duration = value;
            }
        }

        private int _duration;

        /// <summary>
        /// Date and time when the schedule ends, taking into account duration and all occurrences.
        /// Will be null when this item has <see cref="Recurrence"/> with an unlimited number of <see cref="Occurrences"/>.
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset? End
        {
            get
            {
                // Add start and duration
                var end = _start.AddMinutes(_duration);

                // Add any recurrences
                if (Recurrence != ScheduleItemRecurrence.None && Occurrences.HasValue)
                {
                    for (int occurence = 1; occurence < Occurrences; occurence++)
                    {
                        // Get next start date
                        var next = GetNext(end);

                        // Move end date when next start returned
                        if (next.HasValue)
                            end = next.Value.AddMinutes(Duration);
                    }
                }

                // Return result
                return end;
            }
        }

        /// <summary>
        /// Recurrence of the schedule, or null for a one-off event.
        /// </summary>
        [XmlElement("recurrence")]
        public ScheduleItemRecurrence Recurrence
        {
            get { return _recurrence; }
            set
            {
                // Set value
                _recurrence = value;

                // Set related properties
                Offset = ScheduleItemOffset.None;
                if (value == ScheduleItemRecurrence.None)
                {
                    // No recurrence
                    Interval = 0;
                    Days = DaysOfWeek.None;
                }
                else
                {
                    // Default recurrence (every 1)
                    Interval = 1;
                    Days = value == ScheduleItemRecurrence.Weekly
                        ? Start.DayOfWeek.ToDaysOfWeek() : DaysOfWeek.None;
                }
            }
        }

        private ScheduleItemRecurrence _recurrence;

        /// <summary>
        /// Omits the <see cref="Recurrence"/> property from XML serialization when it is default.
        /// </summary>
        [XmlIgnore]
        public bool RecurrenceSpecified { get { return Recurrence != ScheduleItemRecurrence.None; } }

        /// <summary>
        /// Gets or sets the recurrence interval.
        /// </summary>
        /// <remarks>
        /// Range depends on <see cref="Recurrence"/>.
        /// None = 0.
        /// Hourly = 0-23 hours (for 24+ hours use daily recurrence).
        /// Daily = 0-30 days (for 31+ days use monthly recurrence).
        /// Weekly = 0-51 weeks (for 52+ weeks use yearly recurrence).
        /// Months = 0-11 months (for 11+ months use yearly recurrence).
        /// Yearly = 0-4 years (more than 4 years is unrealistic, e.g. 4 could be used for leap years).
        /// </remarks>
        [XmlElement("interval")]
        public int Interval
        {
            get { return _interval; }
            set
            {
                // Validate
                switch (Recurrence)
                {
                    case ScheduleItemRecurrence.None:
                        if (value != 0)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    case ScheduleItemRecurrence.Hourly:
                        if (value < 0 || value > 23)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    case ScheduleItemRecurrence.Daily:
                        if (value < 0 || value > 30)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    case ScheduleItemRecurrence.Weekly:
                        if (value < 0 || value > 51)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    case ScheduleItemRecurrence.Monthly:
                        if (value < 0 || value > 11)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    case ScheduleItemRecurrence.Yearly:
                        if (value < 0 || value > 4)
                            throw new ArgumentOutOfRangeException(nameof(value));
                        break;

                    default:
                        // All cases should be covered
                        throw new NotImplementedException();
                }

                // Set value when valid
                _interval = value;
            }
        }

        private int _interval;

        /// <summary>
        /// Omits the <see cref="Interval"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool IntervalSpecified { get { return Interval > 0; } }

        /// <summary>
        /// Optional offset when a relative recurrence is required, e.g. last day of a month
        /// cannot be expressed with day numbers as each month varies in length.
        /// </summary>
        [XmlElement("offset")]
        public ScheduleItemOffset Offset
        {
            get { return _offset; }
            set
            {
                // Validate
                if (value != ScheduleItemOffset.None &&
                    Recurrence != ScheduleItemRecurrence.Monthly &&
                    Recurrence != ScheduleItemRecurrence.Yearly)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Set value
                _offset = value;

                // Set related properties
                if (_offset != ScheduleItemOffset.None)
                {
                    // Set default when offset is applied
                    Days = Start.DayOfWeek.ToDaysOfWeek();
                }
                else
                {
                    // Set default when offset is removed
                    Days = Recurrence == ScheduleItemRecurrence.Weekly
                        ? Start.DayOfWeek.ToDaysOfWeek() : DaysOfWeek.None;
                }
            }
        }

        private ScheduleItemOffset _offset;

        /// <summary>
        /// Omits the <see cref="Offset"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool OffsetSpecified { get { return Offset != ScheduleItemOffset.None; } }

        /// <summary>
        /// Days of week, when one or more days are relevant.
        /// </summary>
        [XmlElement("days")]
        public DaysOfWeek Days
        {
            get { return _days; }
            set
            {
                // Validate
                if (!(value != DaysOfWeek.None && Recurrence == ScheduleItemRecurrence.Weekly) &&
                    !(value == DaysOfWeek.None && Recurrence != ScheduleItemRecurrence.Weekly) &&
                    !(value != DaysOfWeek.None && Offset != ScheduleItemOffset.None &&
                        (Recurrence == ScheduleItemRecurrence.Monthly || Recurrence == ScheduleItemRecurrence.Yearly)))
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Set value when valid
                _days = value;
            }
        }

        private DaysOfWeek _days;

        /// <summary>
        /// Omits the <see cref="Days"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool DaysSpecified { get { return Days != DaysOfWeek.None; } }

        /// <summary>
        /// Number of occurrences of this item. Null means unlimited.
        /// </summary>
        [XmlElement("occurrences")]
        public int? Occurrences
        {
            get { return _occurrences; }
            set
            {
                // Validate
                if (!(value == 1 && Recurrence == ScheduleItemRecurrence.None) &&
                    !((value == null || value > 1) && Recurrence != ScheduleItemRecurrence.None))
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Set value
                _occurrences = value;
            }
        }

        private int? _occurrences;

        /// <summary>
        /// Omits the <see cref="Occurrences"/> property from XML serialization when it is empty.
        /// </summary>
        [XmlIgnore]
        public bool OccurrencesSpecified { get { return Occurrences.HasValue && Occurrences.Value > 0; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a string describing the schedule item,
        /// using the <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a string describing this schedule item,
        /// using the specified <see cref="CultureInfo"/>.
        /// </summary>
        public string ToString(CultureInfo culture)
        {
            // Get offset text
            string offset = null;
            switch (Offset)
            {
                case ScheduleItemOffset.First: offset = Properties.Resources.ScheduleItemOffsetFirst; break;
                case ScheduleItemOffset.Second: offset = Properties.Resources.ScheduleItemOffsetSecond; break;
                case ScheduleItemOffset.Third: offset = Properties.Resources.ScheduleItemOffsetThird; break;
                case ScheduleItemOffset.Last: offset = Properties.Resources.ScheduleItemOffsetLast; break;
            }

            // Convert to string according to recurrence pattern, occurrences and offset (when relevant)
            switch (Recurrence)
            {
                case ScheduleItemRecurrence.None:
                    return string.Format(culture,
                        Properties.Resources.ScheduleItemToStringOnce, Start, Duration);

                case ScheduleItemRecurrence.Hourly:
                    return string.Format(culture, Occurrences.HasValue ?
                        Properties.Resources.ScheduleItemToStringHourlyFixedEnd :
                        Properties.Resources.ScheduleItemToStringHourlyNoEnd,
                        Start, End, Duration, Interval, Occurrences);

                case ScheduleItemRecurrence.Daily:
                    return string.Format(culture, Occurrences.HasValue ?
                        Properties.Resources.ScheduleItemToStringDailyFixedEnd :
                        Properties.Resources.ScheduleItemToStringDailyNoEnd,
                        Start, End, Duration, Interval, Occurrences);

                case ScheduleItemRecurrence.Weekly:
                    return string.Format(culture, Occurrences.HasValue ?
                        Properties.Resources.ScheduleItemToStringWeeklyFixedEnd :
                        Properties.Resources.ScheduleItemToStringWeeklyNoEnd,
                        Start, End, Duration,
                        string.Join(Properties.Resources.ScheduleItemToStringDayOfWeekSeparator, Days.ToStringArray()),
                        Interval, Occurrences);

                case ScheduleItemRecurrence.Monthly:
                    if (Offset != ScheduleItemOffset.None)
                    {
                        // Month with offset
                        return string.Format(culture, Occurrences.HasValue ?
                            Properties.Resources.ScheduleItemToStringMonthlyOffsetRecurring :
                            Properties.Resources.ScheduleItemToStringMonthlyOffsetNoEnd,
                            Start, End, Duration, offset, Interval, Occurrences);
                    }
                    // Month without offset
                    return string.Format(culture, Occurrences.HasValue
                                                      ? Properties.Resources.ScheduleItemToStringMonthlyDayFixedEnd
                                                      : Properties.Resources.ScheduleItemToStringMonthlyDayNoEnd,
                                         Start, End, Duration, Interval, Occurrences);

                case ScheduleItemRecurrence.Yearly:
                    if (Offset != ScheduleItemOffset.None)
                    {
                        // Yearly with offset
                        return string.Format(culture, Occurrences.HasValue ?
                            Properties.Resources.ScheduleItemToStringYearlyOffsetFixedEnd :
                            Properties.Resources.ScheduleItemToStringYearlyOffsetNoEnd,
                            Start, End, Duration, offset, Interval, Occurrences);
                    }
                    // Yearly without offset
                    return string.Format(culture, Occurrences.HasValue
                                                      ? Properties.Resources.ScheduleItemToStringYearlyDayFixedEnd
                                                      : Properties.Resources.ScheduleItemToStringYearlyDayNoEnd,
                                         Start, End, Duration, Interval, Occurrences);

                default:
                    // All cases should be covered
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the next occurrence after the specified date.
        /// </summary>
        /// <param name="date">Date to check.</param>
        /// <returns>Next scheduled date in UTC or null when the schedule has ended.</returns>
        public DateTimeOffset? GetNext(DateTimeOffset date)
        {
            // Return start when date is before
            if (date < Start) return Start;

            // Return null when no recurrence
            if (Recurrence == ScheduleItemRecurrence.None)
                return null;

            // Calculate next event
            var occurrence = 1;
            var eventStart = Start;
            var eventEnd = eventStart.AddMinutes(Duration);
            var dateEnd = date.AddMinutes(Duration);
            do
            {
                switch (Recurrence)
                {
                    case ScheduleItemRecurrence.Hourly:
                        {
                            // Find next hourly occurrence
                            while (eventStart < eventEnd)
                                eventStart = eventStart.AddHours(Interval);
                            break;
                        }

                    case ScheduleItemRecurrence.Daily:
                        {
                            // Find next daily occurrence
                            while (eventStart < eventEnd)
                                eventStart = eventStart.AddDays(Interval);
                            break;
                        }

                    case ScheduleItemRecurrence.Weekly:
                        {
                            // Find next weekly occurrence
                            while (eventStart < eventEnd)
                                eventStart = eventStart.GetNext(Days);
                            break;
                        }

                    case ScheduleItemRecurrence.Monthly:
                        {
                            // Find next monthly occurrence
                            while (eventStart < eventEnd)
                            {
                                if (Offset == ScheduleItemOffset.None)
                                {
                                    // Next month on same day, skipping months with less days
                                    DateTimeOffset nextMonth = eventStart.AddMonths(Interval);
                                    int skip = 1;
                                    while (nextMonth.Day != eventStart.Day)
                                        nextMonth = eventStart.AddMonths(Interval + skip++);
                                    eventStart = nextMonth;
                                }
                                else
                                {
                                    // Get next month with weekday offset
                                    DayOfWeek weekday = Days.ToArray()[0];
                                    switch (Offset)
                                    {
                                        case ScheduleItemOffset.First:
                                            eventStart = eventStart.AddMonths(Interval).GetFirst(weekday);
                                            break;

                                        case ScheduleItemOffset.Second:
                                            eventStart = eventStart.AddMonths(Interval).GetFirst(weekday).GetNext(weekday);
                                            break;

                                        case ScheduleItemOffset.Third:
                                            eventStart = eventStart.AddMonths(Interval).GetFirst(weekday).GetNext(weekday).GetNext(weekday);
                                            break;

                                        case ScheduleItemOffset.Last:
                                            eventStart = eventStart.AddMonths(Interval).GetLast(weekday);
                                            break;
                                    }
                                }
                            }
                            break;
                        }

                    case ScheduleItemRecurrence.Yearly:
                        {
                            // Find next yearly occurrence
                            while (eventStart < eventEnd)
                            {
                                if (Offset == ScheduleItemOffset.None)
                                {
                                    // Next year on same day, skipping years with less days in the same month
                                    DateTimeOffset nextYear = eventStart.AddYears(Interval);
                                    int skip = 1;
                                    while (nextYear.Day != eventStart.Day)
                                        nextYear = eventStart.AddYears(Interval + skip++);
                                    eventStart = nextYear;
                                }
                                else
                                {
                                    // Get next year with weekday offset
                                    DayOfWeek weekday = Days.ToArray()[0];
                                    switch (Offset)
                                    {
                                        case ScheduleItemOffset.First:
                                            eventStart = eventStart.AddYears(Interval).GetFirst(weekday);
                                            break;

                                        case ScheduleItemOffset.Second:
                                            eventStart = eventStart.AddYears(Interval).GetFirst(weekday).GetNext(weekday);
                                            break;

                                        case ScheduleItemOffset.Third:
                                            eventStart = eventStart.AddYears(Interval).GetFirst(weekday).GetNext(weekday).GetNext(weekday);
                                            break;

                                        case ScheduleItemOffset.Last:
                                            eventStart = eventStart.AddYears(Interval).GetLast(weekday);
                                            break;
                                    }
                                }
                            }
                            break;
                        }

                    default:
                        // Assert all future cases are covered
                        throw new InvalidOperationException();
                }
                eventEnd = eventStart.AddMinutes(Duration);

                // Limit occurrences (when specified)
                if (Occurrences.HasValue && ++occurrence >= Occurrences)
                    break;
            }
            while (eventEnd < dateEnd);

            // Return result in UTC
            return (eventStart > date ? eventStart.ToUniversalTime() : (DateTimeOffset?)null);
        }

        /// <summary>
        /// Checks if the specified time span lies within this schedule.
        /// </summary>
        /// <param name="utcDate">Date to check in UTC, usually <see cref="DateTime.UtcNow"/>.</param>
        /// <param name="duration">Duration in minutes.</param>
        public bool Contains(DateTimeOffset utcDate, int duration)
        {
            // Get the latest date of an occurrence before the test date
            var latestStart = utcDate.AddMinutes(-(Duration - duration + 1));
            var closestStart = GetNext(latestStart);
            if (!closestStart.HasValue)
                return false;
            var closestEnd = closestStart.Value.AddMinutes(Duration);

            // Check if test date lies within occurrence
            var dateEnd = utcDate.AddMinutes(duration);
            return utcDate <= closestEnd && dateEnd >= closestStart;
        }

        #endregion Public Methods
    }
}