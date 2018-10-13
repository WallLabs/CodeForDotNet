using System;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace CodeForDotNet.Xml
{
    /// <summary>
    /// Implements some of the XPath 2.0 functions, which can be added to an <see cref="XPathExpression"/> or <see cref="XslCompiledTransform"/>
    /// using the <see cref="XsltArgumentList.AddExtensionObject"/> method and including the
    /// "http://www.w3.org/2005/xpath-functions" namespace.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/xpath-functions for the specification.
    /// Functions must be lowercase (against typical coding standards) to conform to the XPath 2.0 specification,
    /// i.e. extension functions are matched using case sensitivity.
    /// </remarks>
    public sealed class W3cXPathFunctions
    {
        #region Constants

        /// <summary>
        /// W3C namespace for XPath 2.0 functions (which we implement here).
        /// </summary>
        public const string XmlNamespace = "http://www.w3.org/2005/xpath-functions";

        #endregion

        #region String Functions

        #region Pattern Matches

        /// <summary>
        /// Returns true if the input string matches the regular expression pattern.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="pattern">Regular expression pattern to match.</param>
        /// <returns>True when matched.</returns>
        public bool matches(string input, string pattern)
        {
            // Call overloaded method
            return matches(input, pattern, string.Empty);
        }

        /// <summary>
        /// Returns true if the input string matches the regular expression pattern.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="pattern">Regular expression pattern to match.</param>
        /// <param name="flags">
        /// Regular expression flag characters, one or more of:
        ///     s - Single-line mode. Corresponds to <see cref="RegexOptions.Singleline"/>.
        ///     m - Multi-line mode. Corresponds to <see cref="RegexOptions.Multiline"/>.
        ///     i - Case insensitive match. Corresponds to <see cref="RegexOptions.IgnoreCase"/>.
        ///     x - Ignore whitespace in pattern, e.g. when spaces or new lines are used in long patterns to make it readable.
        ///         Corresponds to <see cref="RegexOptions.IgnorePatternWhitespace"/>.
        /// </param>
        /// <returns>True when matched.</returns>
        /// <remarks>
        /// The <paramref name="flags"/> parameter cannot have a default to replace the <see cref="matches(string, string)"/>
        /// method because the XSLT extensions will not find it (does not support optional parameters).
        /// </remarks>
        public bool matches(string input, string pattern, string flags)
        {
            // Validate
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            // Normalize space of the input string so that line beginnings and ends are valid according to regular expression syntax
            input = input.Trim();

            // Convert XPath flags to .NET regular expression options
            var options = RegexOptions.None;
            if (flags != null)
            {
                if (flags.Contains("s")) options |= RegexOptions.Singleline;
                if (flags.Contains("m")) options |= RegexOptions.Multiline;
                if (flags.Contains("i")) options |= RegexOptions.IgnoreCase;
                if (flags.Contains("x")) options |= RegexOptions.IgnorePatternWhitespace;
            }

            // Execute regular expression then return result
            return Regex.Match(input.Trim(), pattern, options).Success;
        }

        #endregion

        #endregion
    }
}
