using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Data entity for XML abstract base class serialization tests.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = XmlNamespace)]
[XmlType(XmlTypeName, Namespace = XmlNamespace)]
public abstract class TestXmlDataAbstractBase
{
    /// <summary>
    /// XML namespace.
    /// </summary>
    public const string XmlNamespace = TestConstants.XmlNamespace;

    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = "TestXmlDataAbstractBase";

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = "TestXmlDataAbstractBaseType";

    /// <summary>
    /// Base property.
    /// </summary>
    public string? BaseProperty { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlDataAbstractBase left, TestXmlDataAbstractBase right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlDataAbstractBase left, TestXmlDataAbstractBase right)
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
        if (other is not TestXmlDataAbstractBase data)
            return false;

        // Compare values
        return
            data.BaseProperty == BaseProperty;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            BaseProperty?.GetHashCode() ?? 0;
    }
}
