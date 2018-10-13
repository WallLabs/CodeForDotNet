using System;
using System.Text.RegularExpressions;

namespace CodeForDotNet
{
    /// <summary>
    /// Contains extensions for the <see cref="Guid"/> type.
    /// </summary>
    public static class GuidExtensions
    {
        #region Constants

        /// <summary>
        /// Regular expression which matches a valid <see cref="System.Guid"/> in "N" format.
        /// </summary>
        public const string GuidNFormat = @"^[0-9a-fA-F]{32}$";

        /// <summary>
        /// Regular expression which matches a valid <see cref="System.Guid"/> in "D" format.
        /// </summary>
        public const string GuidDFormat = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";

        /// <summary>
        /// Regular expression which matches a valid <see cref="System.Guid"/> in "P" format.
        /// </summary>
        public const string GuidPFormat = @"^\([0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\)$";

        /// <summary>
        /// Regular expression which matches a valid <see cref="System.Guid"/> in "B" format.
        /// </summary>
        public const string GuidBFormat = @"^\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}$";

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a GUID string to a nullable value.
        /// </summary>
        public static bool TryParse(string value, out Guid output)
        {
            // Prevalidate content (avoid exception)
            if (string.IsNullOrEmpty(value) || (
                !Regex.IsMatch(value, GuidNFormat) &&
                !Regex.IsMatch(value, GuidDFormat) &&
                !Regex.IsMatch(value, GuidPFormat) &&
                !Regex.IsMatch(value, GuidBFormat)))
            {
                // Return failure
                output = Guid.Empty;
                return false;
            }

            // Parse (should succeed)
            output = new Guid(value);

            // Return success
            return true;
        }

        #endregion
    }
}
