using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Enumeration of <see cref="TestData"/> column names.
/// </summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Readability.")]
public enum TestDataColumn
{
    /// <summary>
    /// Creation date.
    /// </summary>
    CreatedDate,

    /// <summary>
    /// User which created the record column name.
    /// </summary>
    CreatedByUserName,

    /// <summary>
    /// Last changed date column name.
    /// </summary>
    ChangedDate,

    /// <summary>
    /// User who last changed the record column name.
    /// </summary>
    ChangedByUserName,

    /// <summary>
    /// Test name column name.
    /// </summary>
    Name,

    /// <summary>
    /// Universal time test field column name.
    /// </summary>
    UniversalTime,

    /// <summary>
    /// Local time test field column name.
    /// </summary>
    LocalTime,

    /// <summary>
    /// Local time with offset test field column name.
    /// </summary>
    LocalTimeWithOffset,

    /// <summary>
    /// Serializable local time with offset test field column name.
    /// </summary>
    SerializableLocalTimeWithOffset,

    /// <summary>
    /// Text test field.
    /// </summary>
    Text,

    /// <summary>
    /// Text with string length test field column name.
    /// </summary>
    TextWithStringLength,

    /// <summary>
    /// Text with maximum length test field column name.
    /// </summary>
    TextWithMaxLength,

    /// <summary>
    /// Decimal test field column name.
    /// </summary>
    NumberDecimal,

    /// <summary>
    /// Integer test field column name.
    /// </summary>
    NumberInteger,

    /// <summary>
    /// Long test field column name.
    /// </summary>
    NumberLong,

    /// <summary>
    /// Boolean test field column name.
    /// </summary>
    Boolean,

    /// <summary>
    /// Required boolean test field column name.
    /// </summary>
    BooleanRequired,

    /// <summary>
    /// Nullable test field column name.
    /// </summary>
    BooleanNullable,

    /// <summary>
    /// GUID test field column name.
    /// </summary>
    Guid,

    /// <summary>
    /// Required GUID test field column name.
    /// </summary>
    GuidRequired,

    /// <summary>
    /// Nullable test field column name.
    /// </summary>
    GuidNullable
}
