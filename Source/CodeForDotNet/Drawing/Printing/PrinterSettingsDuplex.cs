namespace CodeForDotNet.Drawing.Printing
{
    /// <summary>
    /// Defines a printer duplex setting which is not bound to any API,
    /// so it can be used for future proof data serialization and cross-platform.
    /// </summary>
    public enum PrinterSettingsDuplex
    {
        /// <summary>
        /// The printer's default duplex setting.
        /// </summary>
        Default,

        /// <summary>
        /// Double-sided, horizontal printing.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Single-sided printing.
        /// </summary>
        Simplex,

        /// <summary>
        /// Double-sided, vertical printing.
        /// </summary>
        Vertical
    }
}
