using System.Drawing;
using System.Globalization;

namespace CodeForDotNet.Drawing;

/// <summary>
/// Extensions for working with the <see cref="Color"/> class.
/// </summary>
public static class ColorExtensions
{
    #region Public Methods

    /// <summary>
    /// Converts the color to a hexadecimal ARGB string, e.g. #AARRGGBB.
    /// </summary>
    public static string ToArgbString(this Color value)
    {
        return "#" +
            value.A.ToString("X2", CultureInfo.InvariantCulture) +
            value.R.ToString("X2", CultureInfo.InvariantCulture) +
            value.G.ToString("X2", CultureInfo.InvariantCulture) +
            value.B.ToString("X2", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the color to a hexadecimal RGB string, e.g. #RRGGBB.
    /// </summary>
    public static string ToRgbString(this Color value)
    {
        return "#" +
            value.R.ToString("X2", CultureInfo.InvariantCulture) +
            value.G.ToString("X2", CultureInfo.InvariantCulture) +
            value.B.ToString("X2", CultureInfo.InvariantCulture);
    }

    #endregion Public Methods
}
