using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using CodeForDotNet.Xml;

#nullable enable

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// CSV test data.
/// </summary>
[Serializable]
[XmlRoot(Namespace = TestConstants.XmlNamespace)]
public class CsvTestData
{
    /// <summary>
    /// Number of properties which should be handled by serialization or data adapter.
    /// </summary>
    public const int SerializablePropertyCount = 6;

    /// <summary>
    /// Required string with maximum length constraint.
    /// </summary>
    [Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// Value type without the required attribute.
    /// </summary>
    public int NotRequiredValue { get; set; }

    /// <summary>
    /// Nullable value type property.
    /// </summary>
    public int? NullableValue { get; set; }

    /// <summary>
    /// Third Property "Remark".
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// Date property which is not a normal <see cref="DateTime"/>.
    /// </summary>
    public XmlDateTime SpecialDateValue { get; set; }

    /// <summary>
    /// Required value type.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(CsvTestData left, CsvTestData right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(CsvTestData left, CsvTestData right)
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
        if (other is not CsvTestData data)
            return false;

        // Compare values
        return
            data.Name == Name &&
            data.Value == Value &&
            data.Remarks == Remarks &&
            data.NotRequiredValue == NotRequiredValue &&
            data.NullableValue == NullableValue &&
            data.SpecialDateValue == SpecialDateValue;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            (Name?.GetHashCode() ?? 0) ^
            Value.GetHashCode() ^
            (Remarks?.GetHashCode() ?? 0) ^
            NotRequiredValue.GetHashCode() ^
            NullableValue.GetHashCode() ^
            SpecialDateValue.GetHashCode();
    }
}
