using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using CodeForDotNet.Collections;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test class which holds difficult data to serialize.
/// </summary>
[Serializable]
[XmlRoot(XmlRootName, Namespace = TestConstants.XmlNamespace)]
[XmlType(XmlTypeName, Namespace = TestConstants.XmlNamespace)]
public class TestXmlData
{
    /// <summary>
    /// XML root element name.
    /// </summary>
    public const string XmlRootName = nameof(TestXmlData);

    /// <summary>
    /// XML type name.
    /// </summary>
    public const string XmlTypeName = nameof(TestXmlData) + "Type";

    /// <summary>
    /// Some value which should be preserved (not read past).
    /// </summary>
    public string AfterTheInheritedXmlCollection { get; set; } = "";

    /// <summary>
    /// Some value which should be preserved (not read past).
    /// </summary>
    public string AfterTheInheritedXmlSerializableDictionary { get; set; } = "";

    /// <summary>
    /// Some value which should be preserved (not read past).
    /// </summary>
    public string AfterTheTypedXmlCollection { get; set; } = "";

    /// <summary>
    /// Some value which should be preserved (not read past).
    /// </summary>
    public string AfterTheTypedXmlSerializableDictionary { get; set; } = "";

    /// <summary>
    /// Some value which should be preserved (not read past).
    /// </summary>
    public string AfterTheXmlCollection { get; set; } = "";

    /// <summary>
    /// Inherited XML serializable collection.
    /// </summary>
    public TestXmlCollectionInherited AnInheritedXmlCollection { get; set; } = [];

    /// <summary>
    /// Inherited XML serializable dictionary.
    /// </summary>
    public TestXmlDictionaryInherited AnInheritedXmlSerializableDictionary { get; set; } = [];

    /// <summary>
    /// Standard collection without any enhanced XML support.
    /// </summary>
    [XmlArrayItem("Small")]
    public Collection<TestXmlDataSmall> StandardCollection { get; set; } = [];

    /// <summary>
    /// Typed XML serializable collection.
    /// </summary>
    public TestXmlCollection ATypedXmlCollection { get; set; } = [];

    /// <summary>
    /// Typed XML serializable dictionary.
    /// </summary>
    public TestXmlDictionary ATypedXmlSerializableDictionary { get; set; } = [];

    /// <summary>
    /// Tests the <see cref="XmlDateTime"/>.
    /// </summary>
    public XmlDateTime XmlDateTime { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestXmlData left, TestXmlData right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestXmlData left, TestXmlData right)
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
        if (other is not TestXmlData data)
            return false;

        // Compare values
        return
            data.XmlDateTime == XmlDateTime &&
            ArrayExtensions.AreEqual(data.StandardCollection, StandardCollection) &&
            data.AfterTheXmlCollection == AfterTheXmlCollection &&
            ArrayExtensions.AreEqual(data.AnInheritedXmlCollection, AnInheritedXmlCollection) &&
            data.AfterTheInheritedXmlCollection == AfterTheInheritedXmlCollection &&
            ArrayExtensions.AreEqual(data.ATypedXmlSerializableDictionary, ATypedXmlSerializableDictionary) &&
            data.AfterTheTypedXmlSerializableDictionary == AfterTheTypedXmlSerializableDictionary &&
            ArrayExtensions.AreEqual(data.AnInheritedXmlSerializableDictionary, AnInheritedXmlSerializableDictionary) &&
            data.AfterTheInheritedXmlSerializableDictionary == AfterTheInheritedXmlSerializableDictionary;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            XmlDateTime.GetHashCode() ^
            ArrayExtensions.GetHashCodeOfItems(StandardCollection) ^
            (AfterTheXmlCollection?.GetHashCode() ?? 0) ^
            ArrayExtensions.GetHashCodeOfItems(AnInheritedXmlCollection) ^
            (AfterTheInheritedXmlCollection?.GetHashCode() ?? 0) ^
            ArrayExtensions.GetHashCodeOfItems(ATypedXmlSerializableDictionary) ^
            (AfterTheTypedXmlSerializableDictionary?.GetHashCode() ?? 0) ^
            ArrayExtensions.GetHashCodeOfItems(AnInheritedXmlSerializableDictionary) ^
            (AfterTheInheritedXmlSerializableDictionary?.GetHashCode() ?? 0);
    }
}
