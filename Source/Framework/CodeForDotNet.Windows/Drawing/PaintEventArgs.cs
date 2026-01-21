using System;
using System.Drawing;

namespace CodeForDotNet.Windows.Drawing;

/// <summary>
/// UI graphics paint event arguments.
/// </summary>
/// <remarks>
/// Portable version of the "System.Windows.Forms.PaintEventArgs" class. To be replaced when
/// available in the .NET Standard framework.
/// </remarks>
public class PaintEventArgs(Graphics graphics, Rectangle region) : EventArgs
{
    /// <summary>
    /// Graphics interface to be used for drawing.
    /// </summary>
    public Graphics Graphics { get; set; } = graphics;

    /// <summary>
    /// Region which has been invalidated and should be re-drawn.
    /// </summary>
    public Rectangle Region { get; set; } = region;
}
