using System;
using System.Xml.Serialization;

namespace CodeForDotNet
{
    /// <summary>
    /// Enumeration representing days of the week as bit flags, starting with Monday at the LSB.
    /// </summary>
    [XmlType(nameof(DaysOfWeek) + "Type", Namespace = Constants.XmlRootNamespace)]
    [Flags]
    public enum DaysOfWeek
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0x0000000,

        /// <summary>
        /// Sunday.
        /// </summary>
        Sunday = 0x00000001,

        /// <summary>
        /// Monday.
        /// </summary>
        Monday = 0x00000002,

        /// <summary>
        /// Tuesday.
        /// </summary>
        Tuesday = 0x00000004,

        /// <summary>
        /// Wednesday.
        /// </summary>
        Wednesday = 0x00000008,

        /// <summary>
        /// Thursday.
        /// </summary>
        Thursday = 0x00000010,

        /// <summary>
        /// Friday.
        /// </summary>
        Friday = 0x00000020,

        /// <summary>
        /// Saturday.
        /// </summary>
        Saturday = 0x00000040,

        /// <summary>
        /// Everyday.
        /// </summary>
        Everyday = 0x0000007f,

        /// <summary>
        /// Weekdays (Monday to Friday).
        /// </summary>
        Weekdays = 0x0000003e,

        /// <summary>
        /// Weekend (Saturday and Sunday).
        /// </summary>
        Weekend = 0x00000041
    }
}
