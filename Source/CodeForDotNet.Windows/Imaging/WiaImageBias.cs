﻿using System.Diagnostics.CodeAnalysis;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.WiaImageBias"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification = "False positive, it is not plural.")]
    public enum WiaImageBias
    {
        /// <summary>
        /// Minimize size.
        /// </summary>
        MinimizeSize = 65536,

        /// <summary>
        /// Maximize quality.
        /// </summary>
        MaximizeQuality = 131072
    }
}