using CodeForDotNet.Collections;
using System;
using Windows.Foundation.Collections;
using CollectionChange = CodeForDotNet.Collections.CollectionChange;

namespace CodeForDotNet.WindowsUniversal.Collections
{
    /// <summary>
    /// Implementation of IObservableMap that supports re-entrance for use as a default view model.
    /// </summary>
    public class ObservableMap<TKey, TValue> : ObservableDictionary<TKey, TValue>, IObservableMap<TKey, TValue>
    {
        #region Events

        /// <summary>
        /// Fired when the dictionary changes.
        /// </summary>
        [CLSCompliant(false)]
        public event MapChangedEventHandler<TKey, TValue> MapChanged;

        #endregion

        #region Events

        /// <summary>
        /// Fires the <see cref="ObservableDictionary{TKey,TValue}.DictionaryChanged"/> and <see cref="MapChanged"/> events when a change occurs.
        /// </summary>
        protected override void OnDictionaryChanged(CollectionChange change, TKey key, TValue value)
        {
            // Fire base dictionary event
            base.OnDictionaryChanged(change, key, value);

            // Fire map changed event
            // The portable base dictionary change value matches the Windows Store collection value
            MapChanged?.Invoke(this, new MapChangedEventArgs<TKey>((global::Windows.Foundation.Collections.CollectionChange)(int)change, key));
        }

        #endregion
    }
}
