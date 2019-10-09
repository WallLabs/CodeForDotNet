using System.Diagnostics.CodeAnalysis;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
	/// <summary>
	/// Managed <see cref="Wia.WiaImageBias"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1717", Justification = "It is not plural.")]
	[SuppressMessage("Microsoft.Usage", "CA1027", Justification = "Is not a flags enumeration.")]
	public enum WiaImageBias
	{
		/// <summary>
		/// Minimize size.
		/// </summary>
		MinimizeSize = 65536,

		/// <summary>
		/// Maximize quality.
		/// </summary>
		MaximizeQuality = 131072
	}
}
