using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Implementation of IObservableMap that supports re-entrance for use as a default view model.
    /// </summary>
    public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        #endregion Lifetime

        #region Fields

        /// <summary>
        /// Underlying dictionary which this class makes observable.
        /// </summary>
        private readonly Dictionary<TKey, TValue> _dictionary;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Looks-up an entry by key.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                // Return current value
                return _dictionary[key];
            }
            set
            {
                // Do nothing when not changed
                var current = _dictionary[key];
                if (Equals(current, value))
                    return;

                // Set new value and fire event when changed
                _dictionary[key] = value;
                OnDictionaryChanged(CollectionChange.Replace, key, value);
            }
        }

        /// <summary>
        /// Key values.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        /// <summary>
        /// Entry values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        /// <summary>
        /// Number of entries.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// Indicates whether this is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a new entry using the specified key and value.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (!_dictionary.ContainsKey(key))
            {
                // Add new entry
                _dictionary.Add(key, value);
                OnDictionaryChanged(CollectionChange.Add, key, value);
            }
            else
            {
                // Update existing entry if changed (fires event)
                this[key] = value;
            }
        }

        /// <summary>
        /// Adds a key pair entry.
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes an entry by key.
        /// </summary>
        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out TValue value))
            {
                OnDictionaryChanged(CollectionChange.Remove, key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an entry.
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.TryGetValue(item.Key, out TValue currentValue) &&
                Equals(item.Value, currentValue) && _dictionary.Remove(item.Key))
            {
                OnDictionaryChanged(CollectionChange.Remove, item.Key, item.Value);
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
            _dictionary.Clear();

            // Fire event
            OnDictionaryChanged(CollectionChange.Reset, default(TKey), default(TValue));
        }

        /// <summary>
        /// Checks if the current key exists in this collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets an entry's value by key if it exists.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Checks if the entry exists in this collection.
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for this collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Copies entries from this collection to an array.
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            var arraySize = array.Length;
            foreach (var pair in _dictionary)
            {
                if (arrayIndex >= arraySize) break;
                array[arrayIndex++] = pair;
            }
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Fired when a change occurs.
        /// </summary>
        public event DictionaryChangedEventHandler<TKey, TValue> DictionaryChanged;

        /// <summary>
        /// Fires the <see cref="DictionaryChanged"/> event.
        /// </summary>
        protected virtual void OnDictionaryChanged(CollectionChange change, TKey key, TValue value)
        {
            DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs<TKey, TValue>(change, key, value));
        }

        #endregion Events
    }
}