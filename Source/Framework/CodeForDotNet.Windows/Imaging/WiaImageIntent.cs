namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="WiaImageIntent"/>.
/// </summary>
public enum WiaImageIntent
{
    /// <summary>
    /// Unspecified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Color.
    /// </summary>
    Color = 1,

    /// <summary>
    /// Grayscale.
    /// </summary>
    Grayscale = 2,

    /// <summary>
    /// Text.
    /// </summary>
    Text = 4
}
