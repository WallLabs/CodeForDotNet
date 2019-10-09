using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Wia = Interop.Wia;

#nullable enable

namespace CodeForDotNet.Windows.Imaging
{
	/// <summary>
	/// Managed <see cref="Wia.Vector"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Named after unmanaged type it extends.")]
	[SuppressMessage("Microsoft.Design", "CA1010", Justification = "Extends unmanaged type which is a collection of different object types.")]
	public class WiaVector : IList
	{
		#region Private Fields

		private static readonly object _syncRoot = new object();

		/// <summary>
		/// Unmanaged <see cref="Wia.Vector"/>.
		/// </summary>
		private readonly Wia.Vector _wiaVector;

		#endregion Private Fields

		#region Internal Constructors

		/// <summary>
		/// Creates an instance to wrap the specified unmanaged object.
		/// </summary>
		internal WiaVector(Wia.Vector vector)
		{
			_wiaVector = vector;
		}

		#endregion Internal Constructors

		#region Public Properties

		/// <summary>
		/// Number of items in the collection.
		/// </summary>
		public int Count { get { return _wiaVector.Count; } }

		/// <summary>
		/// Date.
		/// </summary>
		public DateTime Date
		{
			get { return _wiaVector.Date; }
			set { _wiaVector.Date = value; }
		}

		/// <summary>
		/// Indicates whether this list is a fixed size. Returns false because WIA vectors are variable.
		/// </summary>
		public bool IsFixedSize { get { return false; } }

		/// <summary>
		/// Indicates whether this list is read-only. Returns false because WIA vectors are writable.
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Indicates that the collection is synchronized (thread safe).
		/// </summary>
		public bool IsSynchronized { get { return true; } }

		/// <summary>
		/// Thread synchronization object.
		/// </summary>
		public object SyncRoot { get { return _syncRoot; } }

		#endregion Public Properties

		#region Public Indexers

		/// <summary>
		/// Gets or sets an item at the specified index.
		/// </summary>
		/// <param name="index">Zero based index</param>
		/// <returns>Item value at the specified index.</returns>
		public object this[int index]
		{
			get
			{
				object? item = null;
				_wiaVector.let_Item(index + 1, ref item);
				return item;
			}

			set
			{
				_wiaVector.Remove(index + 1);
				Insert(index, value);
			}
		}

		#endregion Public Indexers

		#region Public Methods

		/// <summary>
		/// Adds an item to the end of the vector data.
		/// </summary>
		public int Add(object value)
		{
			_wiaVector.Add(value);
			return _wiaVector.Count - 1;
		}

		/// <summary>
		/// Clears all data.
		/// </summary>
		public void Clear()
		{
			object? missing = null;
			_wiaVector.set_BinaryData(ref missing);
		}

		/// <summary>
		/// Checks if the member exists.
		/// </summary>
		public bool Contains(object value)
		{
			// Search all items (1 based array)
			for (var i = 1; i <= _wiaVector.Count; i++)
			{
				object? existingItem = null;
				_wiaVector.let_Item(i, ref existingItem);
				if (existingItem == value)
					return true;
			}

			// Not found
			return false;
		}

		/// <summary>
		/// Copies items from this collection to an array.
		/// </summary>
		/// <param name="array">Target array.</param>
		/// <param name="index">Target zero based index.</param>
		public void CopyTo(WiaVector array, int index)
		{
			// Validate
			if (array == null) throw new ArgumentNullException(nameof(array));

			// Copy
			for (var i = 1; i <= _wiaVector.Count; i++)
			{
				object? item = null;
				_wiaVector.let_Item(index + 1, ref item);
				array[index + i - 1] = item;
			}
		}

		/// <summary>
		/// Copies items from this collection to an array.
		/// </summary>
		/// <param name="array">Target array.</param>
		/// <param name="index">Target zero based index.</param>
		public void CopyTo(Array array, int index)
		{
			// Validate
			if (array == null) throw new ArgumentNullException(nameof(array));

			// Copy
			for (var i = 1; i <= _wiaVector.Count; i++)
			{
				object? item = null;
				_wiaVector.let_Item(index + 1, ref item);
				array.SetValue(item, index + i - 1);
			}
		}

