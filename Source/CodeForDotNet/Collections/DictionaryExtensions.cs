using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CodeForDotNet.Collections
{
	/// <summary>
	/// Extensions and helper methods for work with <see cref="IDictionary"/> and <see cref="IDictionary{TKey, TValue}"/> objects.
	/// </summary>
	public static class DictionaryExtensions
	{
		#region Private Fields

		/// <summary>
		/// Default format string used prefix the key of a dictionary to it's values.
		/// </summary>
		private const string AddKeysToValuesDefaultFormat = "{0} - {1}";

		#endregion Private Fields

		#region Public Methods

		/// <summary>
		/// Prefixes the key to all values in a dictionary using the <see cref="AddKeysToValuesDefaultFormat"/>.
		/// </summary>
		/// <param name="dictionary">Dictionary to update.</param>
		public static void AddKeysToValues(this IDictionary dictionary)
		{
			AddKeysToValues(dictionary, AddKeysToValuesDefaultFormat);
		}

		/// <summary>
		/// Prefixes the key to all values in a dictionary using a specific format.
		/// </summary>
		/// <param name="dictionary">Dictionary to update.</param>
		/// <param name="format">Format used to join the values. The first argument is the key and the second the value.</param>
		public static void AddKeysToValues(this IDictionary dictionary, string format)
		{
			// Validate
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			if (format == null)
				throw new ArgumentNullException(nameof(format));

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
		public static bool AreEqual<TKey, TValue>(IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
		{
			// Compare nullability only when either is null
			if (dictionary1 == null)
				return dictionary2 == null;
			if (dictionary2 == null)
				return false;

			// Compare length
			if (dictionary1.Count != dictionary2.Count)
				return false;

			// Compare values
			var dictionary1Enumerator = dictionary1.GetEnumerator();
			var dictionary2Enumerator = dictionary2.GetEnumerator();
			while (dictionary1Enumerator.MoveNext() & dictionary2Enumerator.MoveNext())
			{
				var item1 = dictionary1Enumerator.Current;
				var item2 = dictionary2Enumerator.Current;
				if (!(item1.Key?.Equals(item2.Key) ?? false) || !(item1.Value?.Equals(item2.Value) ?? item2.Value == null))
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
		/// <param name="dictionary">Dictionary of items to dispose.</param>
		public static void Dispose(this IDictionary dictionary)
		{
			// Validate
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			// Dispose members
			foreach (var disposable in dictionary.Values.Cast<IDisposable>().ToArray())
			{
				dictionary.Remove(disposable);
				disposable.Dispose();
			}
		}

		/// <summary>
		/// Disposes all members implementing <see cref="IDisposable"/>.
		/// </summary>
		/// <param name="dictionary">Dictionary of items to dispose.</param>
		public static void Dispose<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			// Validate
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			// Dispose members
			foreach (var pair in dictionary)
			{
				if (pair.Value is IDisposable disposable)
				{
					dictionary.Remove(pair.Key);
					disposable?.Dispose();
				}
			}
		}

		/// <summary>
		/// Gets the hash code of the keys and values of all items in the dictionary, or zero when null.
		/// </summary>
		public static int GetHashCode(IDictionary dictionary)
		{
			return dictionary?.GetHashCodeOfItems() ?? 0;
		}

		/// <summary>
		/// Gets the hash code of the keys and values of all items in the dictionary.
		/// </summary>
		public static int GetHashCodeOfItems(this IDictionary dictionary)
		{
			// Validate
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			// Calculate hash code
			var hash = 0;
			foreach (var item in dictionary.Keys)
				hash ^= item?.GetHashCode() ?? 0;
			foreach (var item in dictionary.Values)
				hash ^= item?.GetHashCode() ?? 0;
			return hash;
		}

		/// <summary>
		/// Returns the item in the dictionary if it exists, otherwise null.
		/// </summary>
		public static TValue GetIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			// Validate
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			// Call overloaded method
			return dictionary.ContainsKey(key) ? dictionary[key] : default!;
		}

		#endregion Public Methods
	}
}
