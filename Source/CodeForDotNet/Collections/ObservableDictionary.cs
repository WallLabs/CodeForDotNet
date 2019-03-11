using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CodeForDotNet.Collections
{
   /// <summary>
   /// Implementation of IObservableMap that supports re-entrance for use as a default view model.
   /// </summary>
   [Serializable]
   public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        INotifyDictionaryChanged<TKey, TValue>, INotifyPropertyChanged, ISerializable
   {
      #region Private Fields

      /// <summary>
      /// Underlying dictionary which this class makes observable.
      /// </summary>
      private readonly Dictionary<TKey, TValue> _dictionary;

      #endregion Private Fields

      #region Public Constructors

      /// <summary>
      /// Creates an instance.
      /// </summary>
      public ObservableDictionary() => _dictionary = new Dictionary<TKey, TValue>();

      #endregion Public Constructors

      #region Protected Constructors

      /// <summary>
      /// Serialization constructor.
      /// </summary>
      [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Unused parameter required by interface.")]
      protected ObservableDictionary(SerializationInfo info, StreamingContext context) => _dictionary = (Dictionary<TKey, TValue>)info.GetValue(nameof(_dictionary), typeof(Dictionary<TKey, TValue>));

      #endregion Protected Constructors

      #region Public Events

      /// <summary>
      /// Fired when a change occurs with arguments containing details about the change.
      /// </summary>
      public event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;

      /// <summary>
      /// Fired when a change occurs.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      #endregion Public Events

      #region Public Properties

      /// <summary>
      /// Number of entries.
      /// </summary>
      public int Count => _dictionary.Count;

      /// <summary>
      /// Indicates whether this is read only.
      /// </summary>
      public bool IsReadOnly => false;

      /// <summary>
      /// Key values.
      /// </summary>
      public ICollection<TKey> Keys => _dictionary.Keys;

      /// <summary>
      /// Entry values.
      /// </summary>
      public ICollection<TValue> Values => _dictionary.Values;

      #endregion Public Properties

      #region Public Indexers

      /// <summary>
      /// Looks-up an entry by key.
      /// </summary>
      public TValue this[TKey key]
      {
         get =>

             // Return current value
             _dictionary[key];
         set
         {
            // Do nothing when not changed
            var current = _dictionary[key];
            if (Equals(current, value))
            {
               return;
            }

            // Set new value and fire event when changed
            _dictionary[key] = value;
            OnDictionaryChanged(NotifyCollectionChangedAction.Replace, key, value);
         }
      }

      #endregion Public Indexers

      #region Public Methods

      /// <summary>
      /// Adds a new entry using the specified key and value.
      /// </summary>
      public void Add(TKey key, TValue value)
      {
         if (!_dictionary.ContainsKey(key))
         {
            // Add new entry
            _dictionary.Add(key, value);
            OnDictionaryChanged(NotifyCollectionChangedAction.Add, key, value);
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
      public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

      /// <summary>
      /// Clears all entries.
      /// </summary>
      public void Clear()
      {
         // Clear entries
         _dictionary.Clear();

         // Fire event
         OnDictionaryChanged(NotifyCollectionChangedAction.Reset, default(TKey), default(TValue));
      }

      /// <summary>
      /// Checks if the entry exists in this collection.
      /// </summary>
      public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

      /// <summary>
      /// Checks if the current key exists in this collection.
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

      /// <summary>
      /// Copies entries from this collection to an array.
      /// </summary>
      public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
      {
         if (array == null)
         {
            throw new ArgumentNullException(nameof(array));
         }

         var arraySize = array.Length;
         foreach (var pair in _dictionary)
         {
            if (arrayIndex >= arraySize)
            {
               break;
            }

            array[arrayIndex++] = pair;
         }
      }

      /// <summary>
      /// Gets an enumerator for this collection.
      /// </summary>
      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

      /// <summary>
      /// Gets an enumerator for this collection.
      /// </summary>
      IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

      /// <summary>
      /// Serialization method.
      /// </summary>
      [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
      public virtual void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue(nameof(_dictionary), _dictionary);

      /// <summary>
      /// Removes an entry by key.
      /// </summary>
      public bool Remove(TKey key)
      {
         if (_dictionary.TryGetValue(key, out var value))
         {
            OnDictionaryChanged(NotifyCollectionChangedAction.Remove, key, value);
            return true;
         }
         return false;
      }

      /// <summary>
      /// Removes an entry.
      /// </summary>
      public bool Remove(KeyValuePair<TKey, TValue> item)
      {
         if (_dictionary.TryGetValue(item.Key, out var currentValue) &&
             Equals(item.Value, currentValue) && _dictionary.Remove(item.Key))
         {
            OnDictionaryChanged(NotifyCollectionChangedAction.Remove, item.Key, item.Value);
            return true;
         }
         return false;
      }

      /// <summary>
      /// Gets an entry's value by key if it exists.
      /// </summary>
      public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

      #endregion Public Methods

      #region Protected Methods

      /// <summary>
      /// Fires the <see cref="DictionaryChanged"/> event.
      /// </summary>
      protected virtual void OnDictionaryChanged(NotifyCollectionChangedAction change, TKey key, TValue value) => DictionaryChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs<TKey, TValue>(change, key, value));

      /// <summary>
      /// Fires the <see cref="PropertyChanged"/> event.
      /// </summary>
      /// <param name="name"></param>
      protected virtual void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

      #endregion Protected Methods
   }
}