		/// <summary>
		/// Gets the binary data.
		/// </summary>
		public byte[] GetBinary()
		{
			return (byte[])_wiaVector.get_BinaryData();
		}

		/// <summary>
		/// Gets an enumerator for this collection.
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			return _wiaVector.GetEnumerator();
		}

		/// <summary>
		/// Gets the data as an <see cref="WiaImageFile"/> with default width and height.
		/// </summary>
		/// <returns>WIA image file.</returns>
		public WiaImageFile GetImageFile()
		{
			return GetImageFile(0, 0);
		}

		/// <summary>
		/// Gets the data as an <see cref="WiaImageFile"/>.
		/// </summary>
		/// <param name="width">Image width or zero for default.</param>
		/// <param name="height">Image height or zero for default.</param>
		/// <returns>WIA image file.</returns>
		public WiaImageFile GetImageFile(int width, int height)
		{
			return new WiaImageFile(_wiaVector.get_ImageFile(width, height));
		}

		/// <summary>
		/// Gets the data as a picture with default width and height.
		/// </summary>
		/// <returns>Picture.</returns>
		public object GetPicture()
		{
			return GetPicture(0, 0);
		}

		/// <summary>
		/// Gets the data as a picture.
		/// </summary>
		/// <param name="width">Image width or zero for default.</param>
		/// <param name="height">Image height or zero for default.</param>
		/// <returns>Picture.</returns>
		public object GetPicture(int width, int height)
		{
			return _wiaVector.get_Picture(width, height);
		}

		/// <summary>
		/// Gets the data as Unicode string.
		/// </summary>
		/// <returns>String value.</returns>
		public string GetString()
		{
			return GetString(true);
		}

		/// <summary>
		/// Gets the data as string.
		/// </summary>
		/// <param name="unicode">True when the string bytes are Unicode.</param>
		/// <returns>String value.</returns>
		public string GetString(bool unicode)
		{
			return _wiaVector.get_String(unicode);
		}

		/// <summary>
		/// Gets the zero-based index of a member.
		/// </summary>
		/// <param name="value">Value to find.</param>
		/// <returns>Index or -1 when it doesn't exist.</returns>
		public int IndexOf(object value)
		{
			// Search all items (1 based array)
			for (var i = 1; i <= _wiaVector.Count; i++)
			{
				object? existingItem = null;
				_wiaVector.let_Item(i, ref existingItem);
				if (existingItem == value)
					return i;
			}

			// Not found
			return -1;
		}

		/// <summary>
		/// Inserts a value at the specified index.
		/// </summary>
		/// <param name="index">Index to insert at, zero based.</param>
		/// <param name="value">Value to insert.</param>
		public void Insert(int index, object value)
		{
			_wiaVector.Add(value, index + 1);
		}

		/// <summary>
		/// Removes an item.
		/// </summary>
		/// <param name="value">Item to remove.</param>
		public void Remove(object value)
		{
			// Get index of item
			var index = IndexOf(value);
			if (index >= 0)
			{
				// Remove item when found
				_wiaVector.Remove(index + 1);
			}
		}

		/// <summary>
		/// Removes an item at the specified index.
		/// </summary>
		/// <param name="index">Zero based index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			_wiaVector.Remove(index + 1);
		}

		/// <summary>
		/// Sets the binary data.
		/// </summary>
		public void SetBinary(ref object value)
		{
			_wiaVector.set_BinaryData(ref value);
		}

		/// <summary>
		/// Sets the data from a string as re-sizable Unicode.
		/// </summary>
		/// <param name="value">String value.</param>
		public void SetString(string value)
		{
			SetString(value, true, true);
		}

		/// <summary>
		/// Sets the data from a string.
		/// </summary>
		/// <param name="value">String value.</param>
		/// <param name="resizable">True if the string buffer can be re-sized.</param>
		/// <param name="unicode">True when the string requires Unicode.</param>
		public void SetString(string value, bool resizable, bool unicode)
		{
			_wiaVector.SetFromString(value, resizable, unicode);
		}

		#endregion Public Methods
	}
}
