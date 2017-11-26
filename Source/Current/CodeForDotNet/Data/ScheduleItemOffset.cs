using System.Xml.Serialization;

namespace CodeForDotNet.Data
{
    /// <summary>
    /// Specifies an offset to make a schedule item relative to the start or end of a recurrence instance.
    /// </summary>
    [XmlType(nameof(ScheduleItemOffset) + "Type", Namespace = Constants.XmlRootNamespace)]
    public enum ScheduleItemOffset
    {
        /// <summary>
        /// No offset, e.g. a specific day of a month, a specific day and month of a year.
        /// </summary>
        None,

        /// <summary>
        /// First instance, e.g. first day of a month, first Wednesday of a month.
        /// </summary>
        First,

        /// <summary>
        /// Second instance, e.g. second Monday in a month.
        /// </summary>
        Second,

        /// <summary>
        /// Third instance, e.g. third Tuesday in a month.
        /// </summary>
        Third,

        /// <summary>
        /// Last instance, e.g. last Wednesday of a month.
        /// </summary>
        Last
    }
}
