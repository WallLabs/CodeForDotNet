using System;
using System.Collections.Generic;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Generic dictionary which fires change notification events.
    /// </summary>
    public class EventedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public EventedDictionary() { }

        /// <summary>
        /// Creates an instance with the specified capacity.
        /// </summary>
        public EventedDictionary(int capacity) : base(capacity) { }

        /// <summary>
        /// Creates an instance with the specified comparer.
        /// </summary>
        public EventedDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        /// <summary>
        /// Creates an instance based on an existing dictionary.
        /// </summary>
        public EventedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        /// <summary>
        /// Creates an instance with the specified capacity and comparer.
        /// </summary>
        public EventedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        /// <summary>
        /// Creates an instance based on an existing dictionary.
        /// </summary>
        public EventedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        #region IDisposable Members

        /// <summary>
        /// Overrides the finalizer to ensure any available dispose logic is called.
        /// </summary>
        ~EventedDictionary()
        {
            // Dispose only once
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;

            // Dispose only un-managed resources
            Dispose(false);
        }

        /// <summary>
        /// Frees all resources held by this object, immediately (rather than waiting for Garbage Collection).
        /// This can provide a performance increase and avoid memory congestion on objects that hold
        /// many or "expensive" (large) external resources.
        /// </summary>
        /// <remarks>
        /// First fires the Disposing event, which could cancel this operation.
        /// When not cancelled, calls the Dispose(bool notFinalized) method, which should overrided in inheriting
        /// classes to release their local resources.
        /// Finally fires the Disposed event.
        /// Use the IsDisposing and IsDisposed properties to avoid using objects that are about to or have been disposed.
        /// </remarks>
        public void Dispose()
        {
            // Dispose only once
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;

            // Full managed dispose
            Dispose(true);

            // Do not finalize
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Inheritors implement the Dipose(bool) method to dispose resources accordingly,
        /// depending on whether they have been called proactively or automatically via
        /// the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// True when called proactively, i.e. Not during garbage collection.
        /// Managed resources should not be accessed when this is False,
        /// just references and unmangaed resources released.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Set second flag to indicate Disposed state
            IsDisposed = true;
        }

        /// <summary>
        /// Indicates that this object is committed to the process of Disposing.
        /// When this flag is TRUE, do not pass any references or queue it for processing.
        /// </summary>
        public bool IsDisposing { get; private set; }

        /// <summary>
        /// Indicated that this object has been Disposed.
        /// When this flag is TRUE, do not use this object in any way.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Replaces the Add method in order to fire the Added and Changed events.
        /// </summary>
        public new void Add(TKey key, TValue value)
        {
            // Call base class implementation to add item to dictionary
            base.Add(key, value);

            // Fire events
            Added?.Invoke(this, EventArgs.Empty);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Replaces the Remove method in order to fire the Remove and Changed events.
        /// </summary>
        public new bool Remove(TKey key)
        {
            // Call base class implementation to remove item from dictionary
            bool result = base.Remove(key);

            // Fire events (if removed)
            if (result)
            {
                Removed?.Invoke(this, EventArgs.Empty);
                Changed?.Invoke(this, EventArgs.Empty);
            }

            // Return result
            return result;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when an item is added.
        /// </summary>
        public event EventHandler Added;

        /// <summary>
        /// Fired when the dictionary is changed (added or removed).
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Fired when an item is removed.
        /// </summary>
        public event EventHandler Removed;

        #endregion
    }
}
