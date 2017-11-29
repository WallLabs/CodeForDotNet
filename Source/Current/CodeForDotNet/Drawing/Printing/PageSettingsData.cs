using System;

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

        /// <summary>
        /// Default page settings.
        /// </summary>
        public static PageSettingsData Default
        {
            get
            {
                return new PageSettingsData {Color = true, Landscape = true, Margins = "", PaperName = "A4"};
            }
        }

        #endregion
    }
}
