using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test implementation of the <see cref="XmlCollection{TItem}"/>.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = TestConstants.XmlNamespace)]
[XmlSchemaProvider(nameof(GetSchema))]
[XmlInclude(typeof(TestXmlDataSmall))]
public class TestXmlCollection : XmlCollection<TestXmlDataSmall>
{
    /// <summary>
    /// XML element name to use when serializing items.
    /// </summary>
    public const string XmlItemName = "TestItem";

    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = nameof(TestXmlCollection);

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = nameof(TestXmlCollection) + "Type";

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public TestXmlCollection()
        : base(CodeTestsMocksXsd.XmlNamespace, CodeTestsMocksXsd.XmlNamespacePrefixDefault, XmlItemName, true)
    {
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    protected TestXmlCollection(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlCollection left, TestXmlCollection right)
    {
        return left is not null ? !left.Equals(right) : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlCollection left, TestXmlCollection right)
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
        if (other is not TestXmlCollection collection)
            return false;

        // Compare values
        return base.Equals(collection);
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
