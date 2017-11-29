using System;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Brush type.
    /// </summary>
    [Serializable]
    public enum LogicalBrushType
    {
        /// <summary>
        /// A simple, single color brush.
        /// </summary>
        SingleColor,

        /// <summary>
        /// A two color brush with a gradient between the start and end color.
        /// </summary>
        TwoColorGradient,

        /// <summary>
        /// A texture brush.
        /// </summary>
        Texture
    }
}