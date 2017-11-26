using System;
using System.Drawing.Printing;

namespace CodeForDotNet.Drawing.Printing
{
    /// <summary>
    /// Printer settings data.
    /// </summary>
    [Serializable]
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
        public Duplex DuplexOutput { get; set; }

        /// <summary>
        /// Printer name.
        /// </summary>
        public string PrinterName { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts this data to a .NET PrinterSettings class.
        /// </summary>
        static public PrinterSettings ToPrinterSettings(PrinterSettingsData source)
        {
            // Validate
            if (source == null) throw new ArgumentNullException("source");

            // Convert...
            var target = new PrinterSettings();
            try
            {
                // Attempt to construct printer settings
                target.Collate = source.Collate;
                target.Copies = source.Copies;
                target.Duplex = source.DuplexOutput;
                target.PrinterName = source.PrinterName;
            }
            catch
            {
                // Reset to defaults when the data is corrupt
                target = new PrinterSettings();
            }
            return target;
        }

        /// <summary>
        /// Default printer settings.
        /// </summary>
        static public PrinterSettingsData Default
        {
            get { return new PrinterSettingsData { Collate = true, DuplexOutput = Duplex.Simplex, Copies = 1 }; }
        }

        /// <summary>
        /// Converts a .NET PrinterSettings class to a data structure of this type.
        /// </summary>
        static public PrinterSettingsData FromPrinterSettings(PrinterSettings source)
        {
            // Validate
            if (source == null) throw new ArgumentNullException("source");

            // Convert...
            return new PrinterSettingsData
                       {
                           Collate = source.Collate,
                           Copies = source.Copies,
                           DuplexOutput = source.Duplex,
                           PrinterName = source.PrinterName
                       };
        }

        #endregion
    }
}
