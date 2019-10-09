using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CodeForDotNet.Collections
{
	/// <summary>
	/// Provides helper methods and extensions for working with arrays and collections.
	/// </summary>
	public static class ArrayExtensions
	{
		#region Public Methods

		/// <summary>
		/// Compares two list based arrays by value.
		/// </summary>
		public static bool AreEqual(IList? left, IList? right)
		{
			// Compare null
			if (left == null)
				return right == null;
			if (right == null)
				return false;

			// Compare length
			if (left.Count != right.Count)
				return false;

			// Compare values
			for (var index = 0; index < left.Count; index++)
			{
				var value1 = left[index];
				var value2 = right[index];
				if (value1 is null)
				{
					// Not same when nullability doesn't match.
					if (!(value2 is null))
						return false;
				}
				else
				{
					// Compare nested array by value too
					if (value1.GetType().IsArray)
						return AreEqual((Array)value1, (Array)value2);

					// Compare other objects using any defined comparer or operator overloads This will still compare reference types by reference when none are defined
					if (!value1.Equals(value2))
						return false;
				}
			}

			// Return same
			return true;
		}

		/// <summary>
		/// Compares two collections by value.
		/// </summary>
		public static bool AreEqual(IEnumerable? left, IEnumerable? right)
		{
			// Compare null
			if (left == null)
				return right == null;
			if (right == null)
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
				if (value1 is null)
				{
					// Not same when nullability doesn't match.
					if (!(value2 is null))
						return false;
				}
				else
				{
					// Compare nested array by value too
					if (value1.GetType().IsArray)
						return AreEqual((Array)value1, (Array)value2);

					// Compare other objects using any defined comparer or operator overloads This will still compare reference types by reference when none are defined
					if (!value1.Equals(value2))
						return false;
				}

				// Next...
			} while (true);
		}

		/// <summary>
		/// Compares part of two arrays for equality.
		/// </summary>
		public static bool AreEqual(byte[]? left, int leftOffset, byte[]? right, int rightOffset, int length)
		{
			// Validate.
			if (leftOffset > 0 && left is null) throw new ArgumentOutOfRangeException(nameof(leftOffset));
			if (rightOffset > 0 && right is null) throw new ArgumentOutOfRangeException(nameof(rightOffset));

			// Check nullability.
			if (left is null && right is null)
				return true;
			if (left is null != right is null)
				return false;

			// Check length does not exceed boundaries.
			if (leftOffset + length > (left?.Length ?? 0) || rightOffset + length > (right?.Length ?? 0))
				return false;

			// Compare array contents.
			for (var i = 0; i < length; i++)
			{
				if (left?[leftOffset + i] != right?[rightOffset + i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Searches any array for a value, i.e. without having to create a list or collection.
		/// </summary>
		/// <typeparam name="T">Array type.</typeparam>
		/// <param name="array">Array to search.</param>
		/// <param name="value">Value to find.</param>
		/// <returns>True when present.</returns>
		public static bool Contains<T>(this IEnumerable<T>? array, T value)
		{
			return array.Any(item => item?.Equals(value) ?? false);
		}

		/// <summary>
		/// Checks if the string array contains the specified value optionally ignoring case.
		/// </summary>
		/// <param name="array">Array to search.</param>
		/// <param name="value">Value to search for.</param>
		/// <param name="comparisonType">Comparison options, e.g. set to <see cref="StringComparison.OrdinalIgnoreCase"/> for a case insensitive comparison.</param>
		/// <returns>True when found.</returns>
		public static bool Contains(this IEnumerable<string>? array, string? value, StringComparison comparisonType = StringComparison.Ordinal)
		{
			return array.FirstOrDefault(item => string.Compare(item, value, comparisonType) == 0) != null;
		}

		/// <summary>
		/// Checks if the string collection contains the specified value optionally ignoring case.
		/// </summary>
		/// <param name="collection">Collection to search.</param>
		/// <param name="value">Value to search for.</param>
		/// <param name="comparisonType">Comparison options, e.g. set to <see cref="StringComparison.OrdinalIgnoreCase"/> for a case insensitive comparison.</param>
		/// <returns>True when found.</returns>
		public static bool Contains(this StringCollection? collection, string? value, StringComparison comparisonType = StringComparison.Ordinal)
		{
			// Cast to array.
			var array = collection.OfType<string>();

			// Call overloaded method to check contents.
			return array.Contains(value, comparisonType);
		}

		/// <summary>
		/// Disposes all members implementing <see cref="IDisposable"/>.
		/// </summary>
		/// <param name="list">List of items to dispose.</param>
		public static void Dispose(this IList? list)
		{
			// Do nothing when null
			if (list == null)
				return;

			// Dispose all members
			foreach (var disposable in list.Cast<IDisposable>().ToArray())
			{
				list.Remove(disposable);
				disposable.Dispose();
			}
		}

		/// <summary>
		/// Gets the hash code of all items in the array.
		/// </summary>
		public static int GetHashCode(this IList? array)
		{
			// Return zero when null
			if (array is null)
				return 0;

			// Calculate and return hash of all items
			return array.Cast<object>().Aggregate(0, (current, item) => current ^ (item?.GetHashCode() ?? 0));
		}

		#endregion Public Methods
	}
}
