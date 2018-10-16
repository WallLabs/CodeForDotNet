using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Drawing.Printing
{
    /// <summary>
    /// Printer settings data.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Usage", "CA2235", Justification = "Custom member types are serializable. Rest are false positive, e.g. built-in value types do not need to be marked serializable.")]
    public class PrinterSettingsData
    {
        #region Properties

        /// <summary>
        /// Collate option.
        /// </summary>
        public bool Collate { get; set; }

        /// <summary>
        /// Number of copies.
        /// </summary>
        public short Copies { get; set; }

        /// <summary>
        /// Duplex option.
        /// </summary>
        public PrinterSettingsDuplex DuplexOutput { get; set; }

        /// <summary>
        /// Printer name.
        /// </summary>
        public string PrinterName { get; set; }

        /// <summary>
        /// Default printer settings.
        /// </summary>
        public static PrinterSettingsData Default
        {
            get { return new PrinterSettingsData { Collate = true, DuplexOutput = PrinterSettingsDuplex.Simplex, Copies = 1 }; }
        }

        #endregion Properties
    }
}