using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Collections;

namespace CodeForDotNet.Xml;

/// <summary>
/// Base <see cref="Collection{T}"/> class which adds improved XML serialization including:
/// <list type="bullet">
/// <item>Root element name respected, not hard-coded to type name.</item>
/// <item>
/// Fully qualified default schema including generic item element name and the ability to
/// override it to fit your own schema.
/// </item>
/// <item>
/// Smart namespace management to dramatically reduce output size by avoiding duplicate prefix declarations.
/// </item>
/// </list>
/// </summary>
/// <remarks>
/// <para>
/// To customize XML serialization this class must be inherited (why constructors are protected
/// not public). It is not be possible to allow dynamic configuration because the name and
/// namespace of all types must be declaratively set by their container or root attribute.
/// </para>
/// <para>
/// Inheritors need only derive from this class then apply the <see cref="XmlRootAttribute"/>
/// and <see cref="XmlSchemaProviderAttribute"/> (along with associated method to get the schema
/// type). Optionally the item name and preferred namespace prefix can be overridden to further
/// customize XML output.
/// </para>
/// </remarks>
[Serializable]
[XmlType(XmlTypeNameDefault, Namespace = CodeXsd.XmlNamespace)]
[XmlSchemaProvider(nameof(GetSchema))]
public abstract class XmlCollection<TItem> : Collection<TItem>, IXmlSerializable
{
    /// <summary>
    /// Default value of the <see cref="_xmlItemName"/>.
    /// </summary>
    public const string XmlItemNameDefault = "Item";

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeNameDefault = "XmlCollectionType";

    /// <summary>
    /// XML namespace.
    /// </summary>
    private string _xmlNamespace = CodeXsd.XmlNamespace;

    /// <summary>
    /// XML namespace prefix.
    /// </summary>
    private string _xmlNamespacePrefix = CodeXsd.XmlNamespacePrefixDefault;

    /// <summary>
    /// XML types supported by serialization.
    /// </summary>
    /// <remarks>
    /// Passed to the <see cref="XmlSerializer"/> in order to support serialization of derived
    /// classes without hard-coded limitations on their base class, i.e. avoids using the
    /// <see cref="XmlIncludeAttribute"/> and provides a solution when you cannot use it.
    /// </remarks>
    private Collection<Type> _xmlTypes = [];

    /// <summary>
    /// Cached XML serializer for the items in this collection.
    /// </summary>
    [NonSerialized]
    private XmlSerializer? _xmlItemSerializer;

    /// <summary>
    /// XML element name to use for each item during serialization.
    /// </summary>
    private string _xmlItemName = XmlItemNameDefault;

    /// <summary>
    /// XML type of the of the items stored in this collection.
    /// </summary>
    private XmlQualifiedName _xmlItemType = XmlQualifiedName.Empty;

    /// <summary>
    /// Indicates the XSD and XSI schema namespaces should be defined at the earliest possible to avoid
    /// repetition on each member which may require them to specify types.
    /// </summary>
    private bool _xsiNamespaceRequired;

    /// <summary>
    /// Creates an instance based on an existing collection.
    /// </summary>
    protected XmlCollection(string xmlNamespace, string xmlNamespacePrefix, string xmlItemName, bool xsiNamespaceRequired)
    {
        Initialize(xmlNamespace, xmlNamespacePrefix, xmlItemName, xsiNamespaceRequired);
    }

