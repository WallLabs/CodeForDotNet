using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Testing;

/// <summary>
/// Test helper methods for operator functionality.
/// </summary>
public static class OperatorAssert
{
    /// <summary>
    /// Tests comparison and hashing of two objects.
    /// </summary>
    [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "Makes sense.")]
    public static void AssertCompare<T>(T left, T right)
    {
        // Validate (should never be used to test null)
        if (left is null || right is null)
            throw new ArgumentNullException(nameof(object.ReferenceEquals));

        // Test equality method (should be true)
        if (!left.Equals(right))
            throw new ArgumentOutOfRangeException(nameof(object.Equals));

        // Test hash code method (should be same)
        var hash1 = left.GetHashCode();
        var hash2 = right.GetHashCode();
        if (hash1 != hash2)
            throw new ArgumentOutOfRangeException(nameof(object.GetHashCode));

        // Test equality operators (should be equal)
        if (left == (dynamic)right != true)
            throw new ArgumentOutOfRangeException("==");
        if (left != (dynamic)right)
            throw new ArgumentOutOfRangeException("!=");
    }
}
