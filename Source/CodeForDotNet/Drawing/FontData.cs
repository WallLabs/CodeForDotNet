using CodeForDotNet.Xml;
using System;

namespace CodeForDotNet.Drawing
{
	/// <summary>
	/// Font data, disconnected from any API dependencies, useful for future proof serialization and cross-platform.
	/// </summary>
	public class FontData : ICloneable
	{
		#region Public Constructors

		/// <summary>
		/// Creates an empty instance.
		/// </summary>
		public FontData()
		{
			Family = "";
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

		#endregion Public Constructors

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
		/// Overrides the inequality operator to compare by value.
		/// </summary>
		public static bool operator !=(FontData font1, FontData font2)
		{
			return !Equals(font1, font2);
		}

		/// <summary>
		/// Overrides the equality operator to compare by value.
		/// </summary>
		public static bool operator ==(FontData font1, FontData font2)
		{
			return Equals(font1, font2);
		}

		/// <summary>
		/// Creates an instance from a string.
		/// </summary>
		public static FontData Parse(string value)
		{
			return XmlSerializerExtensions.DeserializeXml<FontData>(value);
		}

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		public object Clone()
		{
			return Copy();
		}

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		public FontData Copy()
		{
			return new FontData(Family, Size, Style);
		}

		/// <summary>
		/// Overrides the Equals method to compare by value,
		/// </summary>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			// Check type and nullability without operator (would cause endless loop).
			if (!(other is FontData font))
				return false;

			// Compare values.
			return
				font.Family == Family &&
				Math.Abs(font.Size - Size) < float.Epsilon &&
				font.Style == Style;
		}

		/// <summary>
		/// Gets a hash-code for this object based on current values.
		/// </summary>
		public override int GetHashCode()
		{
			return
				Family.GetHashCode() ^
				Size.GetHashCode() ^
				Style.GetHashCode();
		}

		/// <summary>
		/// Converts the brush to an XML string.
		/// </summary>
		public override string ToString()
		{
			return this.SerializeXml();
		}

		#endregion Public Methods
	}
}
