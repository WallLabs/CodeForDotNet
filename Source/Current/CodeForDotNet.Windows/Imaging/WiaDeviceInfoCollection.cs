using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.DeviceInfos"/>.
    /// </summary>
    public class WiaDeviceInfoCollection : ICollection<WiaDeviceInfo>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceInfoCollection(Interop.Wia.DeviceInfos deviceInfos)
        {
            _wiaDeviceInfos = deviceInfos;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Interop.Wia.DeviceInfos"/>.
        /// </summary>
        readonly Interop.Wia.DeviceInfos _wiaDeviceInfos;

        #endregion

        #region ICollection Members

        /// <summary>
        /// Indicates whether this list is read-only.
        /// Returns true because WIA device information entries cannot be added (they are provided by the driver).
        /// </summary>
        public bool IsReadOnly { get { return true; } }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public void Add(WiaDeviceInfo item)
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
        public bool Contains(WiaDeviceInfo item)
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
        public void CopyTo(WiaDeviceInfo[] array, int arrayIndex)
        {
            // Validate
            if (array == null) throw new ArgumentNullException("array");

            // Copy
            for (int i = 1; i <= _wiaDeviceInfos.Count; i++)
            {
                object item = _wiaDeviceInfos[i];
                array.SetValue(item, arrayIndex + i - 1);
            }
        }

        /// <summary>
        /// Not implemented, this collection is read-only.
        /// </summary>
        public bool Remove(WiaDeviceInfo item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count { get { return _wiaDeviceInfos.Count; } }

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
        public IEnumerator<WiaDeviceInfo> GetEnumerator()
        {
            return new WiaDeviceInfoEnumerator(_wiaDeviceInfos.GetEnumerator());
        }

        #endregion
    }

    /// <summary>
    /// Managed enumerator for <see cref="Interop.Wia.DeviceInfo" /> items.
    /// </summary>
    public class WiaDeviceInfoEnumerator : IEnumerator<WiaDeviceInfo>
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaDeviceInfoEnumerator(IEnumerator deviceInfoEnumerator)
        {
            _wiaDeviceInfoEnumerator = deviceInfoEnumerator;
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaDeviceInfoEnumerator()
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
        readonly IEnumerator _wiaDeviceInfoEnumerator;

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Item at the current position.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Item at the current position.
        /// </summary>
        public WiaDeviceInfo Current
        {
            get
            {
                var property = (Interop.Wia.DeviceInfo)_wiaDeviceInfoEnumerator.Current;
                return new WiaDeviceInfo(property);
            }
        }

        /// <summary>
        /// Moves to the next item if available.
        /// </summary>
        /// <returns>True when moved, false when no more items exist.</returns>
        public bool MoveNext()
        {
            return _wiaDeviceInfoEnumerator.MoveNext();
        }

        /// <summary>
        /// Moves to the beginning of the collection.
        /// </summary>
        public void Reset()
        {
            _wiaDeviceInfoEnumerator.Reset();
        }

        #endregion
    }
}
