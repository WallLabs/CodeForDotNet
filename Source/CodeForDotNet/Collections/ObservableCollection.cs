using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Portable observable collection.
    /// </summary>
    public class ObservableCollection<T> : IObservableCollection<T>, IList<T>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public ObservableCollection()
        {
            _collection = new Collection<T>();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Underlying Collection which this class makes observable.
        /// </summary>
        private readonly Collection<T> _collection;
        
        #endregion

        #region Properties

        /// <summary>
        /// Looks-up an entry by key.
        /// </summary>
        public T this[int index]
        {
            get
            {
                // Return current value
                return _collection[index];
            }
            set
            {
                // Do nothing when not changed
                var current = _collection[index];
                if (Equals(current, value))
                    return;

                // Set new value and fire event when changed
                _collection[index] = value;
                OnCollectionChanged(CollectionChange.Replace, value);
            }
        }

        /// <summary>
        /// Number of entries.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        /// Indicates whether this is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the index of an item.
        /// </summary>
        public int IndexOf(T item)
        {
            return _collection.IndexOf(item);
        }

        /// <summary>
        /// Inserts a new item at the specified index.
        /// </summary>
        public void Insert(int index, T item)
        {
            _collection.Insert(index, item);
        }

        /// <summary>
        /// Removes an item at a specific index.
        /// </summary>
        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        /// <summary>
        /// Adds a new entry using the specified key and value.
        /// </summary>
        public void Add(T item)
        {
            if (!_collection.Contains(item))
            {
                // Add new entry
                _collection.Add(item);
                OnCollectionChanged(CollectionChange.Add, item);
            }
        }

        /// <summary>
        /// Removes an entry by key.
        /// </summary>
        public bool Remove(T item)
        {
            if (_collection.Remove(item))
            {
                OnCollectionChanged(CollectionChange.Remove, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all entries.
        /// </summary>
        public void Clear()
        {
            // Clear entries
            _collection.Clear();

            // Fire event
            OnCollectionChanged(CollectionChange.Reset, default(T));
        }

        /// <summary>
        /// Checks if the entry exists in this collection.
        /// </summary>
        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Copies entries from this collection to an array.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            var arraySize = array.Length;
            foreach (var pair in _collection)
            {
                if (arrayIndex >= arraySize) break;
                array[arrayIndex++] = pair;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a change occurs.
        /// </summary>
        public event CollectionChangedEventHandler<T> CollectionChanged;

        /// <summary>
        /// Fires the <see cref="CollectionChanged"/> event.
        /// </summary>
        protected virtual void OnCollectionChanged(CollectionChange change, T value)
        {
            CollectionChanged?.Invoke(this, new CollectionChangedEventArgs<T>(change, value));
        }

        #endregion
    }
}