    /// <summary>
    /// Creates an instance based on an existing collection.
    /// </summary>
    protected XmlCollection(string xmlNamespace, string xmlNamespacePrefix, string xmlItemName, bool xsiNamespaceRequired, IList<TItem> collection)
        : base(collection)
    {
        Initialize(xmlNamespace, xmlNamespacePrefix, xmlItemName, xsiNamespaceRequired);
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by serialization API.")]
    protected XmlCollection([NotNull] SerializationInfo info, [NotNull] StreamingContext context)
    {
        // Restore serializable fields.
        _xmlNamespace = info.GetString(nameof(_xmlNamespace))!;
        _xmlNamespacePrefix = info.GetString(nameof(_xmlNamespacePrefix))!;
        _xmlItemName = info.GetString(nameof(_xmlItemName))!;
        _xmlTypes = (Collection<Type>)info.GetValue(nameof(_xmlTypes), typeof(Collection<Type>))!;
        foreach (var item in (TItem[])info.GetValue(nameof(Items), typeof(TItem[]))!)
            Add(item);

        // Restore non-serializable fields.
        CacheXmlSerializer();
    }

    /// <summary>
    /// XML name table for serialization.
    /// </summary>
    [XmlNamespaceDeclarations]
    [field: NonSerialized]
    public XmlSerializerNamespaces XmlNamespaces { get; set; } = new XmlSerializerNamespaces();

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(XmlCollection<TItem> left, XmlCollection<TItem> right)
    {
        return left is not null ? !left.Equals(right) : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(XmlCollection<TItem> left, XmlCollection<TItem> right)
    {
        return left is not null
            ? left.Equals(right)
            : right is null;
    }

    /// <summary>
    /// Gets the XML schema and type name for serialization.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Required by XML serializer.")]
    public static XmlQualifiedName GetSchema(XmlSchemaSet schemas)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(schemas);

        // Add schemata and return qualified name.
        schemas.Add(CodeXsd.GetSchema());
        return new XmlQualifiedName(XmlTypeNameDefault, CodeXsd.XmlNamespace);
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override bool Equals(object? other)
    {
        // Check for null and type
        if (other is not XmlCollection<TItem> collection)
            return false;

        // Compare values
        return
            collection._xmlNamespace == _xmlNamespace &&
            collection._xmlNamespacePrefix == _xmlNamespacePrefix &&
            collection._xmlItemName == _xmlItemName &&
            ArrayExtensions.AreEqual(_xmlTypes, collection._xmlTypes) &&
            ArrayExtensions.AreEqual(Items, collection.Items);
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
           (_xmlNamespace?.GetHashCode() ?? 0) ^
           (_xmlNamespacePrefix?.GetHashCode() ?? 0) ^
           (_xmlItemName?.GetHashCode() ?? 0) ^
           ArrayExtensions.GetHashCodeOfItems(_xmlTypes) ^
           ArrayExtensions.GetHashCodeOfItems(Items);
    }

    /// <summary>
    /// Gets serialization data.
    /// </summary>
    [SecurityCritical]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(_xmlNamespace), _xmlNamespace);
        info.AddValue(nameof(_xmlNamespacePrefix), _xmlNamespacePrefix);
        info.AddValue(nameof(_xmlItemName), _xmlItemName);
        info.AddValue(nameof(_xmlTypes), _xmlTypes);
        info.AddValue(nameof(Items), this.ToArray());
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
    /// De-serializes the properties of this object from XML.
    /// </summary>
    public virtual void ReadXml(XmlReader reader)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(reader);

        // Clear any existing items.
        Clear();

        // Get serialization information.
        CacheXmlSerializer();

        // Read collection start element.
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (!empty)
        {
            // Read items...
            while (reader.IsStartElement(_xmlItemName, _xmlNamespace))
                Add((TItem)_xmlItemSerializer!.Deserialize(reader)!);

            // Read end of collection element.
            reader.ReadEndElement();
        }
    }

    /// <summary>
    /// Serializes the properties of this object to XML.
    /// </summary>
    public virtual void WriteXml(XmlWriter writer)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(writer);

        // Get serialization information.
        CacheXmlSerializer();
        var itemType = typeof(TItem);
        var itemNeedsInstanceType = itemType.IsValueType || itemType == typeof(object);

        // Extend writer to control output (workaround standard serializer issues).
        using var injectWriter = new XmlInjectionWriter(writer);

