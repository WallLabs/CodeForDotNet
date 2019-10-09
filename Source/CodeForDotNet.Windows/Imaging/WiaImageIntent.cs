using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Windows.Imaging
{
	/// <summary>
	/// Managed <see cref="WiaImageIntent"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Usage", "CA1027", Justification = "Is not a flags enumeration.")]
	public enum WiaImageIntent
	{
		/// <summary>
		/// Unspecified.
		/// </summary>
		Unspecified = 0,

		/// <summary>
		/// Color.
		/// </summary>
		Color = 1,

		/// <summary>
		/// Grayscale.
		/// </summary>
		Grayscale = 2,

		/// <summary>
		/// Text.
		/// </summary>
		Text = 4
	}
}
