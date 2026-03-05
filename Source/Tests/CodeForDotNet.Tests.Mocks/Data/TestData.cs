using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using CodeForDotNet.ComponentModel.DataAnnotations;
using CodeForDotNet.Xml;

namespace CodeForDotNet.Tests.Mocks.Data;

/// <summary>
/// Test data entity used throughout the tiers and components. Stored in the database in the
/// Test table.
/// </summary>
[Serializable]
[XmlRoot(Namespace = TestConstants.XmlNamespace)]
public class TestData
{
    /// <summary>
    /// Name of the test group which should validate separately.
    /// </summary>
    public const string TestValidationGroupName = "GroupTest";

    /// <summary>
    /// Boolean value.
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public bool Boolean { get; set; }

    /// <summary>
    /// Nullable boolean value.
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public bool? BooleanNullable { get; set; }

    /// <summary>
    /// Boolean value for which a specific value is required.
    /// </summary>
    [Required]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public bool BooleanRequired { get; set; }

    /// <summary>
    /// Name of the user which last changed the data.
    /// </summary>
    /// <remarks>Maximum 100 characters.</remarks>
    [Required]
    [ReadOnly(true)]
    [StringLength(100)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string ChangedByUserName { get; set; } = "";

    /// <summary>
    /// Last changed date and time in UTC.
    /// </summary>
    /// <remarks>Defaults to the same value as <see cref="CreatedDate"/>.</remarks>
    [Required]
    [ReadOnly(true)]
    [DateTimeFormat]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public DateTime ChangedDate { get; set; }

    /// <summary>
    /// Name of the user which created the data.
    /// </summary>
    /// <remarks>Maximum 100 characters.</remarks>
    [Required]
    [ReadOnly(true)]
    [StringLength(100)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string CreatedByUserName { get; set; } = "";

    /// <summary>
    /// Creation date and time in UTC.
    /// </summary>
    /// <remarks>Defaults to the current UTC date and time.</remarks>
    [Required]
    [ReadOnly(true)]
    [DateTimeFormat]
    [ValidationGroup(ValidationGroupNames.Create)]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Tests validation of ranges and a sub-group.
    /// </summary>
    [Range(1, 10)]
    [Display(Prompt = "Enter a number inside or outside the range to test group validation.")]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    [ValidationGroup(TestValidationGroupName)]
    public int GroupValidation { get; set; }

    /// <summary>
    /// GUID value.
    /// </summary>
    [RegularExpression(GuidExtensions.GuidFormat)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Readability.")]
    public Guid Guid { get; set; }

    /// <summary>
    /// Nullable GUID value.
    /// </summary>
    [RegularExpression(GuidExtensions.GuidFormat)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public Guid? GuidNullable { get; set; }

    /// <summary>
    /// GUID value which is required.
    /// </summary>
    [Required]
    [RegularExpression(GuidExtensions.GuidFormat)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public Guid GuidRequired { get; set; }

    /// <summary>
    /// Unique ID.
    /// </summary>
    [Required]
    [Key]
    [ReadOnly(true)]
    public Guid Id { get; set; }

    /// <summary>
    /// A date and time which should be displayed in local time.
    /// </summary>
    [Required]
    [DateTimeFormat]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public DateTime LocalTime { get; set; }

    /// <summary>
    /// A date and time which uses the <see cref="DateTimeOffset"/>.
    /// </summary>
    [Required]
    [DateTimeFormat]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public DateTimeOffset LocalTimeWithOffset { get; set; }

    /// <summary>
    /// Name of the record, unique.
    /// </summary>
    /// <remarks>Maximum 50 characters.</remarks>
    [Required]
    [StringLength(50)]
    [Display(Prompt = "Name", Description = "Unique name of this record.")]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Decimal number.
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public decimal? NumberDecimal { get; set; }

    /// <summary>
    /// Integer (32 bit) number.
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public int? NumberInteger { get; set; }

    /// <summary>
    /// Long (64 bit) number.
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public long? NumberLong { get; set; }

    /// <summary>
    /// A date and time which uses the <see cref="XmlDateTime"/>.
    /// </summary>
    [Required]
    [DateTimeFormat]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public XmlDateTime SerializableLocalTimeWithOffset { get; set; }

    /// <summary>
    /// Any text (with no limit).
    /// </summary>
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string? Text { get; set; }

    /// <summary>
    /// Text with new <see cref="MaxLengthAttribute"/>.
    /// </summary>
    [MaxLength(20)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string? TextWithMaxLength { get; set; }

    /// <summary>
    /// Text with old <see cref="StringLengthAttribute"/>.
    /// </summary>
    [StringLength(20)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public string? TextWithStringLength { get; set; }

    /// <summary>
    /// A date and time which should be stored in universal time.
    /// </summary>
    [Required]
    [DateTimeFormat(Display = DateTimeKind.Utc, Store = DateTimeKind.Utc)]
    [ValidationGroup(ValidationGroupNames.Create)]
    [ValidationGroup(ValidationGroupNames.Update)]
    public DateTime UniversalTime { get; set; }

    /// <summary>
    /// Tests two objects of this type for inequality by value.
    /// </summary>
    public static bool operator !=(TestData left, TestData right)
    {
        return left is not null
            ? !left.Equals(right)
            : right is not null;
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(TestData left, TestData right)
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
        if (other is not TestData data)
        {
            return false;
        }

        // Compare values
        return
            data.Id == Id &&
            data.CreatedDate == CreatedDate &&
            data.CreatedByUserName == CreatedByUserName &&
            data.ChangedDate == ChangedDate &&
            data.ChangedByUserName == ChangedByUserName &&
            data.Name == Name &&
            data.UniversalTime == UniversalTime &&
            data.LocalTime == LocalTime &&
            data.LocalTimeWithOffset == LocalTimeWithOffset &&
            data.SerializableLocalTimeWithOffset == SerializableLocalTimeWithOffset &&
            data.Text == Text &&
            data.TextWithStringLength == TextWithStringLength &&
            data.TextWithMaxLength == TextWithMaxLength &&
            data.NumberDecimal == NumberDecimal &&
            data.NumberInteger == NumberInteger &&
            data.NumberLong == NumberLong &&
            data.Boolean == Boolean &&
            data.BooleanRequired == BooleanRequired &&
            data.BooleanNullable == BooleanNullable &&
            data.Guid == Guid &&
            data.GuidRequired == GuidRequired &&
            data.GuidNullable == GuidNullable &&
            data.GroupValidation == GroupValidation;
    }

    /// <summary>
    /// Returns a hash-code based on the current value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return
            Id.GetHashCode() ^
            CreatedDate.GetHashCode() ^
            (CreatedByUserName?.GetHashCode() ?? 0) ^
            ChangedDate.GetHashCode() ^
            (ChangedByUserName?.GetHashCode() ?? 0) ^
            (Name?.GetHashCode() ?? 0) ^
            UniversalTime.GetHashCode() ^
            LocalTime.GetHashCode() ^
            LocalTimeWithOffset.GetHashCode() ^
            SerializableLocalTimeWithOffset.GetHashCode() ^
            (Text?.GetHashCode() ?? 0) ^
            (TextWithStringLength?.GetHashCode() ?? 0) ^
            (TextWithMaxLength?.GetHashCode() ?? 0) ^
            NumberDecimal.GetHashCode() ^
            NumberInteger.GetHashCode() ^
            NumberLong.GetHashCode() ^
            Boolean.GetHashCode() ^
            BooleanRequired.GetHashCode() ^
            BooleanNullable.GetHashCode() ^
            Guid.GetHashCode() ^
            GuidRequired.GetHashCode() ^
            GuidNullable.GetHashCode() ^
            GroupValidation.GetHashCode();
    }
}
