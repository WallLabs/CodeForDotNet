using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using CodeForDotNet.Properties;

namespace CodeForDotNet.Xml;

/// <summary>
/// Creates extensions and helper methods for working with XML.
/// </summary>
[SuppressMessage("Performance", "CA1863:Use 'CompositeFormat'", Justification = "TODO: Upgrade resources.")]
public static class XmlExtensions
{
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

    /// <summary>
    /// Prefix used by XML declarations (xmlns).
    /// </summary>
    public const string XmlDeclarationPrefix = "xmlns";

    /// <summary>
    /// Preferred prefix for the XML Schema namespace.
    /// </summary>
    /// <seealso cref="XmlSchema.Namespace"/>
    public const string XsdNamespacePrefixDefault = "xs";

    /// <summary>
    /// Preferred prefix for the XML Schema Instance namespace.
    /// </summary>
    /// <seealso cref="XmlSchema.InstanceNamespace"/>
    public const string XsiNamespacePrefixDefault = "xsi";

    /// <summary>
    /// XSLT which removes namespaces from the XML, making it anonymous. Used by the
    /// <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
    /// </summary>
    public static XslCompiledTransform FormatXmlAnyTransform
    {
        get
        {
            // Load on first use
            if (field is null)
            {
                // Load transform
                field = new XslCompiledTransform();
                using var resource = typeof(XmlExtensions).Assembly.GetManifestResourceStream(
                    typeof(XmlExtensions).Namespace + "." + FormatXmlAnyXsltFileName)!;
                using var reader = XmlReader.Create(resource);
                field.Load(reader);
            }

            // Return cached targetTyoe
            return field;
        }
    }

    /// <summary>
    /// XSLT which simply copies all XML content. Used by the
    /// <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
    /// </summary>
    /// <remarks>
    /// Normally this transform would not be needed because it does nothing and the
    /// <see cref="XmlWriter"/> can perform indenting directly. However for performance and to
    /// workaround an issue (wrapping a MemoryStream with a StreamWriter then an XmlWriter
    /// produces null output) all formatting is passed through the XSL engine which supports the
    /// switching of encoding at the same time as writing to an <see cref="XmlWriter"/> with
    /// specific settings. The <see cref="XmlWriter"/> used during the transform controls the
    /// indent option.
    /// </remarks>
    public static XslCompiledTransform FormatXmlCopyTransform
    {
        get
        {
            // Load on first use
            if (field is null)
            {
                // Load transform
                field = new XslCompiledTransform();
                using var resource = typeof(XmlExtensions).Assembly.GetManifestResourceStream(
                    typeof(XmlExtensions).Namespace + "." + FormatXmlCopyXsltFileName)!;
                using var reader = XmlReader.Create(resource);
                field.Load(reader);
            }

            // Return cached targetTyoe
            return field;
        }
    }

    /// <summary>
    /// XSLT which trims XML content fully including attribute values and element content. Used
    /// by the <see cref="FormatXml(string,Encoding,XmlFormatOptions)"/> methods.
    /// </summary>
    public static XslCompiledTransform FormatXmlTrimTransform
    {
        get
        {
            // Load on first use
            if (field is null)
            {
                // Load transform
                field = new XslCompiledTransform();
                using var resource = typeof(XmlExtensions).Assembly.GetManifestResourceStream(
                    typeof(XmlExtensions).Namespace + "." + FormatXmlTrimXsltFileName)!;
                using var reader = XmlReader.Create(resource);
                field.Load(reader);
            }

            // Return cached targetTyoe
            return field;
        }
    }

