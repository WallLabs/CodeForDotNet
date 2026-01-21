using System;

namespace CodeForDotNet.UI.Input;

/// <summary>
/// Specifies constants that define which mouse button was pressed.
/// </summary>
/// <remarks>
/// Portable version of the "System.Windows.Forms.MouseButtons" class. To be replaced when
/// available in the .NET Standard framework.
/// </remarks>
[Flags]
public enum MouseButtons
{
    /// <summary>
    /// No mouse button was pressed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The left mouse button was pressed.
    /// </summary>
    Left = 1048576,

    /// <summary>
    /// The right mouse button was pressed.
    /// </summary>
    Right = 2097152,

    /// <summary>
    /// The middle mouse button was pressed.
    /// </summary>
    Middle = 4194304,

    /// <summary>
    /// The first XButton was pressed.
    /// </summary>
    XButton1 = 8388608,

    /// <summary>
    /// The second XButton was pressed.
    /// </summary>
    XButton2 = 16777216
}
