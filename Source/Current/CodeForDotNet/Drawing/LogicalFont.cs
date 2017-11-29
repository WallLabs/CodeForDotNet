using System;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Logical font disconnected from any API dependencies, useful for
    /// future proof serialization and cross-platform.
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
        public LogicalFont(string family, float size, LogicalFontStyle style)
        {
            // Initialize properties
            Family = family;
            Size = size;
            Style = style;
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
        public LogicalFontStyle Style { get; set; }

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
