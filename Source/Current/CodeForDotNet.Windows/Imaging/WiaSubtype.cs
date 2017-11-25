using Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="WiaSubType"/>.
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
