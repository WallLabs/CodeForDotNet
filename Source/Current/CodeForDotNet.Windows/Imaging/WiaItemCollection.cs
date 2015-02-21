using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.Items"/>.
    /// </summary>
    public class WiaItemCollection : ICollection<WiaItem>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaItemCollection(Interop.Wia.Items items)
        {
            _wiaItems = items;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.Items"/>.
        /// </summary>
        readonly Interop.Wia.Items _wiaItems;

        #endregion

        #region ICollection Members

        /// <summary>
        /// Indicates whether this list is read-only.
        /// Returns false because WIA items can be added at runtime.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Not implemented, use the <see cref="Add(string, int)"/> overload.
        /// </summary>
        public void Add(WiaItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        public void Add(string name, int flags)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            while (Count > 0)
                Remove(0);
        }

        /// <summary>
        /// Checks if the member exists.
        /// </summary>
        public bool Contains(WiaItem item)
        {
            // Search all items (1 based array)
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == item)
                        return true;
                }

                // Not found
                return false;
            }
        }

        /// <summary>
        /// Copies items from this collection to an array.
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="arrayIndex">Target zero based index.</param>
        public void CopyTo(WiaItem[] array, int arrayIndex)
        {
            // Validate
            if (array == null) throw new ArgumentNullException("array");

            // Copy
            for (int i = 1; i <= _wiaItems.Count; i++)
            {
                object item = _wiaItems[i];
                array.SetValue(item, arrayIndex + i - 1);
            }
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        public void Remove(int index)
        {
            _wiaItems.Remove(index + 1);
        }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public bool Remove(WiaItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count { get { return _wiaItems.Count; } }

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
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        public IEnumerator<WiaItem> GetEnumerator()
        {
            return new WiaItemEnumerator(_wiaItems.GetEnumerator());
        }

        #endregion
    }

    /// <summary>
    /// Managed enumerator for <see cref="Interop.Wia.Item" /> items.
    /// </summary>
    public class WiaItemEnumerator : IEnumerator<WiaItem>
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaItemEnumerator(IEnumerator itemEnumerator)
        {
            _wiaItemEnumerator = itemEnumerator;
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaItemEnumerator()
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
            // Nothing to release
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// WIA item enumerator.
        /// </summary>
        readonly IEnumerator _wiaItemEnumerator;

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Item at the current position.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Item at the current position.
        /// </summary>
        public WiaItem Current
        {
            get
            {
                var item = (Interop.Wia.Item)_wiaItemEnumerator.Current;
                return new WiaItem(item);
            }
        }

        /// <summary>
        /// Moves to the next item if available.
        /// </summary>
        /// <returns>True when moved, false when no more items exist.</returns>
        public bool MoveNext()
        {
            return _wiaItemEnumerator.MoveNext();
        }

        /// <summary>
        /// Moves to the beginning of the collection.
        /// </summary>
        public void Reset()
        {
            _wiaItemEnumerator.Reset();
        }

        #endregion
    }
}
