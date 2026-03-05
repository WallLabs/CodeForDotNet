using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Collections;
using CodeForDotNet.Properties;

namespace CodeForDotNet.Xml;

/// <summary>
/// <see cref="Dictionary{TKey, TValue}"/> with improved XML serialization defaults (root name
/// not type name), a fully qualified default schema including generic key and value pair
/// element names and the ability to override them to fit your own schema.
/// </summary>
/// <remarks>
/// <para>
/// To customize XML serialization this class must be inherited (why constructors are protected
/// not public). It is not be possible to allow dynamic configuration because the name and
/// namespace of all types must be declaratively set by their container or root attribute.
/// </para>
/// <para>
/// Inheritors need only derive from this class then apply the <see cref="XmlRootAttribute"/>
/// and <see cref="XmlSchemaProviderAttribute"/> (along with associated method to get the schema type).
/// </para>
/// </remarks>
[Serializable]
[XmlType(XmlTypeNameDefault, Namespace = CodeXsd.XmlNamespace)]
[XmlSchemaProvider(nameof(GetSchema))]
[SuppressMessage("Performance", "CA1863:Use 'CompositeFormat'", Justification = "TODO: Upgrade resources.")]
public abstract class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    where TKey : notnull
{
    /// <summary>
    /// Default value of <see cref="_xmlEntryName"/>.
    /// </summary>
    public const string XmlEntryNameDefault = "Entry";

    /// <summary>
    /// Default value of <see cref="_xmlKeyName"/>.
    /// </summary>
    public const string XmlKeyNameDefault = "Key";

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeNameDefault = "XmlDictionaryType";

    /// <summary>
    /// Default value of <see cref="_xmlValueName"/>.
    /// </summary>
    public const string XmlValueNameDefault = "Value";

    /// <summary>
    /// XML types supported by serialization.
    /// </summary>
    /// <remarks>
    /// Passed to the <see cref="XmlSerializer"/> in order to support serialization of derived
    /// classes without hard-coded limitations on their base class, i.e. avoids using the
    /// <see cref="XmlIncludeAttribute"/> and provides a solution when you cannot use it.
    /// </remarks>
    private readonly Collection<Type> _xmlTypes = [];

    /// <summary>
    /// Cached XML serializer for the keys in this dictionary.
    /// </summary>
    [NonSerialized]
    private XmlSerializer? _xmlKeySerializer;

    /// <summary>
    /// Cached XML serializer for the values in this dictionary.
    /// </summary>
    [NonSerialized]
    private XmlSerializer? _xmlValueSerializer;

    /// <summary>
    /// XML namespace.
    /// </summary>
    private string _xmlNamespace = CodeXsd.XmlNamespace;

    /// <summary>
    /// XML namespace prefix.
    /// </summary>
    private string _xmlNamespacePrefix = CodeXsd.XmlNamespacePrefixDefault;

    /// <summary>
    /// Entry XML element name.
    /// </summary>
    private string _xmlEntryName = XmlEntryNameDefault;

    /// <summary>
    /// Key XML element name.
    /// </summary>
    private string _xmlKeyName = XmlKeyNameDefault;

    /// <summary>
    /// Value XML element name.
    /// </summary>
    private string _xmlValueName = XmlValueNameDefault;

    /// <summary>
    /// XML type of the of the keys stored in this dictionary.
    /// </summary>
    private XmlQualifiedName _xmlKeyType = XmlQualifiedName.Empty;

    /// <summary>
    /// XML type of the of the values stored in this dictionary.
    /// </summary>
    private XmlQualifiedName _xmlValueType = XmlQualifiedName.Empty;

    /// <summary>
    /// Indicates the XSD and XSI schema namespaces should be defined at the earliest possible to avoid
    /// repetition on each member which may require them to specify types.
    /// </summary>
    private bool _xsiNamespaceRequired;

    /// <summary>
    /// Creates an empty dictionary.
    /// </summary>
    protected XmlDictionary(string xmlNamespace, string xmlNamespacePrefix,
        string xmlEntryName, string xmlKeyName, string xmlValueName, bool xsiNamespaceRequired)
    {
        Initialize(xmlNamespace, xmlNamespacePrefix, xmlEntryName,
            xmlKeyName, xmlValueName, xsiNamespaceRequired);
    }

