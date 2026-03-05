using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace CodeForDotNet.Xml.Extensions;

/// <summary>
/// Implements some of the XPath 2.0 functions, which can be added to an <see cref="XPathExpression"/> or <see cref="XslCompiledTransform"/> using the
/// <see cref="XsltArgumentList.AddExtensionObject"/> method and including the "http://www.w3.org/2005/xpath-functions" namespace.
/// </summary>
/// <remarks>
/// See http://www.w3.org/TR/xpath-functions for the specification. Functions must be lowercase (against typical coding standards) to conform to the XPath
/// 2.0 specification, i.e. extension functions are matched using case sensitivity.
/// </remarks>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Lower-case methods required to match XPath function specification.")]
[SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Non-static member required by XPath function library support.")]
public sealed class XPathFunctions
{
    /// <summary>
    /// W3C namespace for XPath 2.0 functions (which we implement here).
    /// </summary>
    public const string XmlNamespace = "http://www.w3.org/2005/xpath-functions";

    /// <summary>
    /// Returns true if the input string matches the regular expression pattern.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <param name="pattern">Regular expression pattern to match.</param>
    /// <returns>True when matched.</returns>
    /// <see href="https://www.w3.org/TR/xpath-functions-31/#func-matches" />
    public bool matches(string input, string pattern)
    {
        // Call overloaded method.
        return matches(input, pattern, string.Empty);
    }

    /// <summary>
    /// Returns true if the input string matches the regular expression pattern.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <param name="pattern">Regular expression pattern to match.</param>
    /// <param name="flags">
    /// Regular expression flag characters, one or more of:
    /// <list type="bullet">
    /// <item>s - Single-line mode, also known as "dotall" mode, where ".*" matches across lines.
    /// Corresponds to <see cref="RegexOptions.Singleline"/>.</item>
    /// <item>m - Multi-line mode, where "^" start and "$" end anchors represent each line, not the default whole string.
    /// Corresponds to <see cref="RegexOptions.Multiline"/>.</item>
    /// <item>i - Case insensitive match.
    /// Corresponds to <see cref="RegexOptions.IgnoreCase"/>.</item>
    /// <item>x - Ignore whitespace in pattern, where spaces or new lines are used in long patterns to make it readable.
    /// Corresponds to <see cref="RegexOptions.IgnorePatternWhitespace"/>.</item>
    /// </list>
    /// </param>
    /// <returns>True when matched.</returns>
    /// <remarks>
    /// The <paramref name="flags"/> parameter cannot have a default value to replace the <see cref="matches(string, string)"/> method because the XSLT extensions
    /// will not find it (does not support optional parameters).
    /// </remarks>
    /// <see href="https://www.w3.org/TR/xpath-functions-31/#func-matches" />
    public bool matches(string input, string pattern, string flags)
    {
        // Default parameters to empty strings (W3C behavior).
        // Do not add whitespace trimming to the input because that functionality is not in the W3C standard for this function!
        // For example, if you get extra whitespace when an "xsl:strip-space" pre-processing instruction should apply,
        // then you most likely did not actually read the document during transformation.
        // This occurs when using an XPath or XML document instance as input, which has already been read
        // so cannot be pre-processed.
        var resolvedInput = input ?? string.Empty;
        var resolvedPattern = pattern ?? string.Empty;

        // Convert XPath flags to .NET regular expression options.
        var options = RegexOptions.None;
        if (flags is not null)
        {
            if (flags.Contains('s', StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.Singleline;
            if (flags.Contains('m', StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.Multiline;
            if (flags.Contains('i', StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.IgnoreCase;
            if (flags.Contains('x', StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.IgnorePatternWhitespace;
        }

        // Execute regular expression then return result.
        return Regex.IsMatch(resolvedInput, resolvedPattern, options);
    }
}
