using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace CodeForDotNet.Data;

/// <summary>
/// Error data used to report failures to a service.
/// </summary>
[DataContract]
public class GuidDataKey
{
    #region Public Properties

    /// <summary>
    /// Unique identifier used to group reports from the same source.
    /// </summary>
    [DataMember(IsRequired = true)]
    public Guid Id { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Tests two objects of this type for in-equality by value.
    /// </summary>
    public static bool operator !=(GuidDataKey left, GuidDataKey right)
    {
        return !(left?.Equals(right) ?? (right is null));
    }

    /// <summary>
    /// Tests two objects of this type for equality by value.
    /// </summary>
    public static bool operator ==(GuidDataKey left, GuidDataKey right)
    {
        return left?.Equals(right) ?? (right is null);
    }

    /// <summary>
    /// Compares this object with another by value.
    /// </summary>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "Readability.")]
    public override bool Equals([MaybeNull] object other)
    {
        // Compare null and type
        return other is GuidDataKey key && key is not null && key.Id == Id;
    }

    /// <summary>
    /// Gets an XOR based hash code based on the contents of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    #endregion Public Methods
}
