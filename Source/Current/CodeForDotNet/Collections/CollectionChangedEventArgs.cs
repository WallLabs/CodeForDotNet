using System;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Event arguments for the <see cref="ObservableCollection{T}.CollectionChanged"/> event.
    /// </summary>
    public class CollectionChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Creates an instance with specific values.
        /// </summary>
        public CollectionChangedEventArgs(CollectionChange change, T value)
        {
            CollectionChange = change;
            Value = value;
        }

        /// <summary>
        /// Change details.
        /// </summary>
        public CollectionChange CollectionChange { get; private set; }

        /// <summary>
        /// Changed entry key.
        /// </summary>
        public T Value { get; private set; }
    }
}