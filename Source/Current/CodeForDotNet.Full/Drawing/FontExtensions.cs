using CodeForDotNet.Drawing;
using System;
using System.Drawing;

namespace CodeForDotNet.Full.Drawing
{
    /// <summary>
    /// Extensions for work with <see cref="Font"/> types.
    /// </summary>
    public static class FontExtensions
    {
        #region LogicalFont Conversion

        /// <summary>
        /// Creates a <see cref="Font"/> with the current properties.
        /// </summary>
        public static Font ToFont(this LogicalFont data)
        {
            // Validate
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Create and return system font
            return new Font(data.Family, data.Size, (FontStyle)(int)data.Style);
        }

        /// <summary>
        /// Creates a <see cref="LogicalFont"/> based on the parameters of an existing <see cref="Font"/>.
        /// </summary>
        /// <remarks>
        /// The font is not touched by this instance other than to read it's properties
        /// and must be disposed as usual by the caller or other owner.
        /// </remarks>
        public static LogicalFont ToLogicalFont(this Font font)
        {
            // Validate
            if (font == null) throw new ArgumentNullException(nameof(font));

            // Initialize properties
            return new LogicalFont(
                font.FontFamily.Name,
                font.SizeInPoints,
                (LogicalFontStyle)(int)font.Style);
        }

        /// <summary>
        /// Creates a <see cref="LogicalFont"/> from a string.
        /// </summary>
        public static LogicalFont ToLogicalFont(string value)
        {
            var convert = new FontConverter();
            using (var font = (Font)convert.ConvertFromInvariantString(value))
                return ToLogicalFont(font);
        }

        /// <summary>
        /// Displays a string representation of the <see cref="LogicalFont"/>.
        /// </summary>
        public static string ToFontString(this LogicalFont data)
        {
            var convert = new FontConverter();
            using (var font = ToFont(data))
                return convert.ConvertToInvariantString(font);
        }

        #endregion
    }
}
