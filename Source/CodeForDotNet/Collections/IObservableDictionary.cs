using System.Collections.Generic;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Notifies listeners of dynamic changes to a dictionary, such as when items are added or removed.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Occurs when the dictionary changes.
        /// </summary>
        event DictionaryChangedEventHandler<TKey, TValue> DictionaryChanged;
    }
}
