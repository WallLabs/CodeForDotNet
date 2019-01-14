using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using CodeChief.Collections;
using CodeChief.Properties;

namespace CodeChief.Data
{
    /// <summary>
    /// Collection of <see cref="ScheduleItem"/>.
    /// </summary>
    [XmlRoot(XmlRootName, Namespace = XmlNamespace)]
    [XmlType(XmlTypeName, Namespace = XmlNamespace)]
    public class ScheduleItemCollection : Collection<ScheduleItem>
    {
        #region Constants

        /// <summary>
        /// XML root element name.
        /// </summary>
        public const string XmlRootName = "scheduleItems";

        /// <summary>
        /// XML type name.
        /// </summary>
        public const string XmlTypeName = "scheduleItemsType";

        /// <summary>
        /// XML namespace.
        /// </summary>
        public const string XmlNamespace = ComponentsConstants.XmlRootNamespace;

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(ScheduleItemCollection scheduleItem1, ScheduleItemCollection scheduleItem2)
        {
            if (!ReferenceEquals(scheduleItem1, null))
                return scheduleItem1.Equals(scheduleItem2);

            return ReferenceEquals(scheduleItem2, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(ScheduleItemCollection scheduleItem1, ScheduleItemCollection scheduleItem2)
        {
            if (!ReferenceEquals(scheduleItem1, null))
                return !scheduleItem1.Equals(scheduleItem2);

            return !ReferenceEquals(scheduleItem2, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Compare nullability and type
            var other = obj as ScheduleItemCollection;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return ArrayExtensions.AreEqual(this, other);
        }

        /// <summary>
        /// Returns a hashcode based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return this.GetHashCodeOfItems();
        }

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        public ScheduleItemCollection() { }

        /// <summary>
        /// Creates an instance based on an existing list.
        /// </summary>
        public ScheduleItemCollection(IList<ScheduleItem> list) : base(list) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string describing the schedule items,
        /// using the <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a string describing this schedule items,
        /// using the specified <see cref="CultureInfo"/>.
        /// </summary>
        public string ToString(CultureInfo culture)
        {
            if (Count > 0)
            {
                // Build description string
                return String.Format(culture,
                                     Resources.ScheduleItemCollectionToStringFormat, Count,
                                     String.Join(Resources.ScheduleItemCollectionToStringSeparator,
                                                 (from item in Items select item.ToString(culture)).ToArray()));
            }

            // No text when empty: should be excluded from any descriptions.
            return null;
        }

        #endregion
    }
}