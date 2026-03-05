using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test class which has an <see cref="XmlNamespaceDeclarationsAttribute"/> to test XML serialization.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = XmlNamespace)]
[XmlType(XmlTypeName, Namespace = XmlNamespace)]
public class TestXmlDataNamespace
{
    /// <summary>
    /// XML namespace.
    /// </summary>
    public const string XmlNamespace = TestConstants.XmlNamespace;

    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = "TestXmlDataNamespace";

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = "TestXmlDataNamespaceType";

    /// <summary>
    /// Field behind the <see cref="XmlNamespaces"/> property.
    /// </summary>
    [NonSerialized]
    private XmlSerializerNamespaces _xmlNamespaces;

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public TestXmlDataNamespace()
    {
        // Ensure serializer maintains preferred prefixes when possible
        _xmlNamespaces = new XmlSerializerNamespaces();
        _ = _xmlNamespaces.AssertPrefix(XmlSchema.Namespace, XmlExtensions.XsdNamespacePrefixDefault);
        _ = _xmlNamespaces.AssertPrefix(XmlSchema.InstanceNamespace, XmlExtensions.XsiNamespacePrefixDefault);
    }

    /// <summary>
    /// Unique ID.
    /// </summary>
    /// <remarks>Deliberately mapped to an XML attribute to ensure they also serialize correctly.</remarks>
    [XmlAttribute]
    public Guid Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// XML name table for serialization.
    /// </summary>
    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces XmlNamespaces { get => _xmlNamespaces; set => _xmlNamespaces = value; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlDataNamespace left, TestXmlDataNamespace right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlDataNamespace left, TestXmlDataNamespace right)
    {
        return left is not null
            ? left.Equals(right)
            : right is null;
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override bool Equals(object? other)
    {
        // Check for null and type
        if (other is not TestXmlDataNamespace data)
            return false;

        // Compare values
        return
            data.Id == Id &&
            data.Name == Name;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            Id.GetHashCode() ^
            (Name?.GetHashCode() ?? 0);
    }
}
