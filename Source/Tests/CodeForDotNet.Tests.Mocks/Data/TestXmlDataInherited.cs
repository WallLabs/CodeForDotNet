using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#nullable enable

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test class which extends an base type to test XML serialization.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = XmlNamespace)]
[XmlType(XmlTypeName, Namespace = XmlNamespace)]
public class TestXmlDataInherited : TestXmlDataSmall
{
    /// <summary>
    /// XML root element name.
    /// </summary>
    public new const string XmlRootName = "TestXmlDataInherited";

    /// <summary>
    /// XML type name.
    /// </summary>
    public new const string XmlTypeName = "TestXmlDataInheritedType";

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlDataInherited left, TestXmlDataInherited right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlDataInherited left, TestXmlDataInherited right)
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
        if (other is not TestXmlDataInherited data)
            return false;

        // Compare values
        return
            base.Equals(data) &&
            data.Description == Description;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            base.GetHashCode() ^
            (Description?.GetHashCode() ?? 0);
    }
}
