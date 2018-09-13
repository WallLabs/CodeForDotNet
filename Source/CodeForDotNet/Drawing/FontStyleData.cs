namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Font style, defined separately from any API so it can be used in future proof
    /// data serialization and cross-platform applications.
    /// </summary>
    /// <remarks>
    /// Member names and values map directly to the Universal Windows contract.
    /// </remarks>
    public enum FontStyleData
    {
        /// <summary>
        /// Normal.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Oblique style.
        /// </summary>
        Oblique = 1,

        /// <summary>
        /// Italic.
        /// </summary>
        Italic = 2
    }
}