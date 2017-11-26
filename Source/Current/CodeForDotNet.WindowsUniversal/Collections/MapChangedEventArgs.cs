using System;
using Windows.Foundation.Collections;

namespace CodeForDotNet.WindowsUniversal.Collections
{
    /// <summary>
    /// Change event handler for the <see cref="ObservableMap{TKey,TValue}"/>.
    /// </summary>
    [CLSCompliant(false)]
    public class MapChangedEventArgs<TKey> : IMapChangedEventArgs<TKey>
    {
        /// <summary>
        /// Creates an instance with the specified key and change.
        /// </summary>
        /// <param name="change">Change type.</param>
        /// <param name="key">Key of the changed entry, when relevant.</param>
        public MapChangedEventArgs(CollectionChange change, TKey key)
        {
            CollectionChange = change;
            Key = key;
        }

        /// <summary>
        /// Type of change which occurred.
        /// </summary>
        public CollectionChange CollectionChange { get; private set; }

        /// <summary>
        /// Key of the changed item, when relevant.
        /// </summary>
        public TKey Key { get; private set; }
    }
}