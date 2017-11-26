using System;
using System.ComponentModel;
using System.Drawing.Printing;

namespace CodeForDotNet.Drawing.Printing
{
    /// <summary>
    /// Printer page settings.
    /// </summary>
    [Serializable]
    public class PageSettingsData
    {
        #region Properties

        /// <summary>
        /// Page color.
        /// </summary>
        public bool Color { get; set; }

        /// <summary>
        /// Landscape mode, otherwise portrait.
        /// </summary>
        public bool Landscape { get; set; }

        /// <summary>
        /// Margins.
        /// </summary>
        public string Margins { get; set; }

        /// <summary>
        /// Paper width.
        /// </summary>
        public int PaperWidth { get; set; }

        /// <summary>
        /// Paper height.
        /// </summary>
        public int PaperHeight { get; set; }

        /// <summary>
        /// Paper name.
        /// </summary>
        public string PaperName { get; set; }

        /// <summary>
        /// Paper size name.
        /// </summary>
        public int PaperSizeKind { get; set; }

        /// <summary>
        /// Printer name.
        /// </summary>
        public string PrinterName { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default page settings.
        /// </summary>
        static public PageSettingsData Default
        {
            get
            {
                return new PageSettingsData {Color = true, Landscape = true, Margins = "", PaperName = "A4"};
            }
        }

        /// <summary>
        /// Converts this data to a .NET PageSettings class.
        /// </summary>
        static public PageSettings ToPageSettings(PageSettingsData source)
        {
            // Validate
            if (source == null) throw new ArgumentNullException("source");

            // Convert...
            var target = new PageSettings();
            try
            {
                // Attempt to construct page settings
                target.Color = source.Color;
                target.Landscape = source.Landscape;
                target.Margins = (Margins)new MarginsConverter().ConvertFromInvariantString(source.Margins) ??
                                 new Margins(0, 0, 0, 0);
                var printer = new PrinterSettings { PrinterName = source.PrinterName };
                foreach (PaperSize paperSize in printer.PaperSizes)
                {
                    if (paperSize.PaperName == source.PaperName)
                        target.PaperSize = paperSize;
                }
            }
            catch
            {
                // Reset to defaults when the data is corrupt
                target = new PageSettings();
            }
            return target;
        }

        /// <summary>
        /// Creates a data structure of this type from .NET PageSettings.
        /// </summary>
        static public PageSettingsData FromPageSettings(PageSettings source)
        {
            // Validate
            if (source == null) throw new ArgumentNullException("source");

            // Convert...
            var target = new PageSettingsData
                             {
                                 Color = source.Color,
                                 Landscape = source.Landscape,
                                 Margins =
                                     TypeDescriptor.GetConverter(typeof(Margins)).ConvertToInvariantString(
                                         source.Margins),
                                 PaperWidth = source.PaperSize.Width,
                                 PaperHeight = source.PaperSize.Height,
                                 PaperName = source.PaperSize.PaperName,
                                 PaperSizeKind = (int)source.PaperSize.Kind,
                                 PrinterName = source.PrinterSettings.PrinterName
                             };
            return target;
        }

        #endregion
    }
}
