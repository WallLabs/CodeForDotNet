using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace CodeForDotNet.Xml;

/// <summary>
/// Container for unqualified arbitrary XML data which is serializable as child element of a
/// qualified type, i.e. an "xs:any" container.
/// </summary>
/// <remarks>
/// A default root element is still required when serialized standalone, i.e. as a document.
/// Normally this is a property of another serializable type, in which case the root is set by
/// the XML serialization attributes of the parent property or in the custom serialization code
/// of the parent class when implemented.
/// </remarks>
[Serializable]
[XmlRoot(XmlRootName, Namespace = "")]
[XmlSchemaProvider(nameof(GetSchema))]
public sealed class XmlAnyDocument : ISerializable, IXmlSerializable
{
    /// <summary>
    /// XML namespace.
    /// </summary>
    public const string XmlNamespace = CodeXsd.XmlNamespace;

    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = nameof(XmlAnyDocument);

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = nameof(XmlAnyDocument) + "Type";

    /// <summary>
    /// Field behind the <see cref="Data"/> property.
    /// </summary>
    private readonly XmlDocumentFragment _data;

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public XmlAnyDocument()
    {
        var document = new XmlDocument { XmlResolver = null };
        _data = document.CreateDocumentFragment();
    }

    /// <summary>
    /// Creates an instance with the specified XML content.
    /// </summary>
    public XmlAnyDocument(string xml)
        : this()
    {
        // Validate
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentNullException(nameof(xml));
        }

