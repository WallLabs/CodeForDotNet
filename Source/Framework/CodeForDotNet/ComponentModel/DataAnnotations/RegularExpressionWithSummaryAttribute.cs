using System;
using System.ComponentModel.DataAnnotations;

namespace CodeForDotNet.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value in ASP.NET Dynamic Data must match the specified regular
/// expression. Extended to provide a human readable summary of the pattern in the error
/// message, instead of the pattern itself.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class RegularExpressionWithSummaryAttribute : RegularExpressionAttribute
{
    /// <summary>
    /// Initializes a new instance with required values.
    /// </summary>
    /// <param name="pattern">
    /// The regular expression that is used to validate the data field value.
    /// </param>
    /// <param name="patternSummary">
    /// Summary of the regular expression pattern for the user, displayed in the error message
    /// instead of the pattern.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
    public RegularExpressionWithSummaryAttribute(string pattern, string patternSummary)
        : base(pattern)
    {
        PatternSummary = patternSummary;
    }

    /// <summary>
    /// Summary of the <see cref="RegularExpressionAttribute.Pattern"/> in a simple form,
    /// displayed to the user in the error message instead of the (complex) pattern itself.
    /// </summary>
    /// <remarks>
    /// Avoid using localizable words, i.e. just list the valid content, e.g. "a-z A-Z 0-9 - _"
    /// instead of "^[a-zA-Z0-9\-_]$".
    /// </remarks>
    public string PatternSummary { get; set; }

    /// <summary>
    /// Formats the error message to display if the regular expression validation fails.
    /// </summary>
    /// <param name="name">The name of the field that caused the validation failure.</param>
    /// <returns>The formatted error message.</returns>
    /// <remarks>
    /// Overridden to replace the pattern with the <see cref="PatternSummary"/> text making the
    /// error easier to understand by the user.
    /// </remarks>
    public override string FormatErrorMessage(string name)
    {
        return base.FormatErrorMessage(name).Replace(Pattern, PatternSummary);
    }
}
