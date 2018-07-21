using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Generic List which fires change notification events.
    /// </summary>
    public class EventedCollection<T> : Collection<T>
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public EventedCollection()
        {
        }

        /// <summary>
        /// Creates an instance based on an existing collection.
        /// </summary>
        public EventedCollection(IList<T> list)
            : base(list)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replaces the Add method in order to fire the Added and Changed events.
        /// </summary>
        public new void Add(T item)
        {
            // Call base class implementation to add item to list
            base.Add(item);

            // Fire events
            Added?.Invoke(this, EventArgs.Empty);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Replaces the Remove method in order to fire the Removed and Changed events.
        /// </summary>
        public new bool Remove(T item)
        {
            // Call base class implementation to remove item from list
            bool result = base.Remove(item);

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
        /// Fired when an item is added to the list.
        /// </summary>
        public event EventHandler Added;

        /// <summary>
        /// Fired when the list is changed (an item is added or removed).
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Fired when an item is removed from the list.
        /// </summary>
        public event EventHandler Removed;

        #endregion
    }
}
