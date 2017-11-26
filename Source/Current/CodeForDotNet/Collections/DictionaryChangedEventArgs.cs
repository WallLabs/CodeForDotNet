using CodeForDotNet.Collections;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Event arguments for the <see cref="ObservableDictionary{TKey,TValue}.DictionaryChanged"/> event.
    /// </summary>
    public class DictionaryChangedEventArgs<TKey, TValue> : CollectionChangedEventArgs<TValue>
    {
        /// <summary>
        /// Creates an instance with specific values.
        /// </summary>
        public DictionaryChangedEventArgs(CollectionChange change, TKey key, TValue value)
            : base(change, value)
        {
            Key = key;
        }

        /// <summary>
        /// Changed entry key.
        /// </summary>
        public TKey Key { get; private set; }
    }
}