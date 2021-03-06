using System;
using System.Drawing;

namespace CodeForDotNet.Drawing
{
	/// <summary>
	/// Extensions for work with <see cref="Font"/> types.
	/// </summary>
	public static class FontExtensions
	{
		#region Public Methods

		/// <summary>
		/// Creates a <see cref="FontData"/> based on the parameters of an existing <see cref="Font"/>.
		/// </summary>
		/// <remarks>The font is not touched by this instance other than to read it's properties and must be disposed as usual by the caller or other owner.</remarks>
		public static FontData ToData(this Font font)
		{
			// Validate
			if (font == null) throw new ArgumentNullException(nameof(font));

			// Initialize properties
			return new FontData(
				font.FontFamily.Name,
				font.SizeInPoints,
				(FontStyle)(int)font.Style);
		}

		/// <summary>
		/// Creates a <see cref="Font"/> with the current properties.
		/// </summary>
		public static Font ToFont(this FontData data)
		{
			// Validate
			if (data is null) throw new ArgumentNullException(nameof(data));

			// Create and return system font
			return new Font(data.Family, data.Size, (System.Drawing.FontStyle)(int)data.Style);
		}

		#endregion Public Methods
	}
}
