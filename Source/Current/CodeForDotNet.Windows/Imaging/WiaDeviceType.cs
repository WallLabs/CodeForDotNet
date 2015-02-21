using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.WiaDeviceType"/>
    /// </summary>
    public enum WiaDeviceType
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Scanner.
        /// </summary>
        Scanner = 1,

        /// <summary>
        /// Camera
        /// </summary>
        Camera = 2,

        /// <summary>
        /// Video camera or capture device.
        /// </summary>
        Video = 3,
    }
}
