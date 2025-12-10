namespace CodeForDotNet.Drawing;

/// <summary>
/// String alignment disconnected from any API dependencies, useful for future proof serialization and cross-platform.
/// </summary>
/// <remarks>Explicitly cast to "System.Drawing.StringAlignment" via integer.</remarks>
public enum StringHorizontalAlignment
{
    /// <summary>
    /// Near to the start of the line (according to writing direction).
    /// </summary>
    Near = 0,

    /// <summary>
    /// Centered.
    /// </summary>
    Center = 1,

    /// <summary>
    /// Far from the start of the line (according to writing direction).
    /// </summary>
    Far = 2
}
