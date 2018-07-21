namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Represents the method that handles the changed event of an observable collection.
    /// </summary>
    /// <typeparam name="T">The type of the values in the collection.</typeparam>
    public delegate void CollectionChangedEventHandler<T>(IObservableCollection<T> sender, CollectionChangedEventArgs<T> args);
}
