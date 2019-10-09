using System;

namespace CodeForDotNet.Xml
{
	/// <summary>
	/// Formatting options for the <see cref="XmlFullExtensions.FormatXml"/> method.
	/// </summary>
	[Flags]
	public enum XmlFormatOptions
	{
		/// <summary>
		/// No formatting.
		/// </summary>
		None = 0,

		/// <summary>
		/// Ignore any errors parsing the XML, in which case the unformatted source is returned, i.e. without change.
		/// </summary>
		IgnoreErrors = 1,

		/// <summary>
		/// Exclude the XML declaration processing instruction, i.e. &lt;?xml version="1.0" encoding="..." ?/&gt;
		/// </summary>
		OmitXmlDeclaration = 2,

		/// <summary>
		/// When set indent elements according to their tree level and removes extra white space around nodes. When not set strips all white space around nodes.
		/// Use with the <see cref="Trim"/> option to also strip white space within nodes. Include this option to make the XML easier to read. Exclude this
		/// option to save space.
		/// </summary>
		Indent = 4,

		/// <summary>
		/// Trims all content, removing white space from all element and attribute text. This is independent of the <see cref="Indent"/> option which controls
		/// white space around nodes.
		/// </summary>
		Trim = 8,

		/// <summary>
		/// Removes all comments when set.
		/// </summary>
		OmitComments = 16,

		/// <summary>
		/// Removes all namespaces, making them anonymous.
		/// </summary>
		OmitNamespaces = 32
	}
}
