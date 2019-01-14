using CodeForDotNet.Xml;
using System;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Font data, disconnected from any API dependencies, useful for
    /// future proof serialization and cross-platform.
    /// </summary>
    public class FontData : ICloneable
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public FontData()
        {
        }

        /// <summary>
        /// Creates an instance with the specified properties.
        /// </summary>
        public FontData(string family, float size, FontStyle style)
        {
            // Initialize properties
            Family = family;
            Size = size;
            Style = style;
        }

        #endregion Lifetime

        #region Operators

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(FontData font1, FontData font2)
        {
            return Equals(font1, font2);
        }

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(FontData font1, FontData font2)
        {
            return !Equals(font1, font2);
        }

        /// <summary>
        /// Overrides the Equals method to compare by value,
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check type
            var other = obj as FontData;

            // Check nullability without operator (would cause endless loop)
            if (other is null)
                return false;

            // Compare values
            return
                other.Family == Family &&
                Math.Abs(other.Size - Size) < float.Epsilon &&
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

        #endregion Operators

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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the brush to an XML string.
        /// </summary>
        public override string ToString()
        {
            return this.SerializeXml();
        }

        /// <summary>
        /// Creates an instance from a string.
        /// </summary>
        public static FontData Parse(string value)
        {
            return XmlSerializerExtensions.DeserializeXml<FontData>(value);
        }

        #endregion Public Methods

        #region ICloneable Members

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public FontData Copy()
        {
            return new FontData(Family, Size, Style);
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public object Clone()
        {
            return Copy();
        }

        #endregion ICloneable Members
    }
}