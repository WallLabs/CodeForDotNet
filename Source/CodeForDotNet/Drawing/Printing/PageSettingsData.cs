using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Drawing.Printing
{
	/// <summary>
	/// Printer page settings.
	/// </summary>
	[Serializable]
	[SuppressMessage("Microsoft.Usage", "CA2235", Justification = "Custom member types are serializable. Rest are false positive, e.g. built-in value types do not need to be marked serializable.")]
	public class PageSettingsData
	{
		#region Public Constructors

		/// <summary>
		/// Initializes an empty instance.
		/// </summary>
		public PageSettingsData()
		{
			Color = true;
			Margins = new PageMarginsData();
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// Page color, defaults to true.
		/// </summary>
		public bool Color { get; set; }

		/// <summary>
		/// Landscape mode, otherwise portrait, defaults false.
		/// </summary>
		public bool Landscape { get; set; }

		/// <summary>
		/// Margins.
		/// </summary>
		public PageMarginsData Margins { get; set; }

		/// <summary>
		/// Paper height.
		/// </summary>
		public int PaperHeight { get; set; }

		/// <summary>
		/// Paper name.
		/// </summary>
		public string? PaperName { get; set; }

		/// <summary>
		/// Paper size name.
		/// </summary>
		public int PaperSizeKind { get; set; }

		/// <summary>
		/// Paper width.
		/// </summary>
		public int PaperWidth { get; set; }

		/// <summary>
		/// Printer name.
		/// </summary>
		public string? PrinterName { get; set; }

		#endregion Public Properties
	}
}
