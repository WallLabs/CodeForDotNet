using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.WiaSubType"/>.
    /// </summary>
    public enum WiaSubtype
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Range.
        /// </summary>
        Range = 1,

        /// <summary>
        /// List.
        /// </summary>
        List = 2,

        /// <summary>
        /// Flag.
        /// </summary>
        Flag = 3,
    }
}
