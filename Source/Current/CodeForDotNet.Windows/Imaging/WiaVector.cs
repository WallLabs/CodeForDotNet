using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.Vector"/>.
    /// </summary>
    public class WiaVector : IList
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaVector(Wia.Vector vector)
        {
            _wiaVector = vector;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Wia.Vector"/>.
        /// </summary>
        readonly Wia.Vector _wiaVector;

        #endregion

        #region Public Properties

        /// <summary>
        /// Date.
        /// </summary>
        public DateTime Date
        {
            get { return _wiaVector.Date; }
            set { _wiaVector.Date = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the binary data.
        /// </summary>
        public byte[] GetBinary()
        {
            return (byte[])_wiaVector.get_BinaryData();
        }

        /// <summary>
        /// Sets the binary data.
        /// </summary>
        public void SetBinary(ref object value)
        {
            _wiaVector.set_BinaryData(ref value);
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

        #endregion

        #region IList Members

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
            object missing = null;
            _wiaVector.set_BinaryData(ref missing);
        }

        /// <summary>
        /// Checks if the member exists.
        /// </summary>
        public bool Contains(object value)
        {
            // Search all items (1 based array)
            for (int i = 1; i <= _wiaVector.Count; i++)
            {
                object existingItem = null;
                _wiaVector.let_Item(i, ref existingItem);
                if (existingItem == value)
                    return true;
            }

            // Not found
            return false;
        }

        /// <summary>
        /// Gets the zero-based index of a member.
        /// </summary>
        /// <param name="value">Value to find.</param>
        /// <returns>Index or -1 when it doesn't exist.</returns>
        public int IndexOf(object value)
        {
            // Search all items (1 based array)
            for (int i = 1; i <= _wiaVector.Count; i++)
            {
                object existingItem = null;
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
        /// Indicates whether this list is a fixed size.
        /// Returns false because WIA vectors are variable.
        /// </summary>
        public bool IsFixedSize { get { return false; } }

        /// <summary>
        /// Indicates whether this list is read-only.
        /// Returns false because WIA vectors are writable.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="value">Item to remove.</param>
        public void Remove(object value)
        {
            // Get index of item
            int index = IndexOf(value);
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
        /// Gets or sets an item at the specified index.
        /// </summary>
        /// <param name="index">Zero based index</param>
        /// <returns>Item value at the specified index.</returns>
        public object this[int index]
        {
            get
            {
                object item = null;
                _wiaVector.let_Item(index + 1, ref item);
                return item;
            }
            set
            {
                _wiaVector.Remove(index + 1);
                Insert(index, value);
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies items from this collection to an array.
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="index">Target zero based index.</param>
        public void CopyTo(WiaVector array, int index)
        {
            // Validate
            if (array == null) throw new ArgumentNullException("array");

            // Copy
            for (int i = 1; i <= _wiaVector.Count; i++)
            {
                object item = null;
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
            if (array == null) throw new ArgumentNullException("array");

            // Copy
            for (int i = 1; i <= _wiaVector.Count; i++)
            {
                object item = null;
                _wiaVector.let_Item(index + 1, ref item);
                array.SetValue(item, index + i - 1);
            }
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count { get { return _wiaVector.Count; } }

        /// <summary>
        /// Indicates that the collection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized { get { return true; } }

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }
        static readonly object _syncRoot = new object();

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _wiaVector.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Managed enumerator for <see cref="Wia.Vector" /> items.
    /// </summary>
    public class WiaVectorEnumerator : IEnumerator<WiaVector>
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaVectorEnumerator(IEnumerator vectorEnumerator)
        {
            _wiaVectorEnumerator = vectorEnumerator;
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaVectorEnumerator()
        {
            // Unmanaged only dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalization as no longer necessary
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Frees resources owned by this object.
        /// </summary>
        /// <param name="disposing">True when called via <see cref="Dispose()"/>.</param>
        private void Dispose(bool disposing)
        {
            // Release unmanaged resources
            Marshal.ReleaseComObject(_wiaVectorEnumerator);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// WIA vector enumerator.
        /// </summary>
        readonly IEnumerator _wiaVectorEnumerator;

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Item at the current position.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Item at the current position.
        /// </summary>
        public WiaVector Current
        {
            get
            {
                var vector = (Wia.Vector)_wiaVectorEnumerator.Current;
                return new WiaVector(vector);
            }
        }

        /// <summary>
        /// Moves to the next item if available.
        /// </summary>
        /// <returns>True when moved, false when no more items exist.</returns>
        public bool MoveNext()
        {
            return _wiaVectorEnumerator.MoveNext();
        }

        /// <summary>
        /// Moves to the beginning of the collection.
        /// </summary>
        public void Reset()
        {
            _wiaVectorEnumerator.Reset();
        }

        #endregion
    }
}
