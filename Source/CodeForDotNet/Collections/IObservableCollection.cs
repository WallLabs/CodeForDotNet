using System.Collections.Generic;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Notifies listeners of dynamic changes to a collection, such as when items are added or removed.
    /// </summary>
    /// <typeparam name="T">The type of the values in the collection.</typeparam>
    public interface IObservableCollection<T> : ICollection<T>
    {
        /// <summary>
        /// Occurs when the Collection changes.
        /// </summary>
        event CollectionChangedEventHandler<T> CollectionChanged;
    }
}
