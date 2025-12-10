using System;
using System.Diagnostics.CodeAnalysis;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Drawing.Printing;

/// <summary>
/// Margins data.
/// </summary>
[Serializable]
public class PageMarginsData : ICloneable
{
    #region Public Constructors

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

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Gets or sets the bottom margin, in hundredths of an inch.
    /// </summary>
    public int Bottom { get; set; }

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

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Overrides the inequality operator to compare by value.
    /// </summary>
    public static bool operator !=(PageMarginsData margins1, PageMarginsData margins2)
    {
        return !Equals(margins1, margins2);
    }

    /// <summary>
    /// Overrides the equality operator to compare by value.
    /// </summary>
    public static bool operator ==(PageMarginsData margins1, PageMarginsData margins2)
    {
        return Equals(margins1, margins2);
    }

    /// <summary>
    /// Creates an instance from a string.
    /// </summary>
    public static PageMarginsData Parse(string value)
    {
        return XmlSerializerExtensions.DeserializeXml<PageMarginsData>(value);
    }

    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    public object Clone()
    {
        return Copy();
    }

    /// <summary>
    /// Copies this margins.
    /// </summary>
    public PageMarginsData Copy()
    {
        return new PageMarginsData(Left, Right, Top, Bottom);
    }

    /// <summary>
    /// Overrides the Equals method to compare by value,
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override bool Equals(object other)
    {
        // Check type and nullability
        if (other is not PageMarginsData margins)
            return false;

        // Compare values
        return
            Left == margins.Left &&
            Right == margins.Right &&
            Top == margins.Top &&
            Bottom == margins.Bottom;
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

    /// <summary>
    /// Converts the margins to an XML string.
    /// </summary>
    public override string ToString()
    {
        return this.SerializeXml();
    }

    #endregion Public Methods
}
