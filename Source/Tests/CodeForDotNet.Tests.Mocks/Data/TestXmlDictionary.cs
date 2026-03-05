using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test implementation of the <see cref="XmlDictionary{TKey, TValue}"/>.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = TestConstants.XmlNamespace)]
[XmlSchemaProvider(nameof(GetSchema))]
public sealed class TestXmlDictionary : XmlDictionary<Guid, TestXmlDataSmall>
{
    /// <summary>
    /// XML element name to use when serializing entries.
    /// </summary>
    public const string XmlEntryName = "TestEntry";

    /// <summary>
    /// XML element name to use when serializing keys.
    /// </summary>
    public const string XmlKeyName = "TestKey";

    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = "TestXmlDictionary";

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = "TestXmlDictionaryType";

    /// <summary>
    /// XML element name to use when serializing values.
    /// </summary>
    public const string XmlValueName = "TestValue";

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public TestXmlDictionary()
        : base(CodeTestsMocksXsd.XmlNamespace, CodeTestsMocksXsd.XmlNamespacePrefixDefault, XmlEntryName, XmlKeyName, XmlValueName, true)
    {
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    private TestXmlDictionary(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlDictionary left, TestXmlDictionary right)
    {
        return left is not null ? !left.Equals(right) : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlDictionary left, TestXmlDictionary right)
    {
        return left is not null ? left.Equals(right) : right is null;
    }

    /// <summary>
    /// Gets the XML schema and type name for serialization.
    /// </summary>
    public static new XmlQualifiedName GetSchema(XmlSchemaSet schemas)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(schemas);

        // Add schemata and return qualified name.
        schemas.Add(CodeTestsMocksXsd.GetSchema());
        return new XmlQualifiedName(XmlTypeName, CodeTestsMocksXsd.XmlNamespace);
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    public override bool Equals(object? other)
    {
        // Check for null and type
        if (other is not TestXmlDictionary dictionary)
            return false;

        // Compare values
        return base.Equals(dictionary);
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
