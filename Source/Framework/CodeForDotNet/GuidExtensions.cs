using System;
using System.Text.RegularExpressions;

namespace CodeForDotNet;

/// <summary>
/// Contains extensions for the <see cref="Guid"/> type.
/// </summary>
public static partial class GuidExtensions
{
    /// <summary>
    /// Regular expression which matches a valid <see cref="System.Guid"/> in default ("D"/registry) format.
    /// </summary>
    public const string GuidFormat = GuidDFormat;

    /// <summary>
    /// Regular expression which matches a valid <see cref="System.Guid"/> in "B" format.
    /// </summary>
    public const string GuidBFormat = @"^\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}$";

    /// <summary>
    /// Compiled <see cref="GuidBFormat"/> regular expression.
    /// </summary>
    [GeneratedRegex(GuidBFormat)]
    private static partial Regex GuidBFormatRegEx();

    /// <summary>
    /// Regular expression which matches a valid <see cref="System.Guid"/> in "D" format.
    /// </summary>
    /// <remarks>
    /// Also known as the "registry" format, as it is commonly used in Windows.
    /// This is the default format returned by <see cref="System.Guid.ToString()"/>.
    /// </remarks>
    public const string GuidDFormat = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";

    /// <summary>
    /// Compiled <see cref="GuidDFormat"/> regular expression.
    /// </summary>
    [GeneratedRegex(GuidDFormat)]
    private static partial Regex GuidDFormatRegEx();

    /// <summary>
    /// Regular expression which matches a valid <see cref="System.Guid"/> in "N" format.
    /// </summary>
    public const string GuidNFormat = @"^[0-9a-fA-F]{32}$";

    /// <summary>
    /// Compiled <see cref="GuidNFormat"/> regular expression.
    /// </summary>
    [GeneratedRegex(GuidNFormat)]
    private static partial Regex GuidNFormatRegEx();

    /// <summary>
    /// Regular expression which matches a valid <see cref="System.Guid"/> in "P" format.
    /// </summary>
    public const string GuidPFormat = @"^\([0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\)$";

    /// <summary>
    /// Compiled <see cref="GuidPFormat"/> regular expression.
    /// </summary>
    [GeneratedRegex(GuidPFormat)]
    private static partial Regex GuidPFormatRegEx();

    /// <summary>
    /// Parses a GUID string to a nullable value.
    /// </summary>
    public static bool TryParse(string value, out Guid output)
    {
        // Prevalidate content (avoid exception)
        if (string.IsNullOrEmpty(value) || (
            !GuidNFormatRegEx().IsMatch(value) &&
            !GuidDFormatRegEx().IsMatch(value) &&
            !GuidPFormatRegEx().IsMatch(value) &&
            !GuidBFormatRegEx().IsMatch(value)))
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
}
