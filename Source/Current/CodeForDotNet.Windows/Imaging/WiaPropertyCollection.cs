using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.Properties"/>.
    /// </summary>
    public class WiaPropertyCollection : ICollection<WiaProperty>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaPropertyCollection(Interop.Wia.Properties properties)
        {
            _wiaProperties = properties;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.Properties"/>.
        /// </summary>
        Interop.Wia.Properties _wiaProperties;

        #endregion

        #region ICollection Members

        /// <summary>
        /// Indicates whether this list is read-only.
        /// Returns true because WIA properties cannot be added (they are provided by the driver).
        /// </summary>
        public bool IsReadOnly { get { return true; } }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public void Add(WiaProperty item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the member exists.
        /// </summary>
        public bool Contains(WiaProperty item)
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
        public void CopyTo(WiaProperty[] array, int arrayIndex)
        {
            // Validate
            if (array == null) throw new ArgumentNullException("array");

            // Copy
            for (int i = 1; i <= _wiaProperties.Count; i++)
            {
                object item = _wiaProperties[i];
                array.SetValue(item, arrayIndex + i - 1);
            }
        }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public bool Remove(WiaProperty item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count { get { return _wiaProperties.Count; } }

        /// <summary>
        /// Indicates that the collection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized { get { return true; } }

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }
        static object _syncRoot = new object();

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
        public IEnumerator<WiaProperty> GetEnumerator()
        {
            return new WiaPropertyEnumerator(_wiaProperties.GetEnumerator());
        }

        #endregion
    }

    /// <summary>
    /// Managed enumerator for <see cref="Interop.Wia.Property" /> items.
    /// </summary>
    public class WiaPropertyEnumerator : IEnumerator<WiaProperty>
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaPropertyEnumerator(IEnumerator propertyEnumerator)
        {
            _wiaPropertyEnumerator = propertyEnumerator;
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaPropertyEnumerator()
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
        void Dispose(bool disposing)
        {
            // Nothing to release
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// WIA property enumerator.
        /// </summary>
        IEnumerator _wiaPropertyEnumerator;

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Item at the current position.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Item at the current position.
        /// </summary>
        public WiaProperty Current 
        { 
            get
            {
                var property = (Interop.Wia.Property)_wiaPropertyEnumerator.Current;
                return new WiaProperty(property);
            }
        }

        /// <summary>
        /// Moves to the next item if available.
        /// </summary>
        /// <returns>True when moved, false when no more items exist.</returns>
        public bool MoveNext()
        {
            return _wiaPropertyEnumerator.MoveNext();
        }

        /// <summary>
        /// Moves to the beginning of the collection.
        /// </summary>
        public void Reset()
        {
            _wiaPropertyEnumerator.Reset();
        }

        #endregion
    }
}
