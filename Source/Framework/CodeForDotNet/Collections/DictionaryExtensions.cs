using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CodeForDotNet.Collections;

/// <summary>
/// Extensions and helper methods for work with <see cref="IDictionary"/> and <see cref="IDictionary{TKey, TValue}"/> objects.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Default format string used prefix the key of a dictionary to it's values.
    /// </summary>
    private const string AddKeysToValuesDefaultFormat = "{0} - {1}";

    /// <summary>
    /// Prefixes the key to all values in a dictionary using a specific format.
    /// </summary>
    /// <param name="dictionary">Dictionary to update.</param>
    /// <param name="format">
    /// Format used to join the values. The first argument is the key and the second the value.
    /// </param>
    public static void AddKeysToValues(this IDictionary dictionary, string format = AddKeysToValuesDefaultFormat)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(dictionary);

        // Get a fixed list of keys so we can modify the dictionary
        var keys = dictionary.Keys.Cast<object>().ToArray();

        // Update dictionary
        foreach (var key in keys)
        {
            // Get old value
            var value = dictionary[key];

            // Format and set new value with key added
            dictionary[key] = string.Format(CultureInfo.InvariantCulture, format, key, value);
        }
    }

    /// <summary>
    /// Compares two dictionaries by value.
    /// </summary>
    public static bool AreEqual<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
    {
        // Compare nullability only when either is null
        if (left is null)
            return right is null;
        if (right is null)
            return false;

        // Compare length
        if (left.Count != right.Count)
            return false;

        // Compare values
        var dictionary1Enumerator = left.GetEnumerator();
        var dictionary2Enumerator = right.GetEnumerator();
        while (dictionary1Enumerator.MoveNext() & dictionary2Enumerator.MoveNext())
        {
            var item1 = dictionary1Enumerator.Current;
            var item2 = dictionary2Enumerator.Current;
            if (!(item1.Key?.Equals(item2.Key) ?? item2.Key is not null) ||
                !(item1.Value?.Equals(item2.Value) ?? item2.Value is not null))
            {
                return false;
            }
        }

        // Return same
        return true;
    }

    /// <summary>
    /// Disposes all members implementing <see cref="IDisposable"/>.
    /// </summary>
    public static void Dispose(this IDictionary dictionary)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(dictionary);

        // Search for and dispose members which are disposable.
        foreach (var disposable in dictionary.Values.Cast<IDisposable>().ToArray())
            disposable.Dispose();
    }

    /// <summary>
    /// Gets the hash code of the keys and values of all items in the dictionary, or zero when null.
    /// </summary>
    public static int GetHashCodeOfItems(this IDictionary dictionary)
    {
        // Return zero when null
        if (dictionary is null)
            return 0;

        // Calculate hash code
        var hash = 0;
        foreach (var item in dictionary.Keys)
            hash ^= item is not null ? item.GetHashCode() : 0;
        foreach (var item in dictionary.Values)
            hash ^= item is not null ? item.GetHashCode() : 0;
        return hash;
    }

    /// <summary>
    /// Returns the item in the dictionary if it exists, otherwise null.
    /// </summary>
    public static TValue GetIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        where TKey : notnull
        where TValue : struct
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(dictionary);

        // Return default value when not found.
        return !dictionary.TryGetValue(key, out var value) ? defaultValue : value;
    }

    /// <summary>
    /// Returns the item in the dictionary if it exists, otherwise null.
    /// </summary>
    public static TValue? GetIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        where TValue : class
    {
        // Validate
        ArgumentNullException.ThrowIfNull(dictionary);

        // Call overloaded method
        return !dictionary.TryGetValue(key, out var value) ? default : value;
    }
}