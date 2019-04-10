using System;
using System.Collections.Generic;
using System.Text;

namespace CodeForDotNet
{
    /// <summary>
    /// Extensions for working with the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        #region Constants

        /// <summary>
        /// Minimum ASCII character value which is printable (not a special character).
        /// </summary>
        public const char AsciiMinimumPrintable = '\u0020';

        /// <summary>
        /// Minimum ASCII character value which is printable (not a special character).
        /// </summary>
        public const char AsciiMaximumPrintable = '\u007f';

        /// <summary>
        /// Ellipsis used to end a string which has been truncated.
        /// </summary>
        private const string Truncatellipsis = "...";

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Returns an empty string ("") when the string is null.
        /// </summary>
        public static string EmptyWhenNull(string value)
        {
            return value ?? "";
        }

        /// <summary>
        /// Returns null when the string is empty.
        /// </summary>
        public static string NullWhenEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Split a string with support for an escape sequence (when not to split).
        /// </summary>
        /// <param name="text">The string to split.</param>
        /// <param name="delimiter">Delimiter</param>
        /// <param name="escape">Escape character</param>
        /// <param name="removeEscape">Remove escape characters</param>
        /// <param name="removeEmptyEntries">Remove empty entries?</param>
        public static string[] SplitEscaped(this string text, char delimiter, char escape, bool removeEscape, bool removeEmptyEntries)
        {
            // Validate
            if (text == null) throw new ArgumentNullException(nameof(text));

            // Split...
            var results = new List<string>();
            var item = new StringBuilder();
            var escaped = false;
            foreach (var character in text)
            {
                // Was previous character an escape character
                if (escaped)
                {
                    // If we should not remove the Escape character: Add it.
                    if (!removeEscape)
                        item.Append(escape);

                    // Add the escaped character
                    item.Append(character);

                    // Reset flag
                    escaped = false;
                }
                else
                {
                    // Check if we got an delimiter
                    if (character == delimiter)
                    {
                        if (removeEmptyEntries && item.Length == 0)
                            continue;

                        // Add item to result
                        results.Add(item.ToString());

                        // reset item;
                        item.Length = 0;
                    }
                    else if (character == escape)
                    {
                        // Set flag
                        escaped = true;
                    }
                    else
                    {
                        // Add character to item
                        item.Append(character);
                    }
                }
            }

            // Add the last item if it is there or we should not remove entries
            if (item.Length > 0 || !removeEmptyEntries)
                results.Add(item.ToString());

            return results.ToArray();
        }

        /// <summary>
        /// Adds spaces to a string each time a capital letter is encountered,
        /// e.g. converts camel or pascal case names to spaced words.
        /// </summary>
        public static string Space(this string value)
        {
            // Validate
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Space string...
            var buffer = new StringBuilder();
            var lastLow = false;
            var lastChar = ' ';
            foreach (var currentChar in value)
            {
                // Add space if case changed to uppercase and not a whitespace or whitespace before
                if (!char.IsWhiteSpace(lastChar) && !char.IsWhiteSpace(currentChar) && char.IsUpper(currentChar) && lastLow)
                    buffer.Append(' ');

                // Add current char
                buffer.Append(currentChar);

                // Next char...
                lastChar = currentChar;
                lastLow = char.IsLower(currentChar);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Terminates a string with an ellipsis ending "..." if it exceeds a specific length.
        /// Useful for building summary list items when the length could be much longer.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="length">
        /// Maximum length permitted without change, including the ending. When longer the string is truncated
        /// to this length minus the length of the ending, which is then appended.
        /// </param>
        /// <returns>String which is truncated as necessary.</returns>
        public static string Truncate(this string value, int length)
        {
            return Truncate(value, length, Truncatellipsis);
        }

        /// <summary>
        /// Terminates a string with an ending (e.g. <see cref="Truncatellipsis"/> "...") if it exceeds a specific length.
        /// Useful for building summary list items when the length could be much longer.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="length">
        /// Maximum length permitted without change, including the ending. When longer the string is truncated
        /// to this length minus the length of the ending, which is then appended.
        /// </param>
        /// <param name="ending">Ending to append when the string exceeds the length. Can be null or empty.</param>
        /// <returns>String which is truncated as necessary.</returns>
        public static string Truncate(this string value, int length, string ending)
        {
            // Validate
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Return original string when shorter
            var originalLength = value.Length;
            if (originalLength <= length)
                return value;

            // Do not use ending when length is shorter
            var endingLength = !string.IsNullOrEmpty(ending) ? ending.Length : 0;
            var useEnding = (endingLength > 0) && (endingLength < length);

            // Truncate string when longer, leaving room for ending
            var result = new StringBuilder(length);
            result.Append(value.Substring(0, useEnding ? length - endingLength : length));

            // Add ending (if required)
            if (useEnding)
                result.Append(ending);

            // Return truncated string
            return result.ToString();
        }

        /// <summary>
        /// Searches a string array for the first index starting with the specified value,
        /// using a <see cref="StringComparison.OrdinalIgnoreCase"/> comparison mode.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to search for.</param>
        /// <returns>Index of the first item starting with the value, otherwise -1 when not found.</returns>
        public static int IndexStarting(this IEnumerable<string> array, string value)
        {
            return IndexStarting(array, value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Searches a string array for the first index starting with the specified value.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparison">Comparison mode.</param>
        /// <returns>Index of the first item starting with the value, otherwise -1 when not found.</returns>
        public static int IndexStarting(this IEnumerable<string> array, string value, StringComparison comparison)
        {
            // Validate
            if (array == null) throw new ArgumentNullException(nameof(array));

            // Search string
            var index = 0;
            foreach (var item in array)
            {
                if (item.StartsWith(value, comparison))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Trims all lines of a string, removing tabs or white space that may have been added when reading from a configuration file.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <returns>String with no white space at the beginning or end of every line.</returns>
        public static string TrimLines(this string value)
        {
            // Validate
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Trim...
            var result = new StringBuilder();
            foreach (var line in value.Trim().Split('\n'))
                result.AppendLine(line.Trim().Trim('\r'));
            return result.ToString().Trim();
        }

        /// <summary>
        /// Filters a string down to printable characters.
        /// </summary>
        /// <param name="value">Raw string to filter.</param>
        /// <param name="placeholder">Optional place-holder to replace filtered characters with.</param>
        /// <returns>Filtered string.</returns>
        public static string FilterSpecial(this string value, char? placeholder = '?')
        {
            // Validate
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Filter...
            var result = new StringBuilder();
            foreach (var rawChar in value)
            {
                var valid = rawChar >= AsciiMinimumPrintable && rawChar <= AsciiMaximumPrintable;
                if (valid)
                    result.Append(rawChar);
                else if (placeholder.HasValue)
                    result.Append(placeholder.Value);
            }
            return result.ToString();
        }

        #endregion Public Methods
    }
}