    /// <summary>
    /// Creates an instance based on an existing dictionary.
    /// </summary>
    protected XmlDictionary(string xmlNamespace, string xmlNamespacePrefix,
        string xmlEntryName, string xmlKeyName, string xmlValueName, bool xsiNamespaceRequired, IDictionary<TKey, TValue> dictionary)
        : base(dictionary)
    {
        Initialize(xmlNamespace, xmlNamespacePrefix, xmlEntryName,
            xmlKeyName, xmlValueName, xsiNamespaceRequired);
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    protected XmlDictionary(SerializationInfo info, StreamingContext context)
    {
        // Restore serializable fields.
        _xmlNamespace = info.GetString(nameof(_xmlNamespace))!;
        _xmlNamespacePrefix = info.GetString(nameof(_xmlNamespacePrefix))!;
        _xmlEntryName = info.GetString(nameof(_xmlEntryName))!;
        _xmlKeyName = info.GetString(nameof(_xmlKeyName))!;
        _xmlValueName = info.GetString(nameof(_xmlValueName))!;
        _xmlTypes = (Collection<Type>)info.GetValue(nameof(_xmlTypes), typeof(Collection<Type>))!;
        foreach (var entry in (KeyValuePair<TKey, TValue>[])info.GetValue("Entries", typeof(KeyValuePair<TKey, TValue>[]))!)
            Add(entry.Key, entry.Value);

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
    public static bool operator !=(XmlDictionary<TKey, TValue> left, XmlDictionary<TKey, TValue> right)
    {
        return left is not null ? !left.Equals(right) : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(XmlDictionary<TKey, TValue> left, XmlDictionary<TKey, TValue> right)
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
        if (other is not XmlDictionary<TKey, TValue> dictionary)
        {
            return false;
        }

        // Compare values
        return
            dictionary._xmlNamespace == _xmlNamespace &&
            dictionary._xmlNamespacePrefix == _xmlNamespacePrefix &&
            dictionary._xmlEntryName == _xmlEntryName &&
            dictionary._xmlKeyName == _xmlKeyName &&
            dictionary._xmlValueName == _xmlValueName &&
            ArrayExtensions.AreEqual(Keys, dictionary.Keys) &&
            ArrayExtensions.AreEqual(Values, dictionary.Values);
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            (_xmlNamespace?.GetHashCode() ?? 0) ^
            (_xmlNamespacePrefix?.GetHashCode() ?? 0) ^
            (_xmlEntryName?.GetHashCode() ?? 0) ^
            (_xmlKeyName?.GetHashCode() ?? 0) ^
            (_xmlValueName?.GetHashCode() ?? 0) ^
            ArrayExtensions.GetHashCodeOfItems(Keys) ^
            ArrayExtensions.GetHashCodeOfItems(Values);
    }

    /// <summary>
    /// Gets serialization data.
    /// </summary>
    [SecurityCritical]
    [Obsolete("TODO: Re-evaluate built-in serialization support since base class has marked this method obsolete from .NET 8.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(info);

        // Return data to serialize.
        info.AddValue(nameof(_xmlNamespace), _xmlNamespace);
        info.AddValue(nameof(_xmlNamespacePrefix), _xmlNamespacePrefix);
        info.AddValue(nameof(_xmlEntryName), _xmlEntryName);
        info.AddValue(nameof(_xmlKeyName), _xmlKeyName);
        info.AddValue(nameof(_xmlValueName), _xmlValueName);
        info.AddValue(nameof(_xmlKeyType), _xmlKeyType);
        info.AddValue(nameof(_xmlValueType), _xmlValueType);
        info.AddValue(nameof(_xmlTypes), _xmlTypes);
        info.AddValue("Entries", this.ToArray());
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
    public void ReadXml(XmlReader reader)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(reader);

        // Clear any existing entries.
        Clear();

        // Get serialization information.
        CacheXmlSerializer();

        // Read dictionary start element.
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (!empty)
        {
            // Read entries...
            while (reader.IsStartElement(_xmlEntryName, _xmlNamespace))
            {
                // Read past entry start element
                reader.ReadStartElement(_xmlEntryName, _xmlNamespace);

                // Read key
                if (!reader.IsStartElement(_xmlKeyName, _xmlNamespace))
                {
                    // Invalid content - throw error with detail
                    throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.XmlDictionaryMissingKey,
                        GetType().FullName, _xmlKeyType));
                }
                var key = (TKey)_xmlKeySerializer!.Deserialize(reader)!;

                // Read value
                if (!reader.IsStartElement(_xmlValueName, _xmlNamespace))
                {
                    // Invalid content - throw error with detail
                    throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.XmlDictionaryMissingValue,
                        GetType().FullName, _xmlValueType));
                }
                var value = (TValue)_xmlValueSerializer!.Deserialize(reader)!;

                // Add to results
                Add(key, value);

                // Read end of entry element
                reader.ReadEndElement();
            }

            // Read end of dictionary element
            reader.ReadEndElement();
        }
    }

    /// <summary>
    /// Serializes the properties of this object to XML.
    /// </summary>
    public void WriteXml(XmlWriter writer)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(writer);

        // Get serialization information.
        CacheXmlSerializer();
        var keyType = typeof(TKey);
        var keyNeedsInstanceType = keyType.IsValueType || keyType == typeof(object);
        var valueType = typeof(TValue);
        var valueNeedsInstanceType = valueType.IsValueType || valueType == typeof(object);

        // Extend writer to control output (workaround standard serializer issues).
        using var injectWriter = new XmlInjectionWriter(writer);

        // Merge any preferred namespaces stored using XML serializer namespaces feature.
        foreach (var serializerNamespace in XmlNamespaces!.ToArray())
            _ = injectWriter.AssertPrefix(serializerNamespace.Namespace, serializerNamespace.Name);

        // Declare or suppress XSI and XSD namespaces as necessary.
        if (_xsiNamespaceRequired || keyNeedsInstanceType || valueNeedsInstanceType)
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
        if (thisPrefix?.Length > 0)
            _ = injectWriter.AssertPrefixWritten(_xmlNamespace, thisPrefix);

        // Write dictionary entries...
        foreach (var entry in this)
        {
            // Write entry start element.
            injectWriter.WriteStartElement(_xmlEntryName, _xmlNamespace);

            // Inject key XSI type when required.
            if (keyNeedsInstanceType)
                injectWriter.AddXsiType(_xmlKeyType!);

            // Write key element.
            injectWriter.RenameElement(thisPrefix, _xmlKeyName, _xmlNamespace);
            _xmlKeySerializer!.Serialize(injectWriter, entry.Key, XmlNamespaces);

            // Inject value XSI type when required.
            if (valueNeedsInstanceType)
                injectWriter.AddXsiType(_xmlValueType!);

            // Write value element
            injectWriter.RenameElement(thisPrefix, _xmlValueName, _xmlNamespace);
            _xmlValueSerializer!.Serialize(injectWriter, entry.Value, XmlNamespaces);

            // Write entry end element.
            injectWriter.WriteEndElement();
        }
    }

    /// <summary>
    /// Caches XML serialization information for items in this collection.
    /// </summary>
    private void CacheXmlSerializer()
    {
        // Do nothing when already cached.
        if (_xmlKeySerializer != null && _xmlValueSerializer != null)
            return;

        // Prepare serializer types.
        var keyType = typeof(TKey);
        var valueType = typeof(TValue);
        var xmlTypes = _xmlTypes.ToArray();

        // Create serializer for key type.
        var keyOverrides = new XmlAttributeOverrides();
        var keyInAnotherNamespace = _xmlKeyType != null && _xmlKeyType.Namespace != _xmlNamespace;
        if (keyInAnotherNamespace)
        {
            // Ensure serializer uses correct type when not system type.
            if (_xmlKeyType!.Namespace != XmlSchema.Namespace)
            {
                keyOverrides.Add(keyType, new XmlAttributes {
                    XmlType = new XmlTypeAttribute(_xmlKeyType.Name) { Namespace = _xmlKeyType.Namespace }
                });
            }
        }
        var keySerializerKey = GetType().FullName + "." + _xmlKeyName;
        _xmlKeySerializer = XmlSerializerCache.Create(keyType, keySerializerKey, () =>
        {
            return new XmlSerializer(keyType, keyOverrides, xmlTypes,
                new XmlRootAttribute(_xmlKeyName) { Namespace = _xmlNamespace }, _xmlNamespace);
        });

        // Create serializer for value type.
        var valueOverrides = new XmlAttributeOverrides();
        var valueInAnotherNamespace = _xmlValueType != null && _xmlValueType.Namespace != _xmlNamespace;
        if (valueInAnotherNamespace)
        {
            // Ensure serializer uses correct type when not system type.
            if (_xmlValueType!.Namespace != XmlSchema.Namespace)
            {
                valueOverrides.Add(valueType, new XmlAttributes {
                    XmlType = new XmlTypeAttribute(_xmlValueType.Name) { Namespace = _xmlValueType.Namespace }
                });
            }
        }
        var valueSerializerKey = GetType().FullName + "." + _xmlValueName;
        _xmlValueSerializer = XmlSerializerCache.Create(valueType, valueSerializerKey, () =>
        {
            return new XmlSerializer(valueType, valueOverrides, xmlTypes,
                new XmlRootAttribute(_xmlValueName) { Namespace = _xmlNamespace }, _xmlNamespace);
        });
    }

    /// <summary>
    /// Common initialization.
    /// </summary>
    private void Initialize(string xmlNamespace, string xmlNamespacePrefix,
        string xmlEntryName, string xmlKeyName, string xmlValueName, bool xsiNamespaceRequired)
    {
        // .NET Types for XML serialization
        var keyType = typeof(TKey);
        var valueType = typeof(TValue);
        _xmlTypes.Add(new[] { keyType, valueType });
        XmlSerializerCache.RegisterTypes(_xmlTypes);

        // XML element names
        _xmlNamespace = xmlNamespace;
        _xmlNamespacePrefix = xmlNamespacePrefix;
        _xmlEntryName = xmlEntryName;
        _xmlKeyName = xmlKeyName;
        _xmlValueName = xmlValueName;
        _xsiNamespaceRequired = xsiNamespaceRequired;

        // XML instance types (XSI)
        _xmlKeyType = XmlExtensions.GetXmlType(keyType)!;
        _xmlValueType = XmlExtensions.GetXmlType(valueType)!;
    }
}
