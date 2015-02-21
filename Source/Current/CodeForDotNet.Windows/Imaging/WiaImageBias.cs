using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.WiaImageBias"/>.
    /// </summary>
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
