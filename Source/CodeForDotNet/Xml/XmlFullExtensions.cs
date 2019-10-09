using CodeForDotNet.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace CodeForDotNet.Xml
{
	/// <summary>
	/// Creates extensions and helper methods for working with XML.
	/// </summary>
	public static class XmlFullExtensions
	{
		#region Public Fields

		/// <summary>
		/// Name of the resource containing the <see cref="FormatXmlAnyTransform"/>.
		/// </summary>
		public const string FormatXmlAnyXsltFileName = "FormatXmlAnonymous.xslt";

		/// <summary>
		/// Name of the resource containing the <see cref="FormatXmlCopyTransform"/>.
		/// </summary>
		public const string FormatXmlCopyXsltFileName = "FormatXmlCopy.xslt";

		/// <summary>
		/// Name of the resource containing the <see cref="FormatXmlTrimTransform"/>.
		/// </summary>
		public const string FormatXmlTrimXsltFileName = "FormatXmlTrim.xslt";

		#endregion Public Fields

		#region Private Fields

		private static XslCompiledTransform? _formatXmlAnyTransform;

		private static XslCompiledTransform? _formatXmlCopyTransform;

		private static XslCompiledTransform? _formatXmlTrimTransform;

		#endregion Private Fields

		#region Public Properties

		/// <summary>
		/// XSLT which removes namespaces from the XML, making it anonymous. Used by the <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
		/// </summary>
		public static XslCompiledTransform FormatXmlAnyTransform
		{
			get
			{
				// Load on first use
				if (_formatXmlAnyTransform == null)
				{
					// Load transform
					_formatXmlAnyTransform = new XslCompiledTransform();
					using var resource = typeof(XmlFullExtensions).Assembly.GetManifestResourceStream(
						typeof(XmlFullExtensions).Namespace + "." + FormatXmlAnyXsltFileName);
					Debug.Assert(resource != null);
					using var reader = XmlReader.Create(resource);
					_formatXmlAnyTransform.Load(reader);
				}

				// Return cached value
				return _formatXmlAnyTransform;
			}
		}

		/// <summary>
		/// XSLT which simply copies all XML content. Used by the <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
		/// </summary>
		/// <remarks>
		/// Normally this transform would not be needed because it does nothing and the <see cref="XmlWriter"/> can perform indenting directly. However for
		/// performance and to workaround an issue (wrapping a MemoryStream with a StreamWriter then an XmlWriter produces null output) all formatting is passed
		/// through the XSL engine which supports the switching of encoding at the same time as writing to an <see cref="XmlWriter"/> with specific settings. The
		/// <see cref="XmlWriter"/> used during the transform controls the indent option.
		/// </remarks>
		public static XslCompiledTransform FormatXmlCopyTransform
		{
			get
			{
				// Load on first use
				if (_formatXmlCopyTransform == null)
				{
					// Load transform
					_formatXmlCopyTransform = new XslCompiledTransform();
					using var resource = typeof(XmlFullExtensions).Assembly.GetManifestResourceStream(
						typeof(XmlFullExtensions).Namespace + "." + FormatXmlCopyXsltFileName);
					Debug.Assert(resource != null);
					using var reader = XmlReader.Create(resource);
					_formatXmlCopyTransform.Load(reader);
				}

				// Return cached value
				return _formatXmlCopyTransform;
			}
		}

		/// <summary>
		/// XSLT which trims XML content fully including attribute values and element content. Used by the
		/// <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
		/// </summary>
		public static XslCompiledTransform FormatXmlTrimTransform
		{
			get
			{
				// Load on first use
				if (_formatXmlTrimTransform == null)
				{
					// Load transform
					_formatXmlTrimTransform = new XslCompiledTransform();
					using var resource = typeof(XmlFullExtensions).Assembly.GetManifestResourceStream(
						typeof(XmlFullExtensions).Namespace + "." + FormatXmlTrimXsltFileName);
					Debug.Assert(resource != null);
					using var reader = XmlReader.Create(resource);
					_formatXmlTrimTransform.Load(reader);
				}

				// Return cached value
				return _formatXmlTrimTransform;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Creates an <see cref="XPathDocument"/> from this string assuming it contains well formed XML data.
		/// </summary>
		public static XPathDocument CreateXPathDocument(this string xml)
		{
			var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
			using var xmlReader = XmlReader.Create(new StringReader(xml), settings);
			return new XPathDocument(xmlReader);
		}

		/// <summary>
		/// Deletes all child nodes of the specified type.
		/// </summary>
		/// <param name="navigator">Extension instance.</param>
		/// <param name="type">The <see cref="XPathNodeType"/> of children to delete.</param>
		public static void DeleteChildren(this XPathNavigator navigator, XPathNodeType type)
		{
			// Validate
			if (navigator == null) throw new ArgumentNullException(nameof(navigator));

			// Delete...
			foreach (XPathNavigator child in navigator.SelectChildren(type))
				child.DeleteSelf();
		}

		/// <summary>
		/// Formats an XML string applying specific encoding and formatting options.
		/// </summary>
		/// <param name="value">String to which this extension method applies.</param>
		/// <param name="encoding">
		/// Encoding use for the transformed output. Use the same encoding as the target device to preserve all content, e.g. HTTP request encoding or database
		/// XML encoding (Unicode for SQL Server).
		/// </param>
		/// <param name="options">Formatting options.</param>
		/// <returns>Formatted string.</returns>
		public static string FormatXml(this string value, Encoding encoding, XmlFormatOptions options = XmlFormatOptions.Indent | XmlFormatOptions.Trim)
		{
			// Validate
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (encoding == null) throw new ArgumentNullException(nameof(encoding));

			// Format...
			try
			{
				// Create XML settings from formatting options
				var outputSettings = new XmlWriterSettings
				{
					OmitXmlDeclaration = (options & XmlFormatOptions.OmitXmlDeclaration) != 0,
					Encoding = encoding,
					Indent = (options & XmlFormatOptions.Indent) != 0,
					ConformanceLevel = ConformanceLevel.Fragment,
					NewLineHandling = NewLineHandling.Replace
				};
				var inputSettings = new XmlReaderSettings
				{
					IgnoreWhitespace = (options & (XmlFormatOptions.Trim | XmlFormatOptions.Indent)) != 0,
					IgnoreComments = (options & XmlFormatOptions.OmitComments) != 0,
					ConformanceLevel = ConformanceLevel.Fragment
				};

				// Decide which transform to use for best performance
				XslCompiledTransform transform;
				if ((options & XmlFormatOptions.OmitNamespaces) != 0)
					transform = FormatXmlAnyTransform;
				else if ((options & XmlFormatOptions.Trim) != 0)
					transform = FormatXmlTrimTransform;
				else
					transform = FormatXmlCopyTransform;

				// Apply transform and switch to Unicode encoding
				var buffer = new StringBuilder();
				using (var reader = XmlReader.Create(new StringReader(value), inputSettings))
				using (var writer = XmlWriter.Create(buffer, outputSettings))
				{
					if ((options & XmlFormatOptions.OmitXmlDeclaration) == 0)
					{
						// Add XML declaration manually when required because it is always removed when we support fragments
						writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + encoding.BodyName + "\"");
					}
					transform.Transform(reader, null, writer);
				}

				// Return result
				return buffer.ToString();
			}
			catch
			{
				// Ignore error if specified
				if ((options | XmlFormatOptions.IgnoreErrors) != 0)
					return value;
				throw;
			}
		}

		/// <summary>
		/// Gets a unique list of all prefixes and their namespaces from a <see cref="XmlSchemaSet"/>
		/// </summary>
		public static Dictionary<string, string> GetNamespaces(this XmlSchemaSet schemas)
		{
			// Validate
			if (schemas == null) throw new ArgumentNullException(nameof(schemas));

			// Get namespaces
			var namespaces = new Dictionary<string, string>();
			foreach (XmlSchema schema in schemas.Schemas())
			{
				foreach (var qname in schema.Namespaces.ToArray())
				{
					if (!namespaces.ContainsKey(qname.Name))
						namespaces.Add(qname.Name, qname.Namespace);
				}
			}
			return namespaces;
		}

		/// <summary>
		/// Gets <see cref="XmlReaderSettings"/> configured to validate the specified schemas and throw exceptions when warnings occur.
		/// </summary>
		/// <param name="schemas">Schemas to validate.</param>
		/// <returns>Settings to specify in the <see cref="XmlReader"/> to cause validation to occur.</returns>
		public static XmlReaderSettings GetReaderValidationSettings(XmlSchemaSet schemas)
		{
			var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
			settings.Schemas.Add(schemas);
			settings.ValidationFlags =
				XmlSchemaValidationFlags.ReportValidationWarnings |
				XmlSchemaValidationFlags.AllowXmlAttributes |
				XmlSchemaValidationFlags.ProcessIdentityConstraints;
			settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs args)
			{
				var lineInfo = (IXmlLineInfo)sender;
				throw new FormatException(string.Format(CultureInfo.CurrentCulture,
					Resources.XmlValdiationError, lineInfo.LineNumber, lineInfo.LinePosition,
					args.Exception.Message), args.Exception);
			};
			return settings;
		}

		/// <summary>
		/// Loads an XSLT transform from an XML string.
		/// </summary>
		/// <param name="transform"><see cref="XslCompiledTransform"/> to which this extension applies.</param>
		/// <param name="xml">String containing the XML to load.</param>
		public static void LoadXml(this XslCompiledTransform transform, string xml)
		{
			// Validate
			if (transform == null) throw new ArgumentNullException(nameof(transform));
			if (xml == null) throw new ArgumentNullException(nameof(xml));

			// Load XML...
			using var xmlReader = XmlReader.Create(new StringReader(xml));
			transform.Load(xmlReader);
		}

		/// <summary>
		/// Adds the schemas if they are not already present.
		/// </summary>
		public static void Merge(this XmlSchemaSet targetSchemas, XmlSchemaSet schemas)
		{
			// Validate
			if (targetSchemas == null) throw new ArgumentNullException(nameof(targetSchemas));
			if (schemas == null) throw new ArgumentNullException(nameof(schemas));

			// Merge...
			foreach (XmlSchema schema in schemas.Schemas())
			{
				if (!targetSchemas.Contains(schema.TargetNamespace))
					targetSchemas.Add(schema);
			}
		}

		/// <summary>
		/// Overload of the <see cref="XmlReader.ReadElementContentAsDateTime()"/> which behaves similarly to the <see cref="XmlReader.ReadElementString()"/>
		/// method. Required because the <see cref="XmlReader.ReadElementContentAsDateTime()"/> is not robust enough because it does not move to the next element
		/// before reading, so fails if any other content (whitespace, comments, etc...) appears at the current position, just before the next element to read.
		/// </summary>
		/// <param name="reader">Extension target.</param>
		/// <param name="localName">
		/// Optional, when both this and the <paramref name="namespace"/> parameter are specified the <see cref="XmlReader.ReadStartElement(string, string)"/>
		/// method is called before reading the content. If only this parameter is specified the <see cref="XmlReader.ReadStartElement(string)"/> overload is
		/// called, otherwise the <see cref="XmlReader.ReadStartElement()"/>.
		/// </param>
		/// <param name="namespace">Optional, when specified causes the <see cref="XmlReader.ReadStartElement(string, string)"/> to be called.</param>
		/// <return>Value read from the current or specified (name/namespace) element.</return>
		public static DateTime ReadElementDateTime(this XmlReader reader, string? localName = null, string? @namespace = null)
		{
			// Validate
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			// Read element start (moving past any other non-element content) using the appropriate overload depending on parameters passed
			if (@namespace != null)
				reader.ReadStartElement(localName, @namespace);
			else if (localName != null)
				reader.ReadStartElement(localName);
			else
				reader.ReadStartElement();

			// Read value
			var value = reader.ReadContentAsDateTime();

			// Read element end
			reader.ReadEndElement();

			// Return result
			return value;
		}

		#endregion Public Methods
	}
}
