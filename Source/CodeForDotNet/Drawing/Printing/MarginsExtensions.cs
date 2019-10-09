using System;
using System.Drawing.Printing;

namespace CodeForDotNet.Drawing.Printing
{
	/// <summary>
	/// Extensions for work with <see cref="Margins"/> types.
	/// </summary>
	public static class MarginsExtensions
	{
		#region Public Methods

		/// <summary>
		/// Creates a <see cref="PageMarginsData"/> based on the parameters of an existing <see cref="Margins"/>.
		/// </summary>
		/// <remarks>The font is not touched by this instance other than to read it's properties and must be disposed as usual by the caller or other owner.</remarks>
		public static PageMarginsData ToData(this Margins value)
		{
			// Validate.
			if (value is null) throw new ArgumentNullException(nameof(value));

			// Return data.
			return new PageMarginsData(value.Left, value.Right, value.Top, value.Bottom);
		}

		/// <summary>
		/// Creates a <see cref="Margins"/> with the current properties.
		/// </summary>
		public static Margins ToMargins(this PageMarginsData value)
		{
			// Validate.
			if (value is null) throw new ArgumentNullException(nameof(value));

			// Return margins.
			return new Margins(value.Left, value.Right, value.Top, value.Bottom);
		}

		#endregion Public Methods
	}
}
