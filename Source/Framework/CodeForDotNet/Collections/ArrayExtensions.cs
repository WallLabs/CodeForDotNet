using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeForDotNet.Collections;

/// <summary>
/// Provides helper methods and extensions for working with arrays and collections.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Adds the entire contents of an array to another.
    /// </summary>
    public static void Add(this IList target, IList source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        // Add all items from source
        foreach (var item in source)
            _ = target.Add(item);
    }

    /// <summary>
    /// Adds an item to a list if it is not only part of the list.
    /// </summary>
    public static void AddDistinct(this IList list, object item)
    {
        // validate parameters
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        // Add item if it is not inside the list already.
        if (!list.Contains(item))
            _ = list.Add(item);
    }

    /// <summary>
    /// Adds an item to a list if it is not only part of the list.
    /// </summary>
    public static void AddDistinct<T>(this IList<T> list, T item)
    {
        // validate parameters
        ArgumentNullException.ThrowIfNull(list);
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        // Add item if it is not inside the list already.
        if (!list.Contains(item))
            list.Add(item);
    }

    /// <summary>
    /// Adds items to a list if they are not already inside that list.
    /// </summary>
    public static void AddDistinct(this IList list, IEnumerable items)
    {
        // validate parameters
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(items);

        // Add items if they are not inside the list already.
        foreach (var item in items)
        {
            if (!list.Contains(item))
                _ = list.Add(item);
        }
    }

    /// <summary>
    /// Adds items to a list if they are not already inside that list.
    /// </summary>
    public static void AddDistinct<T>(this IList<T> list, IEnumerable<T> items)
    {
        // validate parameters
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(items);

        // Add items if they are not inside the list already.
        foreach (var item in items)
        {
            if (!list.Contains(item))
                list.Add(item);
        }
    }

    /// <summary>
    /// Compares two list based arrays by value.
    /// </summary>
    /// <remarks>Null and empty arrays are considered equal.</remarks>
    public static bool AreEqual(this IList? left, IList? right)
    {
        // Compare null or empty
        if (left is null || left.Count == 0)
            return right is null || right.Count == 0;
        if (right is null || right.Count == 0)
            return false;

        // Compare length
        if (left.Count != right.Count)
            return false;

        // Compare values
        for (var index = 0; index < left.Count; index++)
        {
            var value1 = left[index];
            var value2 = right[index];
            if (value1 is not null)
            {
                if (value2 is null)
                    return false;

                // Compare nested array by value too
                if (value1.GetType().IsArray)
                    return AreEqual((Array)value1, (Array)value2);

                // Compare other objects using any defined comparer or operator overloads This will
                // still compare reference types by reference when none are defined
                if (!value1.Equals(value2))
                    return false;
            }
            else if (value2 is not null)
            {
                return false;
            }
        }

        // Return same
        return true;
    }

    /// <summary>
    /// Compares part of two arrays for equality.
    /// </summary>
    /// <remarks>Null and empty arrays are considered equal.</remarks>
    public static bool AreEqual(byte[]? left, int leftOffset, byte[]? right, int rightOffset, int length)
    {
        // Compare null or empty
        if (left is null || left.Length == 0)
            return right is null || right.Length == 0;
        if (right is null || right.Length == 0)
            return false;

        // Check length does not exceed boundaries
        if (leftOffset + length > left.Length || rightOffset + length > right.Length)
            return false;

        // Compare array contents
        for (var i = 0; i < length; i++)
        {
            if (left[leftOffset + i] != right[rightOffset + i])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Compares two collections by value.
    /// </summary>
    /// <returns>True when equal.</returns>
    public static bool AreEqual(this IEnumerable? left, IEnumerable? right)
    {
        // Compare nullability only when either is null
        if (left is null)
            return right is null;
        if (right is null)
            return false;

        // Compare values
        var enumerator1 = left.GetEnumerator();
        var enumerator2 = right.GetEnumerator();
        do
        {
            // Get next item and check length
            var more1 = enumerator1.MoveNext();
            var more2 = enumerator2.MoveNext();
            if (more1 != more2)
            {
                // Different lengths
                return false;
            }
            if (!more1)
            {
                // End with no differences
                return true;
            }

            // Compare current values
            var value1 = enumerator1.Current;
            var value2 = enumerator2.Current;
            if (value1 is not null)
            {
                // Compare nested array by value too
                if (value1.GetType().IsArray)
                    return AreEqual((Array)value1, (Array)value2);

                // Compare other objects using any defined comparer or operator overloads This will
                // still compare reference types by reference when none are defined
                if (!value1.Equals(value2))
                    return false;
            }
            else if (value2 is not null)
            {
                return false;
            }

            // Next...
        }
        while (true);
    }

    /// <summary>
    /// Compares an array of values numerically.
    /// </summary>
    /// <param name="left">
    /// Array of values with which to compare <paramref name="right"/>. Can be null.
    /// </param>
    /// <param name="right">
    /// Array of values which to compare against <paramref name="left"/>. Can be null.
    /// </param>
    /// <returns>
    /// Returns -1 when <see paramref="left"/> is greater than <paramref name="right"/>, 1 when less
    /// than and zero when they are the same.
    /// </returns>
    public static int Compare<T>(this T[]? left, T[]? right)
        where T : struct
    {
        // Calculate length and padding necessary to compare The most significant value is first,
        // which means the last array value must align on both sides so the first value must be
        // padded/shifted along. e.g. 1234 and 12345678 compares 00001234 and 12345678 (second is greater).
        var leftLength = left?.Length ?? 0;
        var rightLength = right?.Length ?? 0;
        var size = leftLength > rightLength ? leftLength : rightLength;
        var leftPad = size - leftLength;
        var rightPad = size - rightLength;

        // Compare until one is different
        for (int count = 0, leftIndex = -leftPad, rightIndex = -rightPad;
            count < size; count++, leftIndex++, rightIndex++)
        {
            // Get left and right value with padding
            var leftValue = leftIndex >= 0 ? left![leftIndex] : default;
            var rightValue = rightIndex >= 0 ? right![rightIndex] : default;

            // Return first difference (most significant = no need to compare further)
            var comparison = Comparer<T>.Default.Compare(leftValue, rightValue);
            if (comparison != 0)
                return comparison;
        }

        // Same
        return 0;
    }

    /// <summary>
    /// Checks if the array contains the value.
    /// </summary>
    /// <typeparam name="T">Value type inside the array.</typeparam>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to search for.</param>
    /// <returns>True when found.</returns>
    public static bool ContainsItem<T>(this IEnumerable<T> array, T value)
    {
        // Call overloaded method
        return ContainsItem(array, item => item?.Equals(value) ?? false);
    }

    /// <summary>
    /// Checks if the array contains the value using a specific comparison.
    /// </summary>
    /// <typeparam name="T">Value type inside the array.</typeparam>
    /// <param name="array">Array to search.</param>
    /// <param name="comparison">Comparison expression.</param>
    /// <returns>True when found.</returns>
    public static bool ContainsItem<T>(this IEnumerable<T> array, Func<T, bool> comparison)
    {
        // Validate (in debug mode)
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(comparison);

        // Search array...
        foreach (var item in array)
        {
            if (comparison(item))
            {
                // Found
                return true;
            }
        }

        // Not found
        return false;
    }

    /// <summary>
    /// Checks if the string array contains the specified value.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to search for.</param>
    /// <returns>True when found.</returns>
    /// <remarks>Not called "Contains" to avoid naming conflicts when used alongside LINQ.</remarks>
    public static bool ContainsString(this IEnumerable<string> array, string value)
    {
        return ContainsString(array, value, StringComparison.CurrentCulture);
    }

    /// <summary>
    /// Checks if the string array contains the specified value optionally ignoring case.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to search for.</param>
    /// <param name="comparisonType">
    /// Comparison options, e.g. set to <see cref="StringComparison.OrdinalIgnoreCase"/> for a case
    /// insensitive comparison.
    /// </param>
    /// <returns>True when found.</returns>
    /// <remarks>Not called "Contains" to avoid naming conflicts when used alongside LINQ.</remarks>
    public static bool ContainsString(this IEnumerable<string> array, string value, StringComparison comparisonType)
    {
        return ContainsItem(array, (item) => string.Equals(item, value, comparisonType));
    }

    /// <summary>
    /// Disposes all members implementing <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="list">List of items to dispose.</param>
    public static void Dispose(this IList list)
    {
        // Do nothing when null
        if (list is null)
            return;

        // Dispose each member when possible
        foreach (var disposable in list.Cast<IDisposable>().ToArray())
        {
            list.Remove(disposable);
            disposable.Dispose();
        }
    }

    /// <summary>
    /// Gets the hash code of all items in the array, or zero when null.
    /// </summary>
    public static int GetHashCodeOfItems<T>(this IEnumerable<T>? array)
    {
        return GetHashCodeOfItems((IEnumerable?)array);
    }

    /// <summary>
    /// Gets the hash code of all items in the array, or zero when null.
    /// </summary>
    public static int GetHashCodeOfItems(this IEnumerable? array)
    {
        // Return zero when null
        if (array is null)
            return 0;

        // Calculate and return hash of items
        var hash = 0;
        var sequence = 0;
        foreach (var item in array)
        {
            // Add sequence number to hash to assert same order.
            hash ^= sequence;

            // Hash according to type
            if (item is not null)
            {
                if (item is IEnumerable childArray)
                {
                    // Recurse child arrays (else we would not get a useful hash of child arrays)
                    hash ^= GetHashCodeOfItems(childArray);
                }
                else
                {
                    // Add direct hash of flat object
                    hash ^= item.GetHashCode();
                }
            }
            else
            {
                // Add null item hash.
                hash ^= 0;
            }
        }

        // Return total hash
        return hash;
    }

    /// <summary>
    /// Replaces the entire contents of an array with another, without changing the reference
    /// (clears then adds all members).
    /// </summary>
    public static void Replace(this IList target, IList source)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        // Clear target
        target.Clear();

        // Add all items from source
        _ = target.Add(source);
    }
}
