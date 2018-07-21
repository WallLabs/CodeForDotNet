using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.WiaImageBias"/>.
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