        // Load XML
        _data.InnerXml = xml;
    }

    /// <summary>
    /// Creates an instance with the specified XML content.
    /// </summary>
    public XmlAnyDocument(TextReader xml)
        : this()
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xml);

        // Load XML
        _data.InnerXml = xml.ReadToEnd();
    }

    /// <summary>
    /// Creates an instance with the specified XML content.
    /// </summary>
    public XmlAnyDocument(Stream xml)
        : this()
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xml);

        // Load XML
        using var reader = new StreamReader(xml);
        _data.InnerXml = reader.ReadToEnd();
    }

    /// <summary>
    /// Creates an instance with the specified XML content.
    /// </summary>
    public XmlAnyDocument(XmlReader xml)
        : this()
    {
        // Validate
        ArgumentNullException.ThrowIfNull(xml);

        // Load XML
        _data.InnerXml = xml.ReadOuterXml();
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    private XmlAnyDocument(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        _data = new XmlDocument { XmlResolver = null /* Prohibit DTD secure default (CA3075). */ }.CreateDocumentFragment();
        var xml = info.GetString(XmlRootName);
        if (!string.IsNullOrEmpty(xml))
        {
            Add(xml, null, null);
        }
    }

    /// <summary>
    /// Indicates whether the XML is currently empty.
    /// </summary>
    public bool IsEmpty => _data is null || _data.ChildNodes.Count == 0;

    /// <summary>
    /// XML data.
    /// </summary>
    public IXPathNavigable Data => _data;

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(XmlAnyDocument left, XmlAnyDocument right)
    {
        return left is not null ? !left.Equals(right) : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(XmlAnyDocument left, XmlAnyDocument right)
    {
        return left is not null
            ? left.Equals(right)
            : right is null;
    }

    /// <summary>
    /// Gets the schema and XML type of this class.
    /// </summary>
    public static XmlQualifiedName GetSchema(XmlSchemaSet schemaSet)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(schemaSet);

        // Add schema and return qualified name.
        schemaSet.Add(CodeXsd.GetSchema());
        return new XmlQualifiedName(XmlTypeName, XmlNamespace);
    }

    /// <summary>
    /// Adds XML to the specified data path.
    /// </summary>
    /// <param name="sourceXml">XML to add at the path.</param>
    /// <param name="sourcePath">Source XPath, or null for root.</param>
    /// <param name="targetPath">Target XPath, or null for root.</param>
    public void Add(string sourceXml, string? sourcePath, string? targetPath)
    {
        // Call overloaded method
        Set(sourceXml.CreateXPathDocument().CreateNavigator(), sourcePath, targetPath, false);
    }

    /// <summary>
    /// Adds XML to the specified data path.
    /// </summary>
    /// <param name="sourceXml">XML to add at the path.</param>
    /// <param name="sourcePath">Source XPath, or null for root.</param>
    /// <param name="targetPath">Target XPath, or null for root.</param>
    public void Add(XPathNavigator sourceXml, string? sourcePath, string? targetPath)
    {
        // Call overloaded method
        Set(sourceXml, sourcePath, targetPath, false);
    }

    /// <summary>
    /// Removing all content.
    /// </summary>
    public void Clear()
    {
        _data.RemoveAll();
    }

    /// <summary>
    /// Deletes XML from the specified path.
    /// </summary>
    /// <param name="path">XPath target path, null for root.</param>
    public void Delete(string? path)
    {
        // Delete entire document when null or root specified
        if (string.IsNullOrEmpty(path) || path == "/")
        {
            _data.RemoveAll();
            return;
        }

        // Select and delete individual nodes when path specified
        foreach (XPathNavigator node in _data.CreateNavigator()!.Select(path))
        {
            node.DeleteSelf();
        }
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override bool Equals(object? other)
    {
        // Check for null and type
        if (other is not XmlAnyDocument xml)
        {
            // Try comparing as string
            return other is string otherString && otherString == ToString();
        }

        // Compare values
        return
            _data != null && xml.Data != null &&
            _data.GetOuterXml() == xml.Data.GetOuterXml();
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return _data?.GetOuterXml()?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Called by the binary serializer to get object data.
    /// </summary>
    /// <remarks>
    /// We need to control serialization because the <see cref="XmlDocumentFragment"/> class is
    /// not serializable.
    /// </remarks>
    [SecurityCritical]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(info);

        // Add data for serialization.
        info.AddValue("Xml", GetRoot().OuterXml);
    }

    /// <summary>
    /// Returns an <see cref="XPathNavigator"/> positioned within the data. The path is not created.
    /// </summary>
    /// <remarks>
    /// Even if this instance was serialized standalone the default "root" element is not part
    /// of the path because it is stripped during de-serialization.
    /// </remarks>
    /// <param name="path">
    /// Optional XPath expression to select the sub-path. Set null to select the root element.
    /// </param>
    /// <returns><see cref="XPathNavigator"/> positioned at the path or null when not found.</returns>
    public XPathNavigator? GetPath(string? path)
    {
        // Call overloaded method
        return GetPath(path, false);
    }

    /// <summary>
    /// Returns an <see cref="XPathNavigator"/> positioned within the data. Parent elements in
    /// the path without filters will be created if the <paramref name="create"/> option is set.
    /// </summary>
    /// <remarks>
    /// Even if this instance was serialized standalone the default "root" element is not part
    /// of the path because it is stripped during de-serialization.
    /// </remarks>
    /// <param name="path">
    /// Optional XPath expression to select the sub-path. Set null to select the root element.
    /// </param>
    /// <param name="create">Create the path if it doesn't exist.</param>
    /// <returns>
    /// <see cref="XPathNavigator"/> positioned at the path or null when not found and
    /// <paramref name="create"/> was not set true.
    /// </returns>
    public XPathNavigator? GetPath(string? path, bool create)
    {
        // Return document root when no path
        if (string.IsNullOrEmpty(path))
            return _data.CreateNavigator();

        // Attempt to get path
        var existing = _data.SelectSingleNode(path);
        if (existing != null)
            return existing.CreateNavigator();

        // Create when missing and option specified, else return not found (null)
        return create ? _data.CreatePath(path) : null;
    }

    /// <summary>
    /// Returns an <see cref="XPathNavigator"/> positioned at the root of the data.
    /// </summary>
    public XPathNavigator GetRoot()
    {
        return _data.CreateNavigator()!;
    }

    /// <summary>
    /// This method is reserved and should not be used. When implementing the IXmlSerializable
    /// interface, you should return null (Nothing in Visual Basic) from this method, and
    /// instead, if specifying a custom schema is required, apply the
    /// <see cref="XmlSchemaProviderAttribute"/> to the class.
    /// </summary>
    /// <returns>
    /// An <see cref="XmlSchema"/> that describes the XML representation of the object that is
    /// produced by the <see cref="IXmlSerializable.WriteXml(XmlWriter)"/> method and consumed
    /// by the <see cref="IXmlSerializable.ReadXml(XmlReader)"/> method.
    /// </returns>
    public XmlSchema? GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Populates this instance during XML de-serialization.
    /// </summary>
    /// <remarks>
    /// The root element is skipped according to normal XML de-serialization behavior because it
    /// is defined by the parent object. When using this method for general purpose population
    /// of XML content, use the <see cref="ReadXml(XmlReader, bool)"/> overload to read the
    /// entire content (including the root element).
    /// </remarks>
    public void ReadXml(XmlReader reader)
    {
        ReadXml(reader, false);
    }

    /// <summary>
    /// Populates the XML content of this instance, optionally skipping the root element.
    /// </summary>
    public void ReadXml(XmlReader reader, bool includeRoot)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(reader);

        // Remove any existing content
        Clear();

        // Ensure we are positioned on a node
        _ = reader.IsStartElement();

        // Skip root element when not requested
        var empty = reader.IsEmptyElement;
        if (!includeRoot)
        {
            reader.ReadStartElement();
        }

        // Read new XML
        Add(new XPathDocument(reader).CreateNavigator(), null, null);

        // Read past root end element when not specified and not empty
        if (!includeRoot && !empty)
            reader.ReadEndElement();
    }

    /// <summary>
    /// Replaces XML at the specified data path.
    /// </summary>
    /// <param name="sourceXml">XML to add at the path.</param>
    /// <param name="sourcePath">Source XPath, or null for root.</param>
    /// <param name="targetPath">Target XPath, or null for root.</param>
    public void Replace(string sourceXml, string? sourcePath, string? targetPath)
    {
        // Validate
        if (string.IsNullOrEmpty(sourceXml))
            throw new ArgumentNullException(nameof(sourceXml));

        // Select source path when specified
        if (!string.IsNullOrEmpty(sourcePath))
            _ = sourceXml.CreateXPathDocument().CreateNavigator().Select(sourcePath);

        // Call overloaded method
        Set(sourceXml.CreateXPathDocument().CreateNavigator(), sourcePath, targetPath, true);
    }

    /// <summary>
    /// Replaces XML at the specified data path.
    /// </summary>
    /// <param name="sourceXml">XML to replace at the path.</param>
    /// <param name="sourcePath">Source XPath, or null for root.</param>
    /// <param name="targetPath">Target XPath, or null for root.</param>
    public void Replace(XPathNavigator sourceXml, string? sourcePath, string? targetPath)
    {
        // Call overloaded method
        Set(sourceXml, sourcePath, targetPath, true);
    }

    /// <summary>
    /// Returns the current XML content as a string.
    /// </summary>
    public override string ToString()
    {
        return _data != null ? _data.OuterXml : "";
    }

    /// <summary>
    /// Writes properties of this object to XML during serialization.
    /// </summary>
    public void WriteXml(XmlWriter writer)
    {
        GetRoot().WriteSubtree(writer);
    }

    /// <summary>
    /// Sets (adds or replaces) XML at the specified data path.
    /// </summary>
    /// <param name="sourceXml">XML to add at the path.</param>
    /// <param name="sourcePath">Source XPath, or null for root.</param>
    /// <param name="targetPath">Target XPath, or null for root.</param>
    /// <param name="overwrite">Set true to delete data first.</param>
    private void Set(XPathNavigator sourceXml, string? sourcePath, string? targetPath, bool overwrite)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(sourceXml);

        // Find or create target path
        var targetXml = GetPath(targetPath, true)!;

        // Remove existing XML when overwrite specified
        if (overwrite)
        {
            if (string.IsNullOrEmpty(targetXml.Name))
                targetXml.InnerXml = "";
            else
                targetXml.ReplaceSelf("<" + targetXml.Name + "/>");
        }

        // Write source XML to target
        if (!string.IsNullOrEmpty(sourcePath) && sourcePath != "/")
        {
            // Select source XML matching path/filters when specified
            var buffer = new StringBuilder();
            using (var writer = XmlWriter.Create(buffer,
                new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Auto }))
            {
                sourceXml.Copy(sourcePath, writer);
            }

            // Write source XML to target
            if (string.IsNullOrEmpty(targetXml.Name))
            {
                targetXml.InnerXml += buffer.ToString();
            }
            else
            {
                targetXml.AppendChild(buffer.ToString());
            }
        }
        else
        {
            // Write entire source XML when no path or root specified
            if (string.IsNullOrEmpty(targetXml.Name))
            {
                targetXml.InnerXml += sourceXml.OuterXml;
            }
            else
            {
                targetXml.AppendChild(sourceXml.ReadSubtree());
            }
        }
    }
}
