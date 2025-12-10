using System.Xml.Serialization;

namespace CodeForDotNet.Data;

/// <summary>
/// Pattern of recurrence for a <see cref="ScheduleItem"/>, quantifying its <see cref="ScheduleItem.Interval"/>, e.g. an interval of 3 with recurrence of
/// <see cref="Daily"/> means "every 3 days", whereas the same interval of 3 with recurrence of <see cref="Monthly"/> means "every 3 months".
/// </summary>
[XmlType(nameof(ScheduleItemRecurrence) + "Type", Namespace = Constants.XmlRootNamespace)]
public enum ScheduleItemRecurrence
{
    /// <summary>
    /// Once, i.e. no recurrence.
    /// </summary>
    None,

    /// <summary>
    /// Every # hours (depends on interval).
    /// </summary>
    Hourly,

    /// <summary>
    /// Every # days (depends on interval).
    /// </summary>
    Daily,

    /// <summary>
    /// Every # weeks (depends on interval).
    /// </summary>
    Weekly,

    /// <summary>
    /// Every # months (depends on interval).
    /// </summary>
    Monthly,

    /// <summary>
    /// Every # years (depends on interval).
    /// </summary>
    Yearly
}
