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
        /// A simple, single colour brush.
        /// </summary>
        SingleColor,

        /// <summary>
        /// A two colour brush with a gradient between the start and end color.
        /// </summary>
        TwoColorGradient,

        /// <summary>
        /// A texture brush.
        /// </summary>
        Texture
    }
}