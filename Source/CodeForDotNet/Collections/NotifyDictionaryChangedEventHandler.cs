namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Represents the method that handles the changed event of an observable dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam><typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public delegate void NotifyDictionaryChangedEventHandler<TKey, TValue>(INotifyDictionaryChanged<TKey, TValue> sender, NotifyDictionaryChangedEventArgs<TKey, TValue> args);
}