    /// <summary>
    /// Looks-up or adds a namespace, returning the prefix.
    /// </summary>
    /// <param name="xmlns">XML namespaces to search.</param>
    /// <param name="namespace">Namespace to find or add.</param>
    /// <param name="preferredPrefix">
    /// Preferred prefix to assign when a new namespace entry is made, when available.
    /// </param>
    /// <returns>
    /// Existing prefix or a new generated prefix in the format "ns#" when new.
    /// </returns>
    public static string AssertPrefix(this XmlSerializerNamespaces xmlns, string @namespace, string? preferredPrefix = null)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xmlns);
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));

        // Check if already exists
        var prefix = LookupPrefix(xmlns, @namespace);

        // Return when found
        if (prefix != null)
            return prefix;

        // Check if preferred prefix is available when specified
        if (!string.IsNullOrWhiteSpace(preferredPrefix) && xmlns.LookupNamespace(preferredPrefix) is null)
        {
            // Use preferred prefix
            prefix = preferredPrefix;
        }
        else
        {
            // Generate new prefix
            prefix = GeneratePrefix(xmlns);
        }

        // Add and return
        xmlns.Add(prefix, @namespace);
        return prefix!;
    }

    /// <summary>
    /// Converts an XML value (string) to a supported .NET type.
    /// </summary>
    /// <param name="value">XML value to convert.</param>
    /// <param name="returnType">Type to convert into.</param>
    /// <returns>Converted value.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the type conversion is not supported.
    /// </exception>
    public static object ConvertValue(string value, Type returnType)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException(nameof(value));
        ArgumentNullException.ThrowIfNull(returnType);

        // Convert enumeration
        if (returnType.IsEnum)
            return Enum.Parse(returnType, value);

        // Convert other values with XmlConvert
        if (returnType == typeof(bool)) return XmlConvert.ToBoolean(value.Trim());
        if (returnType == typeof(byte)) return XmlConvert.ToByte(value.Trim());
        if (returnType == typeof(char)) return XmlConvert.ToChar(value.Trim());
        if (returnType == typeof(DateTime)) return XmlConvert.ToDateTime(value.Trim(), XmlDateTimeSerializationMode.RoundtripKind);
        if (returnType == typeof(DateTimeOffset)) return XmlConvert.ToDateTimeOffset(value.Trim());
        if (returnType == typeof(decimal)) return XmlConvert.ToDecimal(value.Trim());
        if (returnType == typeof(double)) return XmlConvert.ToDouble(value.Trim());
        if (returnType == typeof(Guid)) return XmlConvert.ToGuid(value.Trim());
        if (returnType == typeof(short)) return XmlConvert.ToInt16(value.Trim());
        if (returnType == typeof(int)) return XmlConvert.ToInt32(value.Trim());
        if (returnType == typeof(long)) return XmlConvert.ToInt64(value.Trim());
        if (returnType == typeof(sbyte)) return XmlConvert.ToSByte(value.Trim());
        if (returnType == typeof(float)) return XmlConvert.ToSingle(value.Trim());
        if (returnType == typeof(string)) return value;
        if (returnType == typeof(TimeSpan)) return XmlConvert.ToTimeSpan(value.Trim());
        if (returnType == typeof(ushort)) return XmlConvert.ToUInt16(value.Trim());
        if (returnType == typeof(uint)) return XmlConvert.ToUInt32(value.Trim());
        if (returnType == typeof(ulong)) return XmlConvert.ToUInt64(value.Trim());

        // Unsupported type
        throw new NotSupportedException();
    }

    /// <summary>
    /// Converts a supported .NET type to an XML value (string) or calls
    /// <see cref="object.ToString()"/> for other types.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value.</returns>
    public static string ConvertValueToString(object value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);

        // Return strings directly (no additional conversion required)
        if (value is string @string) return @string;

        // Convert supported value types using XmlConvert
        if (value is bool boolValue) return XmlConvert.ToString(boolValue);
        if (value is byte byteValue) return XmlConvert.ToString(byteValue);
        if (value is char charValue) return XmlConvert.ToString(charValue);
        if (value is DateTime dateTimeValue) return XmlConvert.ToString(dateTimeValue, XmlDateTimeSerializationMode.RoundtripKind);
        if (value is DateTimeOffset dateTimeOffsetValue) return XmlConvert.ToString(dateTimeOffsetValue);
        if (value is decimal decimalValue) return XmlConvert.ToString(decimalValue);
        if (value is double doubleValue) return XmlConvert.ToString(doubleValue);
        if (value is Guid guidValue) return XmlConvert.ToString(guidValue);
        if (value is short shortValue) return XmlConvert.ToString(shortValue);
        if (value is int integerValue) return XmlConvert.ToString(integerValue);
        if (value is sbyte signedByteValue) return XmlConvert.ToString(signedByteValue);
        if (value is float floatValue) return XmlConvert.ToString(floatValue);
        if (value is TimeSpan timeSpanValue) return XmlConvert.ToString(timeSpanValue);
        if (value is ushort unsignedShortValue) return XmlConvert.ToString(unsignedShortValue);
        if (value is uint unsignedIntegerValue) return XmlConvert.ToString(unsignedIntegerValue);
        if (value is ulong unsignedLongValue) return XmlConvert.ToString(unsignedLongValue);

        // Call standard object string method on all other types (includes enumerations)
        return value.ToString()!;
    }

    /// <summary>
    /// Copies XML from one <see cref="XPathNavigator"/> to another.
    /// </summary>
    /// <param name="source">
    /// Navigator positioned at the source XML location to copy (including children).
    /// </param>
    /// <param name="target">
    /// Navigator positioned at the target XML location to which the source XML will be
    /// appended.
    /// </param>
    public static void Copy(this XPathNavigator source, XPathNavigator target)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        // Copy entire XML subtree
        var readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
        using var reader = XmlReader.Create(source.ReadSubtree(), readSettings);
        target.AppendChild(reader);
    }

    /// <summary>
    /// Copies XML from a source path to a target <see cref="XmlWriter"/>, with optional path
    /// filter.
    /// </summary>
    /// <param name="sourceXml">Source XML.</param>
    /// <param name="sourcePath">
    /// Optional XPath to select/filter from the source. Leave null to copy the entire source.
    /// </param>
    /// <param name="writer">
    /// Target <see cref="XmlWriter"/> into which to copy the selected source XML.
    /// </param>
    public static void Copy(this XPathNavigator sourceXml, string? sourcePath, XmlWriter writer)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(sourceXml);

        // Copy source with select when path specified
        if (!string.IsNullOrEmpty(sourcePath))
        {
            // Select source XPath then copy each match to target
            foreach (XPathNavigator match in sourceXml.Select(sourcePath))
                match.WriteSubtree(writer);
        }
        else
        {
            // Copy entire source to target
            sourceXml.WriteSubtree(writer);
        }
    }

    /// <summary>
    /// Creates an <see cref="XPathNavigator"/> positioned within an
    /// <see cref="IXPathNavigable"/> XML document/fragment, also creating any elements in the
    /// path which do not exist.
    /// </summary>
    /// <param name="xml">XML document/fragment in which to create the path.</param>
    /// <param name="path">
    /// XPath expression to select the sub-path. Null is allowed, it will just select the root
    /// element.
    /// </param>
    /// <returns>
    /// <see cref="XPathNavigator"/> positioned at the path, which may have been created in part
    /// or whole.
    /// </returns>
    /// <remarks>
    /// Filters in the path are only allowed for elements which exist, ensuring no attempt is
    /// made to create elements with invalid schema/missing required content specified in the
    /// missing path filter.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Path cannot be created because filter content is be required but cannot be determined.
    /// </exception>
    public static XPathNavigator CreatePath(this IXPathNavigable xml, string? path)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xml);

        // Return document root when no path or root specified
        if (string.IsNullOrEmpty(path) || path == "/")
            return xml.CreateNavigator()!;

        // Attempt to get path
        var result = xml.CreateNavigator()!.SelectSingleNode(path);
        if (result is null)
        {
            // Create path when not found and requested
            result = xml.CreateNavigator();
            var currentPath = "";
            foreach (var pathPart in path!.Split(['/'], StringSplitOptions.RemoveEmptyEntries))
            {
                // Check if next part exists
                if (result != null)
                {
                    currentPath += "/" + pathPart;
                    var child = result.SelectSingleNode(pathPart);
                    if (child is null)
                    {
                        // Error when missing path includes filter (cannot automatically create
                        // conditional content)
                        if (pathPart.Contains('['))
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                Resources.XmlExtensionsCreatePathCannotCreateMissingPartWithFilter,
                                currentPath));
                        }

                        // Create path parts which do not exist
                        result.AppendChildElement("", pathPart, "", null);
                        child = result.SelectSingleNode(pathPart);
                    }

                    // Next/last part
                    result = child;
                }
            }
        }

        // Return result
        return result!;
    }

    /// <summary>
    /// Creates an <see cref="XmlReader"/> from this string assuming it contains well formed XML
    /// data.
    /// </summary>
    /// <returns>
    /// <see cref="XmlReader"/> positioned at the first node, which the caller must dispose.
    /// </returns>
    public static XmlReader CreateXmlReader(this string xml)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

        // Create and return XML reader
        var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
        var reader = XmlReader.Create(new StringReader(xml), settings);

        // Move to first node
        if (reader.NodeType == XmlNodeType.None)
        {
            // Make sure it is not empty
            if (!reader.Read())
                throw new ArgumentNullException(nameof(xml));
        }

        // Return reader positioned at first node
        return reader;
    }

    /// <summary>
    /// Creates an <see cref="XPathDocument"/> from this string assuming it contains well formed
    /// XML data.
    /// </summary>
    public static XPathDocument CreateXPathDocument(this string xml)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

        // Create and return XPath document
        using var xmlReader = xml.CreateXmlReader();
        return new XPathDocument(xmlReader);
    }

    /// <summary>
    /// Deletes all child nodes of the specified type.
    /// </summary>
    /// <param name="navigator">Extension instance.</param>
    /// <param name="type">The <see cref="XPathNodeType"/> of children to delete.</param>
    public static void DeleteChildren(this XPathNavigator navigator, XPathNodeType type)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(navigator);

        // Delete...
        foreach (XPathNavigator child in navigator.SelectChildren(type))
            child.DeleteSelf();
    }

    /// <summary>
    /// Escapes any invalid XML characters in a string, e.g. \u0008 =&gt; &amp;#x0008.
    /// </summary>
    /// <param name="value">XML string to check.</param>
    /// <returns>Encoded XML string.</returns>
    public static string EscapeInvalidXmlChars(this string value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);

        // Return string with invalid characters removed
        var result = new StringBuilder();
        foreach (var character in value)
        {
            _ = XmlConvert.IsXmlChar(character)
                ? result.Append(character)
                : result.AppendFormat(CultureInfo.InvariantCulture, "&#{0:X4}", (byte)character);
        }
        return result.ToString();
    }

    /// <summary>
    /// Formats an XML string applying specific default encoding (UTF-8) and formatting options
    /// (indent and trim).
    /// </summary>
    /// <param name="value">Source XML string to format.</param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this string value)
    {
        // Call overloaded method
        return FormatXml(value, Encoding.UTF8, XmlFormatOptions.Indent | XmlFormatOptions.Trim);
    }

    /// <summary>
    /// Formats an XML string applying specific encoding and default formatting options (indent
    /// and trim).
    /// </summary>
    /// <param name="value">Source XML string to format.</param>
    /// <param name="encoding">
    /// Encoding use for the transformed output. Use the same encoding as the target device to
    /// preserve all content, e.g. HTTP request encoding or database XML encoding (Unicode for
    /// SQL Server).
    /// </param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this string value, Encoding encoding)
    {
        // Call overloaded method
        return FormatXml(value, encoding, XmlFormatOptions.Indent | XmlFormatOptions.Trim);
    }

    /// <summary>
    /// Formats an XML string applying specific encoding and formatting options.
    /// </summary>
    /// <param name="value">Source XML string to format.</param>
    /// <param name="encoding">
    /// Encoding use for the transformed output. Use the same encoding as the target device to
    /// preserve all content, e.g. HTTP request encoding or database XML encoding (Unicode for
    /// SQL Server).
    /// </param>
    /// <param name="options">Formatting options.</param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this string value, Encoding encoding, XmlFormatOptions options)
    {
        try
        {
            // Convert to XML document then call overloaded method
            var xml = value.CreateXPathDocument().CreateNavigator();
            return FormatXml(xml, encoding, options);
        }
        catch
        {
            // Ignore error when option specified (return original value)
            if ((options | XmlFormatOptions.IgnoreErrors) != 0)
                return value;

            // Otherwise throw error
            throw;
        }
    }

    /// <summary>
    /// Formats XML applying specific default encoding (UTF-8) and formatting options (indent
    /// and trim).
    /// </summary>
    /// <param name="value">
    /// XML navigator positioned at the source XML location to format (including children).
    /// </param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this XPathNavigator value)
    {
        // Call overloaded method with defaults
        return FormatXml(value, Encoding.UTF8, XmlFormatOptions.Indent | XmlFormatOptions.Trim);
    }

    /// <summary>
    /// Formats XML applying specific encoding and default formatting options (indent and trim).
    /// </summary>
    /// <param name="value">
    /// XML navigator positioned at the source XML location to format (including children).
    /// </param>
    /// <param name="encoding">
    /// Encoding use for the transformed output. Use the same encoding as the target device to
    /// preserve all content, e.g. HTTP request encoding or database XML encoding (Unicode for
    /// SQL Server).
    /// </param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this XPathNavigator value, Encoding encoding)
    {
        // Call overloaded method with defaults
        return FormatXml(value, encoding, XmlFormatOptions.Indent | XmlFormatOptions.Trim);
    }

    /// <summary>
    /// Formats XML applying specific encoding and formatting options.
    /// </summary>
    /// <param name="value">
    /// XML navigator positioned at the source XML location to format (including children).
    /// </param>
    /// <param name="encoding">
    /// Encoding use for the transformed output. Use the same encoding as the target device to
    /// preserve all content, e.g. HTTP request encoding or database XML encoding (Unicode for
    /// SQL Server).
    /// </param>
    /// <param name="options">Formatting options.</param>
    /// <returns>Formatted XML string.</returns>
    public static string FormatXml(this XPathNavigator value, Encoding encoding, XmlFormatOptions options)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(encoding);

        // Format XML
        try
        {
            // Create XML settings from formatting options New line characters are normalized to
            // CRLF according to the .NET/Windows standard, rather than the XML 1.0
            // specification which favors the older Unix standard of LF only. In this context,
            // of a formatting function which cleans-up XML for display, that is preferred.
            var outputSettings = new XmlWriterSettings {
                OmitXmlDeclaration = (options & XmlFormatOptions.OmitXmlDeclaration) != 0,
                Encoding = encoding,
                Indent = (options & XmlFormatOptions.Indent) != 0,
                ConformanceLevel = ConformanceLevel.Fragment,
                NewLineHandling = NewLineHandling.Replace
            };
            var inputSettings = new XmlReaderSettings {
                IgnoreWhitespace = (options & (XmlFormatOptions.Trim | XmlFormatOptions.Indent)) != 0,
                IgnoreComments = (options & XmlFormatOptions.OmitComments) != 0,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            // Decide which transform to use for best performance
            var transform = (options & XmlFormatOptions.OmitNamespaces) != 0
                ? FormatXmlAnyTransform
                : (options & XmlFormatOptions.Trim) != 0 ? FormatXmlTrimTransform : FormatXmlCopyTransform;

            // Apply transform and switch to Unicode encoding
            var buffer = new StringBuilder();
            using (var reader = XmlReader.Create(value.ReadSubtree(), inputSettings))
            using (var writer = XmlWriter.Create(buffer, outputSettings))
            {
                if ((options & XmlFormatOptions.OmitXmlDeclaration) == 0)
                {
                    // Add XML declaration manually when required because it is always removed
                    // when we support fragments
                    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + encoding.BodyName + "\"");
                }
                transform.Transform(reader, null, writer);
            }

            // Return result
            return buffer.ToString();
        }
        catch
        {
            // Ignore error when option specified (return original value)
            if ((options | XmlFormatOptions.IgnoreErrors) != 0)
                return value.OuterXml;

            // Otherwise throw error
            throw;
        }
    }

    /// <summary>
    /// Generates a unique prefix in the format "ns#" which does not already exist in the
    /// namespace table.
    /// </summary>
    /// <param name="xmlns">XML namespace to generate a new prefix for.</param>
    public static string GeneratePrefix(this XmlSerializerNamespaces xmlns)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xmlns);

        // Increment prefix until unique
        var index = 1;
        var existing = xmlns.ToArray();
        while (true)
        {
            // Generate prefix
            var prefix = "ns" + index++;

            // Check if exists
            foreach (var qname in existing)
            {
                if (qname.Name == prefix)
                {
                    // Exists, try next...
                    continue;
                }
            }

            // Doesn't exist, return new unique prefix
            return prefix;
        }
    }

    /// <summary>
    /// Returns the outer XML of an <see cref="IXPathNavigable"/> document/fragment.
    /// </summary>
    /// <param name="xml">XML document/fragment.</param>
    /// <returns>XML string or null when the source is null or empty.</returns>
    public static string? GetInnerXml(this IXPathNavigable xml)
    {
        if (xml is null) return null;
        var navigator = xml.CreateNavigator();
        return navigator?.InnerXml;
    }

    /// <summary>
    /// Gets a unique list of all prefixes and their namespaces from a
    /// <see cref="XmlSchemaSet"/>.
    /// </summary>
    public static Dictionary<string, string> GetNamespaces(this XmlSchemaSet schemas)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(schemas);

        // Get namespaces...
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
    /// Returns the outer XML of an <see cref="IXPathNavigable"/> document/fragment.
    /// </summary>
    /// <param name="xml">XML document/fragment.</param>
    /// <returns>XML string or null when the source is null or empty.</returns>
    public static string? GetOuterXml(this IXPathNavigable xml)
    {
        if (xml is null) return null;
        var navigator = xml.CreateNavigator();
        return navigator?.OuterXml;
    }

    /// <summary>
    /// Gets <see cref="XmlReaderSettings"/> configured to validate the specified schemata and
    /// throw exceptions when warnings occur.
    /// </summary>
    /// <param name="schemas">Schemata to validate.</param>
    /// <returns>
    /// Settings to specify in the <see cref="XmlReader"/> to cause validation to occur.
    /// </returns>
    public static XmlReaderSettings GetReaderValidationSettings(XmlSchemaSet schemas)
    {
        // Create reader settings with validation enabled
        var settings = new XmlReaderSettings {
            // Secure default to avoid CA3075 warning.
            DtdProcessing = DtdProcessing.Prohibit,

            // Validation settings.
            ValidationType = ValidationType.Schema,
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings |
                              XmlSchemaValidationFlags.AllowXmlAttributes |
                              XmlSchemaValidationFlags.ProcessIdentityConstraints
        };

        // Add schema to validate
        settings.Schemas.Add(schemas);

        // Register validation failure event handler
        settings.ValidationEventHandler += (sender, arguments) =>
        {
            // Throw a descriptive exception when validation errors occur
            var lineInfo = (IXmlLineInfo)sender!;
            throw new FormatException(string.Format(CultureInfo.CurrentCulture,
                Resources.XmlValdiationError, lineInfo.LineNumber, lineInfo.LinePosition,
                arguments.Exception.Message), arguments.Exception);
        };

        // Return settings
        return settings;
    }

    /// <summary>
    /// Gets the XML type of a .NET type, first returning the built-in type mappings (see
    /// <see href="https://msdn.microsoft.com/en-us/library/xa669bew%28v=vs.110%29.aspx"/>),
    /// then calling <see cref="GetXmlTypeDeclaration(Type)"/> to resolve any custom type
    /// declarations.
    /// </summary>
    /// <param name="type">The .NET type to return the XML type for.</param>
    /// <returns>
    /// Fully qualified XML type, or null when no appropriate mapping and .NET generates an
    /// anonymous complex type.
    /// </returns>
    public static XmlQualifiedName? GetXmlType(this Type type)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Return mappings for system types
        if (type == typeof(bool)) return new XmlQualifiedName("boolean", XmlSchema.Namespace);
        if (type == typeof(byte)) return new XmlQualifiedName("unsignedByte", XmlSchema.Namespace);
        if (type == typeof(byte[])) return new XmlQualifiedName("base64Binary", XmlSchema.Namespace);
        if (type == typeof(DateTime)) return new XmlQualifiedName("dateTime", XmlSchema.Namespace);
        if (type == typeof(decimal)) return new XmlQualifiedName("decimal", XmlSchema.Namespace);
        if (type == typeof(double)) return new XmlQualifiedName("double", XmlSchema.Namespace);
        if (type == typeof(short)) return new XmlQualifiedName("short", XmlSchema.Namespace);
        if (type == typeof(int)) return new XmlQualifiedName("int", XmlSchema.Namespace);
        if (type == typeof(long)) return new XmlQualifiedName("long", XmlSchema.Namespace);
        if (type == typeof(sbyte)) return new XmlQualifiedName("byte", XmlSchema.Namespace);
        if (type == typeof(float)) return new XmlQualifiedName("float", XmlSchema.Namespace);
        if (type == typeof(string)) return new XmlQualifiedName("string", XmlSchema.Namespace);
        if (type == typeof(string[])) return null;    // No appropriate mapping and .NET generates an anonymous complex type
        if (type == typeof(TimeSpan)) return new XmlQualifiedName("duration", XmlSchema.Namespace);
        if (type == typeof(ushort)) return new XmlQualifiedName("unsignedShort", XmlSchema.Namespace);
        if (type == typeof(uint)) return new XmlQualifiedName("unsignedInt", XmlSchema.Namespace);
        if (type == typeof(ulong)) return new XmlQualifiedName("unsignedLong", XmlSchema.Namespace);
        if (type == typeof(Uri)) return new XmlQualifiedName("anyURI", XmlSchema.Namespace);
        if (type == typeof(XmlQualifiedName)) return new XmlQualifiedName("QName", XmlSchema.Namespace);

        // Look-up custom type declaration
        return GetXmlTypeDeclaration(type, true);
    }

    /// <summary>
    /// Looks-up the XML type name declared on a type (excluding base classes).
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <returns>Fully qualified XML type or null when not declared.</returns>
    /// <remarks>
    /// Searches the <see cref="XmlTypeAttribute"/>, <see cref="XmlRootAttribute"/> then
    /// <see cref="XmlSchemaProviderAttribute"/>. For the XML schema provider, the method is
    /// called to retrieve the type.
    /// </remarks>
    public static XmlQualifiedName? GetXmlTypeDeclaration(this Type type)
    {
        // Call overloaded method
        return GetXmlTypeDeclaration(type, false);
    }

    /// <summary>
    /// Looks-up the XML type name declared on a type, optionally including base classes.
    /// </summary>
    /// <param name="type">Type to examine.</param>
    /// <param name="inherit">
    /// Optionally include base class XML declarations. Use carefully as XML types should be
    /// unique.
    /// </param>
    /// <returns>Fully qualified XML type or null when not declared.</returns>
    /// <remarks>
    /// Searches the <see cref="XmlTypeAttribute"/>, <see cref="XmlSchemaProviderAttribute"/>
    /// then <see cref="XmlRootAttribute"/>. For the XML schema provider, the method is called
    /// to retrieve the type.
    /// </remarks>
    public static XmlQualifiedName? GetXmlTypeDeclaration(this Type type, bool inherit)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(type);

        // Lookup XmlType attribute
        var xmlTypeAttribute = type.GetAttribute<XmlTypeAttribute>(inherit);
        if (xmlTypeAttribute != null)
        {
            // Found, return XML type
            return new XmlQualifiedName(xmlTypeAttribute.TypeName, xmlTypeAttribute.Namespace);
        }

        // Not found, try XmlSchemaProvider...
        var xmlSchemaProviderAttribute = type.GetAttribute<XmlSchemaProviderAttribute>(inherit);
        if (xmlSchemaProviderAttribute != null)
        {
            // Found, call method to get type then return
            var schemasParameter = new XmlSchemaSet();
            return (XmlQualifiedName)type.InvokeMember(xmlSchemaProviderAttribute.MethodName!,
                BindingFlags.InvokeMethod, null, null, [schemasParameter],
                CultureInfo.InvariantCulture)!;
        }

        // Not found, try XmlRoot attribute...
        var xmlRootAttribute = type.GetAttribute<XmlRootAttribute>(inherit);
        if (xmlRootAttribute != null && !string.IsNullOrWhiteSpace(xmlRootAttribute.DataType))
        {
            // Found, return XML type
            return new XmlQualifiedName(xmlRootAttribute.DataType, xmlRootAttribute.Namespace);
        }

        // Not found
        return null;
    }

    /// <summary>
    /// Checks whether a character is valid according to the XML v1.0 standard, e.g. most
    /// control characters are not allowed.
    /// </summary>
    /// <param name="value">Character to check.</param>
    /// <returns>True if the character is allowed else false.</returns>
    /// <remarks>
    /// This method has been superseded by the new <see cref="XmlConvert.IsXmlChar(char)"/>
    /// method so will be removed in a future build.
    /// </remarks>
    [Obsolete("Use the new system XmlConvert.IsValidXml(char) method.")]
    public static bool IsValidInXml(this char value)
    {
        return XmlConvert.IsXmlChar(value);
    }

    /// <summary>
    /// Loads an XSLT transform from an XML string.
    /// </summary>
    /// <param name="transform">
    /// <see cref="XslCompiledTransform"/> to which this extension applies.
    /// </param>
    /// <param name="xml">String containing the XML to load.</param>
    public static void LoadXml(this XslCompiledTransform transform, string xml)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(transform);

        // Read XML into transform.
        using var xmlReader = XmlReader.Create(new StringReader(xml));
        transform.Load(xmlReader);
    }

    /// <summary>
    /// Looks-up the namespace of a prefix if it exists.
    /// </summary>
    /// <param name="xmlns">Namespace collection to search.</param>
    /// <param name="prefix">Prefix to find.</param>
    /// <returns>Namespace or null when not found.</returns>
    public static string? LookupNamespace(this XmlSerializerNamespaces xmlns, string? prefix)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xmlns);
        if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentNullException(nameof(prefix));

        // Search all entries
        foreach (var qname in xmlns.ToArray())
        {
            if (qname.Name == prefix)
            {
                // Return namespace when found
                return qname.Namespace;
            }
        }

        // Return null when not found
        return null;
    }

    /// <summary>
    /// Looks-up the prefix of a namespace if it exists.
    /// </summary>
    /// <param name="xmlns">Namespace collection to search.</param>
    /// <param name="namespace">Namespace to find.</param>
    /// <returns>Prefix or null when not found.</returns>
    public static string? LookupPrefix(this XmlSerializerNamespaces xmlns, string @namespace)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xmlns);
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));

        // Search all entries
        foreach (var qname in xmlns.ToArray())
        {
            if (qname.Namespace == @namespace)
            {
                // Return prefix when found
                return qname.Name;
            }
        }

        // Return null when not found
        return null;
    }

    /// <summary>
    /// Adds the schemata if they are not already present.
    /// </summary>
    public static void Merge(this XmlSchemaSet targetSchemas, XmlSchemaSet schemas)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(targetSchemas);
        ArgumentNullException.ThrowIfNull(schemas);

        // Add schemata which do not already exist...
        foreach (XmlSchema schema in schemas.Schemas())
        {
            if (!targetSchemas.Contains(schema.TargetNamespace))
                _ = targetSchemas.Add(schema);
        }
    }

    /// <summary>
    /// Merges two <see cref="XmlSerializerNamespaces"/>.
    /// </summary>
    public static void Merge(this XmlSerializerNamespaces target, XmlSerializerNamespaces source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        // Add source namespaces to target
        foreach (var qname in source.ToArray())
            target.Add(qname.Name, qname.Namespace);
    }

    /// <summary>
    /// Overload of <see cref="XmlReader.GetAttribute(string)"/> which gets an attribute with
    /// conversion to a desired return type.
    /// </summary>
    /// <typeparam name="T">Type to return.</typeparam>
    /// <param name="reader">Extension target.</param>
    /// <param name="name">Attribute name.</param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ConvertValue(string, Type)"/> for details about how each type is handled.
    /// </remarks>
    public static T ReadAttribute<T>(this XmlReader reader, string name)
    {
        // Call overloaded method
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        return (T)ReadAttributeAs(reader, name, typeof(T));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
    }

    /// <summary>
    /// Overload of the
    /// <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> which behaves
    /// similarly to the <see cref="XmlReader.ReadElementString()"/> method. Required because
    /// the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> is not
    /// robust enough because it does not move to the next element before reading, so fails if
    /// any other content (whitespace, comments, etc...) appears at the current position, just
    /// before the next element to read.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadElement<T>(this XmlReader reader)
    {
        return ReadElement<T>(reader, null, null, null);
    }

    /// <summary>
    /// Overload of the
    /// <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> which behaves
    /// similarly to the <see cref="XmlReader.ReadElementString()"/> method. Required because
    /// the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> is not
    /// robust enough because it does not move to the next element before reading, so fails if
    /// any other content (whitespace, comments, etc...) appears at the current position, just
    /// before the next element to read.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="localName">
    /// Local name to pass to the <see cref="XmlReader.ReadStartElement(string)"/> method when
    /// reading.
    /// </param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadElement<T>(this XmlReader reader, string localName)
    {
        return ReadElement<T>(reader, localName, null, null);
    }

    /// <summary>
    /// Overload of the
    /// <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> which behaves
    /// similarly to the <see cref="XmlReader.ReadElementString()"/> method. Required because
    /// the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> is not
    /// robust enough because it does not move to the next element before reading, so fails if
    /// any other content (whitespace, comments, etc...) appears at the current position, just
    /// before the next element to read.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="localName">
    /// Local name to pass to the <see cref="XmlReader.ReadStartElement(string, string)"/>
    /// method when reading.
    /// </param>
    /// <param name="namespace">
    /// Namespace to pass to the <see cref="XmlReader.ReadStartElement(string, string)"/> method
    /// when reading.
    /// </param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadElement<T>(this XmlReader reader, string localName, string @namespace)
    {
        return ReadElement<T>(reader, localName, @namespace, null);
    }

    /// <summary>
    /// Overload of the
    /// <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> which behaves
    /// similarly to the <see cref="XmlReader.ReadElementString()"/> method. Required because
    /// the <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/> is not
    /// robust enough because it does not move to the next element before reading, so fails if
    /// any other content (whitespace, comments, etc...) appears at the current position, just
    /// before the next element to read.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="localName">
    /// Local name to pass to the <see cref="XmlReader.ReadStartElement(string, string)"/>
    /// method when reading.
    /// </param>
    /// <param name="namespace">
    /// Namespace to pass to the <see cref="XmlReader.ReadStartElement(string, string)"/> method
    /// when reading.
    /// </param>
    /// <param name="namespaceResolver">
    /// See <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/>.
    /// </param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadElement<T>(this XmlReader reader, string? localName, string? @namespace, IXmlNamespaceResolver? namespaceResolver)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(reader);

        // Read element start (moving past any other non-element content) using the appropriate
        // overload depending on parameters passed
        var exists = @namespace is not null
            ? reader.IsStartElement(localName ?? throw new ArgumentNullException(nameof(localName)), @namespace)
            : localName != null ? reader.IsStartElement(localName) : reader.IsStartElement();
        if (!exists)
            throw new InvalidOperationException();

        // Read start element
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();

        // Return null or default value when empty
        if (empty)
            return default!;

        // Read element
        var value = reader.ReadValue<T>(namespaceResolver);
        reader.ReadEndElement();

        // Return result
        return value;
    }

    /// <summary>
    /// Overload of the <see cref="XmlReader.ReadElementContentAsBase64"/> which reads the
    /// entire stream. Normally you have to specify a fixed buffer and call back the method
    /// until all data is read.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <returns>Byte array containing all of the data available.</returns>
    public static byte[] ReadElementAsBase64(this XmlReader reader)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(reader);

        // Read and return whole value as byte array
        const int chunkSize = 512;
        var buffer = new byte[chunkSize];
        var position = 0;
        int bytesRead;
        do
        {
            // Read chunk of data
            bytesRead = reader.ReadContentAsBase64(buffer, position, chunkSize);
            if (bytesRead <= 0)
                continue;

            // Move to next position
            position += bytesRead;

            // Extend or shrink buffer
            if (bytesRead == chunkSize)
            {
                // Extend buffer for next chunk...
                Array.Resize(ref buffer, buffer.Length + chunkSize);
            }
            else
            {
                // Shrink buffer at end when bytes read smaller than chunk size
                var remainder = chunkSize - bytesRead;
                if (remainder > 0)
                    Array.Resize(ref buffer, buffer.Length - remainder);
            }
        }
        while (bytesRead > 0);

        // Return data read
        return buffer;
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which reads a value with conversion to a desired return
    /// type.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadValue<T>(this XmlReader reader)
    {
        // Call overloaded method
        return (T)ReadValueAs(reader, typeof(T), null);
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which reads a value with conversion to a desired return
    /// type.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="namespaceResolver">
    /// See <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/>.
    /// </param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static T ReadValue<T>(this XmlReader reader, IXmlNamespaceResolver? namespaceResolver)
    {
        // Call overloaded method
        return (T)ReadValueAs(reader, typeof(T), namespaceResolver);
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which reads a value with conversion to a desired return
    /// type.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="returnType">Return type.</param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ReadValueAs(XmlReader,Type,IXmlNamespaceResolver)"/> for details about
    /// how each type is handled.
    /// </remarks>
    public static object ReadValue(this XmlReader reader, Type returnType)
    {
        // Call overloaded method
        return ReadValueAs(reader, returnType, null);
    }

    /// <summary>
    /// Removes any invalid XML characters from a string.
    /// </summary>
    /// <param name="value">XML string.</param>
    /// <returns>Valid XML string.</returns>
    public static string StripInvalidXmlChars(this string value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(value);

        // Return string with invalid characters removed
        return new string([.. value.Where(XmlConvert.IsXmlChar)]);
    }

    /// <summary>
    /// Executes an XSL transformation on an XML string.
    /// </summary>
    /// <param name="xml">XML to transform.</param>
    /// <param name="xslt">Transform XSLT.</param>
    /// <returns>XML string with the result of the transformation.</returns>
    public static string Transform(string xml, string xslt)
    {
        // Validate
        ArgumentNullException.ThrowIfNullOrWhiteSpace(xml);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(xslt);

        // Load transform
        var transform = new XslCompiledTransform();
        transform.LoadXml(xslt);

        // Transform XML
        var buffer = new StringBuilder();
        using var reader = xml.CreateXmlReader();
        using var writer = XmlWriter.Create(buffer);
        transform.Transform(reader, null, writer);

        // Return result
        return buffer.ToString();
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which gets an attribute with conversion.
    /// </summary>
    /// <typeparam name="T">Type to write.</typeparam>
    /// <param name="writer">Extension target.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Value to write.</param>
    /// <remarks>
    /// See <see cref="ConvertValueToString(object)"/> for details about how each type is
    /// handled.
    /// </remarks>
    public static void WriteAttribute<T>(this XmlWriter writer, string name, T value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(writer);
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        if (value is null) throw new ArgumentNullException(nameof(value));

        // Convert value
        var xmlValue = ConvertValueToString(value);

        // Write attribute
        writer.WriteAttributeString(name, xmlValue);
    }

    /// <summary>
    /// Overload of the <see cref="XmlWriter.WriteValue(object)"/> which behaves similarly to
    /// the <see cref="XmlWriter.WriteElementString(string, string)"/> method and uses the
    /// <see cref="WriteValueAs"/> alternate method. Helps reduce the amount of code necessary
    /// to write element values other than string types.
    /// </summary>
    /// <param name="writer">Extension target.</param>
    /// <param name="localName">Element name to write.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteElement(this XmlWriter writer, string localName, object value)
    {
        // Call overloaded method
        WriteElement(writer, localName, null, value);
    }

    /// <summary>
    /// Overload of the <see cref="XmlWriter.WriteValue(object)"/> which behaves similarly to
    /// the <see cref="XmlWriter.WriteElementString(string, string, string)"/> method and uses
    /// the <see cref="WriteValueAs"/> alternate method. Helps reduce the amount of code
    /// necessary to write element values other than string types.
    /// </summary>
    /// <param name="writer">Extension target.</param>
    /// <param name="localName">Element name to write.</param>
    /// <param name="namespace">Optional element namespace to write.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteElement(this XmlWriter writer, string localName, string? @namespace, object value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(localName);

        // Write element start (moving past any other non-element content) using the appropriate
        // overload depending on parameters passed
        if (@namespace is null)
            writer.WriteStartElement(localName);
        else
            writer.WriteStartElement(localName, @namespace);

        // Write target type
        writer.WriteValueAs(value);

        // Write element end
        writer.WriteEndElement();
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which gets an attribute with conversion to a desired
    /// return type.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="returnType">Type to return.</param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// See <see cref="ConvertValue(string, Type)"/> for details about how each type is handled.
    /// </remarks>
    private static object? ReadAttributeAs(this XmlReader reader, string name, Type returnType)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(reader);

        // Read attribute value
        var value = reader.GetAttribute(name);

        // Return default value when value is null or empty
        if (string.IsNullOrEmpty(value))
            return returnType.GetDefaultValue();

        // Convert and return
        return ConvertValue(value, returnType);
    }

    /// <summary>
    /// <see cref="XmlReader"/> method which reads a value with conversion to a desired return
    /// type.
    /// </summary>
    /// <param name="reader">Extension target.</param>
    /// <param name="returnType">Return type.</param>
    /// <param name="namespaceResolver">
    /// See <see cref="XmlReader.ReadElementContentAs(Type, IXmlNamespaceResolver)"/>.
    /// </param>
    /// <returns>Value or null/default when empty.</returns>
    /// <remarks>
    /// The following type enhancements are included:
    /// * Enumerations - read as string then converted using
    ///   <see cref="Enum.Parse(Type, string)"/>.
    /// * System.Boolean - read using <see cref="XmlReader.ReadContentAsBoolean"/>.
    /// * System.Byte - read as string then converted using
    ///   <see cref="XmlConvert.ToByte(string)"/>. Normally this is not supported.
    /// * System.Byte[] - read using <see cref="XmlReader.ReadContentAsBase64"/>.
    /// * System.Char - read as string then converted using
    ///   <see cref="XmlConvert.ToChar(string)"/>. Normally this is not supported.
    /// * System.DateTime - read using <see cref="XmlReader.ReadContentAsDateTime"/>.
    /// * System.DateTimeOffset - read as a string then converted using
    ///   <see cref="XmlConvert.ToDateTimeOffset(string)"/>. Normally this is not supported.
    /// * System.Decimal - read using <see cref="XmlReader.ReadContentAsDecimal"/>.
    /// * System.Double - read using <see cref="XmlReader.ReadContentAsDouble"/>.
    /// * System.Guid - read as string then converted using
    ///   <see cref="XmlConvert.ToGuid(string)"/>. Normally this is not supported.
    /// * System.Int16 - read as string then converted using
    ///   <see cref="XmlConvert.ToInt16(string)"/>. Normally this is not supported.
    /// * System.Int32 - read using <see cref="XmlReader.ReadContentAsInt"/>.
    /// * System.Int64 - read using <see cref="XmlReader.ReadContentAsLong"/>.
    /// * System.SByte - read as string then converted using
    ///   <see cref="XmlConvert.ToSByte(string)"/>. Normally this is not supported.
    /// * System.Single - read using <see cref="XmlReader.ReadContentAsFloat"/>.
    /// * System.String - read using <see cref="XmlReader.ReadContentAsString"/>.
    /// * System.TimeSpan - read as string then converted using
    ///   <see cref="XmlConvert.ToTimeSpan(string)"/>. Normally this is not supported.
    /// * System.UInt16 - read as string then converted using
    ///   <see cref="XmlConvert.ToUInt16(string)"/>. Normally this is not supported.
    /// * System.UInt32 - read as string then converted using
    ///   <see cref="XmlConvert.ToUInt32(string)"/>. Normally this is not supported.
    /// * System.UInt64 - read as string then converted using
    ///   <see cref="XmlConvert.ToUInt64(string)"/>. Normally this is not supported. All other
    /// types read using <see cref="XmlReader.ReadContentAs"/>.
    /// </remarks>
    private static object ReadValueAs(this XmlReader reader, Type returnType, IXmlNamespaceResolver? namespaceResolver)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(returnType);

        // Read types which support direct conversion
        if (returnType.IsEnum) return Enum.Parse(returnType, reader.ReadContentAsString().Trim());
        if (returnType == typeof(bool)) return reader.ReadContentAsBoolean();
        if (returnType == typeof(byte)) return XmlConvert.ToByte(reader.ReadContentAsString().Trim());
        if (returnType == typeof(byte[])) return reader.ReadElementAsBase64();
        if (returnType == typeof(char)) return XmlConvert.ToChar(reader.ReadContentAsString().Trim());
        if (returnType == typeof(DateTime)) return reader.ReadElementContentAsDateTime();
        if (returnType == typeof(DateTimeOffset)) return XmlConvert.ToDateTimeOffset(reader.ReadContentAsString().Trim());
        if (returnType == typeof(decimal)) return XmlConvert.ToDecimal(reader.ReadContentAsString().Trim());
        if (returnType == typeof(double)) return XmlConvert.ToDouble(reader.ReadContentAsString().Trim());
        if (returnType == typeof(Guid)) return XmlConvert.ToGuid(reader.ReadContentAsString().Trim());
        if (returnType == typeof(short)) return XmlConvert.ToInt16(reader.ReadContentAsString().Trim());
        if (returnType == typeof(int)) return XmlConvert.ToInt32(reader.ReadContentAsString().Trim());
        if (returnType == typeof(long)) return XmlConvert.ToInt64(reader.ReadContentAsString().Trim());
        if (returnType == typeof(sbyte)) return XmlConvert.ToSByte(reader.ReadContentAsString().Trim());
        if (returnType == typeof(float)) return XmlConvert.ToSingle(reader.ReadContentAsString().Trim());
        if (returnType == typeof(string)) return reader.ReadContentAsString().Trim();
        if (returnType == typeof(TimeSpan)) return XmlConvert.ToTimeSpan(reader.ReadContentAsString().Trim());
        if (returnType == typeof(ushort)) return XmlConvert.ToUInt16(reader.ReadContentAsString().Trim());
        if (returnType == typeof(uint)) return XmlConvert.ToUInt32(reader.ReadContentAsString().Trim());
        if (returnType == typeof(ulong)) return XmlConvert.ToUInt64(reader.ReadContentAsString().Trim());

        // Try to read other types generically
        return reader.ReadContentAs(returnType, namespaceResolver);
    }

    /// <summary>
    /// Overload of <see cref="XmlWriter.WriteValue(object)"/> which overcomes limitations of
    /// the base method by selecting the best method to call based on the type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following type enhancements are included:
    /// * Enumerations - converted with <see cref="Enum.GetName(Type, object)"/> then written as
    /// a string.
    /// * System.Boolean - written using <see cref="XmlWriter.WriteValue(bool)"/>.
    /// * System.Byte - converted with <see cref="XmlConvert.ToString(byte)"/> then written as a
    /// string.
    /// * System.Byte[] - written using <see cref="XmlWriter.WriteBase64"/>.
    /// * System.Char - converted with <see cref="XmlConvert.ToString(char)"/> then written as a
    /// string.
    /// * System.DateTime - written using <see cref="XmlWriter.WriteValue(DateTime)"/>.
    /// * System.DateTimeOffset - converted with
    ///   <see cref="XmlConvert.ToString(DateTimeOffset)"/> then written as a string.
    /// * System.DBNull - nothing is written.
    /// * System.Decimal - written using <see cref="XmlWriter.WriteValue(decimal)"/>.
    /// * System.Double - written using <see cref="XmlWriter.WriteValue(double)"/>.
    /// * System.Guid - converted with <see cref="XmlConvert.ToString(Guid)"/> then written as a
    /// string.
    /// * System.Int16 - converted with <see cref="XmlConvert.ToString(short)"/> then written as
    /// a string.
    /// * System.Int32 - written using <see cref="XmlWriter.WriteValue(int)"/>.
    /// * System.Int64 - written using <see cref="XmlWriter.WriteValue(long)"/>.
    /// * System.String - written using <see cref="XmlWriter.WriteString"/>.
    /// * System.SByte - converted with <see cref="XmlConvert.ToString(sbyte)"/> then written as
    /// a string.
    /// * System.Single - written using <see cref="XmlWriter.WriteValue(float)"/>.
    /// * System.TimeSpan - converted with <see cref="XmlConvert.ToString(TimeSpan)"/> then
    /// written as a string.
    /// * System.UInt16 - converted with <see cref="XmlConvert.ToString(ushort)"/> then written
    /// as a string.
    /// * System.UInt32 - converted with <see cref="XmlConvert.ToString(uint)"/> then written as
    /// a string.
    /// * System.UInt64 - converted with <see cref="XmlConvert.ToString(ulong)"/> then written
    /// as a string. All other types written with <see cref="XmlWriter.WriteValue(object)"/>.
    /// </para>
    /// <para>
    /// Generics are not used because they are not needed (the type can be inferred directly
    /// from the value). If they were used there is a danger that the base "System.Object" type
    /// would be used where the type is not strictly cast, e.g. enumerating an item array of a
    /// data set. That would cause values to be written incorrectly, without the type specific
    /// handling as typeof(T) would return object not the same as value.GetType() which is
    /// better.
    /// </para>
    /// </remarks>
    private static void WriteValueAs(this XmlWriter writer, object value)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(writer);

        // Do nothing when target type is null
        if (value is null)
            return;

        // Write according to type
        var valueType = value.GetType();
        if (valueType.IsEnum)
        {
            // Write enumerations as strings
            writer.WriteString(Enum.GetName(valueType, value));
            return;
        }
        switch (valueType.FullName)
        {
            case "System.Boolean":
                writer.WriteValue((bool)value);
                break;

            case "System.Byte":
                writer.WriteValue(XmlConvert.ToString((byte)value));
                break;

            case "System.Byte[]":
                var buffer = (byte[])value;
                writer.WriteBase64(buffer, 0, buffer.Length);
                break;

            case "System.Char":
                writer.WriteValue(XmlConvert.ToString((char)value));
                break;

            case "System.DateTime":
                writer.WriteValue((DateTime)value);
                break;

            case "System.DateTimeOffset":
                writer.WriteValue(XmlConvert.ToString((DateTimeOffset)value));
                break;

            case "System.DBNull":

                // DB null is same as null (nothing)
                break;

            case "System.Decimal":
                writer.WriteValue((decimal)value);
                break;

            case "System.Double":
                writer.WriteValue((double)value);
                break;

            case "System.Guid":
                writer.WriteValue(XmlConvert.ToString((Guid)value));
                break;

            case "System.Int16":
                writer.WriteValue(XmlConvert.ToString((short)value));
                break;

            case "System.Int32":
                writer.WriteValue((int)value);
                break;

            case "System.Int64":
                writer.WriteValue((long)value);
                break;

            case "System.String":
                writer.WriteString((string)value);
                break;

            case "System.SByte":
                writer.WriteValue(XmlConvert.ToString((sbyte)value));
                break;

            case "System.Single":
                writer.WriteValue((float)value);
                break;

            case "System.TimeSpan":
                writer.WriteValue(XmlConvert.ToString((TimeSpan)value));
                break;

            case "System.UInt16":
                writer.WriteValue(XmlConvert.ToString((ushort)value));
                break;

            case "System.UInt32":
                writer.WriteValue(XmlConvert.ToString((uint)value));
                break;

            case "System.UInt64":
                writer.WriteValue(XmlConvert.ToString((ulong)value));
                break;

            default:

                // Write all other values using default method
                writer.WriteValue(value);
                break;
        }
    }
}