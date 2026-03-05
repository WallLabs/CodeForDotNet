using System;
using System.Xml.Serialization;

#nullable enable

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test class which extends an abstract base type but has no XML root itself (requiring XSI
/// type specification) to test XML serialization.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = XmlNamespace)]
[XmlType(XmlTypeName, Namespace = XmlNamespace)]
public class TestXmlDataAbstractInherited : TestXmlDataAbstractBase
{
    /// <summary>
    /// XML root name.
    /// </summary>
    public new const string XmlRootName = nameof(TestXmlDataAbstractInherited);

    /// <summary>
    /// XML type name.
    /// </summary>
    public new const string XmlTypeName = XmlRootName + "Type";

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlDataAbstractInherited left, TestXmlDataAbstractInherited right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlDataAbstractInherited left, TestXmlDataAbstractInherited right)
    {
        return left is not null
            ? left.Equals(right)
            : right is null;
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    public override bool Equals(object? other)
    {
        // Check for null and type
        if (other is not TestXmlDataAbstractInherited data)
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
