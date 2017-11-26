using System;
using System.Drawing;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Logical <see cref="Font"/>. Facilitates storage of font values without creating and managing resources.
    /// </summary>
    public class LogicalFont : ICloneable
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public LogicalFont()
        {
        }

        /// <summary>
        /// Creates an instance with the specified properties.
        /// </summary>
        public LogicalFont(string family, float size, FontStyle style)
        {
            // Initialize properties
            Family = family;
            Size = size;
            Style = style;
        }

        /// <summary>
        /// Creates an instance based on the parameters of an existing font.
        /// </summary>
        /// <remarks>
        /// The font is not touched by this instance other than to read it's properties
        /// and must be disposed as usual by the caller or other owner.
        /// </remarks>
        public LogicalFont(Font font)
        {
            // Validate
            if (font == null) throw new ArgumentNullException("font");

            // Initialize properties
            Family = font.FontFamily.Name;
            Size = font.SizeInPoints;
            Style = font.Style;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Font family.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Font size in "em" points.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Font style.
        /// </summary>
        public FontStyle Style { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a <see cref="Font"/> with the current properties.
        /// </summary>
        public Font ToFont()
        {
            return new Font(Family, Size, Style);
        }

        /// <summary>
        /// Creates an instance from a <see cref="Font"/>.
        /// </summary>
        public static LogicalFont FromFont(Font font)
        {
            return new LogicalFont(font);
        }

        /// <summary>
        /// Creates an instance from a string.
        /// </summary>
        public static LogicalFont Parse(string value)
        {
            var convert = new FontConverter();
            using (var font = (Font)convert.ConvertFromInvariantString(value))
                return FromFont(font);
        }

        /// <summary>
        /// Displays a string representation of the font.
        /// </summary>
        public override string ToString()
        {
            var convert = new FontConverter();
            using (var font = ToFont())
                return convert.ConvertToInvariantString(font);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(LogicalFont font1, LogicalFont font2)
        {
            return Equals(font1, font2);
        }

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(LogicalFont font1, LogicalFont font2)
        {
            return !Equals(font1, font2);
        }

        /// <summary>
        /// Overrides the Equals method to compare by value,
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check type
            var other = obj as LogicalFont;

            // Check nullability without operator (would cause endless loop)
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other.Family == Family &&
                Math.Abs(other.Size - Size) < Single.Epsilon &&
                other.Style == Style;
        }

        /// <summary>
        /// Gets a hash-code for this object based on current values.
        /// </summary>
        public override int GetHashCode()
        {
            return
                (Family ?? "").GetHashCode() ^
                Size.GetHashCode() ^
                Style.GetHashCode();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public LogicalFont Copy()
        {
            return new LogicalFont(Family, Size, Style);
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public object Clone()
        {
            return Copy();
        }

        #endregion
    }
}
