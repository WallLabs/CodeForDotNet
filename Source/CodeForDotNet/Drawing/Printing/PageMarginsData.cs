using CodeForDotNet.Xml;
using System;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Margins data.
    /// </summary>
    [Serializable]
    public class PageMarginsData : ICloneable
    {
        #region Lifetime

        /// <summary>
        /// Creates the margins.
        /// </summary>
        public PageMarginsData()
        {
        }

        /// <summary>
        /// Creates the margins from specified values.
        /// </summary>
        public PageMarginsData(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// Gets or sets the left margin width, in hundredths of an inch.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the right margin width, in hundredths of an inch.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the top margin width, in hundredths of an inch.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the bottom margin, in hundredths of an inch.
        /// </summary>
        public int Bottom { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Converts the margins to an XML string.
        /// </summary>
        public override string ToString()
        {
            return this.SerializeXml();
        }

        /// <summary>
        /// Creates an instance from a string.
        /// </summary>
        public static PageMarginsData Parse(string value)
        {
            return XmlSerializerExtensions.DeserializeXml<PageMarginsData>(value);
        }

        #endregion Public Methods

        #region Operators

        /// <summary>
        /// Overrides the equality operator to compare by value.
        /// </summary>
        public static bool operator ==(PageMarginsData margins1, PageMarginsData margins2)
        {
            return Equals(margins1, margins2);
        }

        /// <summary>
        /// Overrides the inequality operator to compare by value.
        /// </summary>
        public static bool operator !=(PageMarginsData margins1, PageMarginsData margins2)
        {
            return !Equals(margins1, margins2);
        }

        /// <summary>
        /// Overrides the Equals method to compare by value,
        /// </summary>
        public override bool Equals(object obj)
        {
            // Check type and nullability
            var other = obj as PageMarginsData;
            if (other is null)
                return false;

            // Compare values
            return
                Left == other.Left &&
                Right == other.Right &&
                Top == other.Top &&
                Bottom == other.Bottom;
        }

        /// <summary>
        /// Gets a hash-code for this object based on current values.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Left.GetHashCode() ^
                Right.GetHashCode() ^
                Top.GetHashCode() ^
                Bottom.GetHashCode();
        }

        #endregion Operators

        #region ICloneable Members

        /// <summary>
        /// Copies this margins.
        /// </summary>
        public PageMarginsData Copy()
        {
            return new PageMarginsData(Left, Right, Top, Bottom);
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