        // Merge any preferred namespaces stored using XML serializer namespaces feature.
        foreach (var serializerNamespace in XmlNamespaces.ToArray())
            _ = injectWriter.AssertPrefix(serializerNamespace.Namespace, serializerNamespace.Name);

        // Declare or suppress XSI and XSD namespaces as necessary.
        if (_xsiNamespaceRequired || itemNeedsInstanceType)
        {
            _ = injectWriter.AssertPrefixWritten(XmlSchema.InstanceNamespace, XmlExtensions.XsiNamespacePrefixDefault);
            _ = injectWriter.AssertPrefixWritten(XmlSchema.Namespace, XmlExtensions.XsdNamespacePrefixDefault);
        }
        else if (!_xsiNamespaceRequired)
        {
            injectWriter.SetPrefix(XmlSchema.InstanceNamespace, "");
            injectWriter.SetPrefix(XmlSchema.Namespace, "");
        }

        // Ensure child element prefixes are declared at top level (so not repeated below).
        var thisPrefix = injectWriter.LookupPrefix(_xmlNamespace);
        if (thisPrefix is null)
            injectWriter.SetPrefix(_xmlNamespace, _xmlNamespacePrefix);
        else if (thisPrefix?.Length > 0)
            _ = injectWriter.AssertPrefixWritten(_xmlNamespace, thisPrefix);

        // Write collection items...
        foreach (var item in Items)
        {
            // Inject or block item XSI type when required.
            if (itemNeedsInstanceType)
                injectWriter.AddXsiType(_xmlItemType!);

            // Write item element.
            injectWriter.RenameElement(thisPrefix, _xmlItemName, _xmlNamespace);
            _xmlItemSerializer!.Serialize(injectWriter, item, XmlNamespaces);
        }
    }

    /// <summary>
    /// Caches XML serialization information for items in this collection.
    /// </summary>
    private void CacheXmlSerializer()
    {
        // Do nothing when already cached.
        if (_xmlItemSerializer != null)
            return;

        // Prepare serializer types.
        var itemType = typeof(TItem);
        var xmlTypes = _xmlTypes.ToArray();

        // Create serializer for item type.
        var itemOverrides = new XmlAttributeOverrides();
        var itemInAnotherNamespace = _xmlItemType != null && _xmlItemType.Namespace != _xmlNamespace;
        if (itemInAnotherNamespace)
        {
            // Ensure serializer uses correct type when not system type.
            if (_xmlItemType!.Namespace != XmlSchema.Namespace)
            {
                itemOverrides.Add(itemType, new XmlAttributes {
                    XmlType = new XmlTypeAttribute(_xmlItemType.Name) { Namespace = _xmlItemType.Namespace }
                });
            }
        }
        var itemSerializerKey = GetType().FullName + "." + _xmlItemName;
        _xmlItemSerializer = XmlSerializerCache.Create(itemType, itemSerializerKey, () =>
        {
            return new XmlSerializer(itemType, itemOverrides, xmlTypes,
                new XmlRootAttribute(_xmlItemName) { Namespace = _xmlNamespace }, _xmlNamespace);
        });
    }

    /// <summary>
    /// Shared initialization.
    /// </summary>
    private void Initialize(string xmlNamespace, string xmlNamespacePrefix, string xmlItemName, bool xsiNamespaceRequired)
    {
        // .NET Types for XML serialization
        _xmlTypes = [];
        var itemType = typeof(TItem);
        _xmlTypes.Add(itemType);
        XmlSerializerCache.RegisterTypes(_xmlTypes);

        // XML element names
        _xmlNamespace = xmlNamespace;
        _xmlNamespacePrefix = xmlNamespacePrefix;
        _xmlItemName = xmlItemName;
        _xsiNamespaceRequired = xsiNamespaceRequired;

        // XML instance types (XSI)
        _xmlItemType = XmlExtensions.GetXmlType(itemType)!;
    }
}
