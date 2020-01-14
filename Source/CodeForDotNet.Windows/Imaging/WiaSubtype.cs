using System.Diagnostics.CodeAnalysis;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.WiaSubType"/>.
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
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
        Flag = 3,
    }
}
