using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeForDotNet.Collections
{
    /// <summary>
    /// Disposable collection of <typeparamref name="T"/> items.
    /// Also disposes items when removed or the collection is cleared.
    /// </summary>
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : class
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public DisposableCollection()
        { }

        /// <summary>
        /// Creates an instance based on an existing list.
        /// </summary>
        public DisposableCollection(IList<T> list) : base(list) { }

        #region IDisposable Members

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> during finalization to free resources in case it was forgotten.
        /// </summary>
        ~DisposableCollection()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources.
        /// </summary>
        public void Dispose()
        {
            // Full dispose
            Dispose(true);

            // Suppress finalization as no longer needed
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources owned by this object.
        /// </summary>
        /// <param name="disposing">
        /// True when called proactively by <see cref="Dispose()"/>.
        /// False when called during finalization.
        /// </param>
        void Dispose(bool disposing)
        {
            if (disposing)
                ClearItems();
            else
                base.ClearItems();
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears and disposes items.
        /// </summary>
        protected override void ClearItems()
        {
            try
            {
                // Dispose items
                foreach (var item in Items)
                    ((IDisposable)item).Dispose();
            }
            finally
            {
                // Release references
                base.ClearItems();
            }
        }

        /// <summary>
        /// Removes and disposes an item.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            try
            {
                var item = Items[index];
                if (item != null)
                    ((IDisposable)item).Dispose();
            }
            finally
            {
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// Disposes the replaces an item.
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            try
            {
                var oldItem = Items[index];
                if (oldItem != null)
                    ((IDisposable)oldItem).Dispose();
            }
            finally
            {
                base.SetItem(index, item);
            }
        }

        #endregion
    }
}
