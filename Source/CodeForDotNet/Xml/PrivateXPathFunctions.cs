using System;
using System.Globalization;

namespace CodeForDotNet.Xml
{
    /// <summary>
    /// Provides private (non-standard) XPath functions which are not available in XPath 1.0 or 2.0.
    /// </summary>
    /// <remarks>
    /// Use of private functions should be avoided to ensure all data remains standard and portable.
    /// Functions must be lowercase (against typical coding standards) to conform to XML naming standards,
    /// i.e. extension functions are matched using case sensitivity.
    /// </remarks>
    public sealed class PrivateXPathFunctions
    {
        #region Constants

        /// <summary>
        /// XPath function namespace.
        /// </summary>
        public const string XmlNamespace = "urn:PrivateXPathFunctions";

        #endregion

        #region DateTime Functions

        /// <summary>
        /// Converts a UTC date and time to local time.
        /// </summary>
        /// <param name="value">Date in UTC.</param>
        /// <returns>Date in local time.</returns>
        public DateTime ToLocalTime(DateTime value)
        {
            return value.ToLocalTime();
        }

        /// <summary>
        /// Converts a date and time to UTC.
        /// </summary>
        /// <param name="value">Date in local time.</param>
        /// <returns>Date in UTC.</returns>
        public DateTime ToUniversalTime(DateTime value)
        {
            return value.ToUniversalTime();
        }

        /// <summary>
        /// Converts a date and time to a short format string.
        /// </summary>
        /// <param name="value">Date to format.</param>
        /// <returns>Short format date and time string.</returns>
        public string ToDateTimeShortString(DateTime value)
        {
            return value.ToString("g", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a date and time to a long format string.
        /// </summary>
        /// <param name="value">Date to format.</param>
        /// <returns>Long format date and time string.</returns>
        public string ToDateTimeLongString(DateTime value)
        {
            return value.ToString("f", CultureInfo.CurrentCulture);
        }

        #endregion
    }
}
