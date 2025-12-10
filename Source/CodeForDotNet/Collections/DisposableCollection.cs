using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeForDotNet.Collections;

/// <summary>
/// Disposable collection of <typeparamref name="T"/> items. Also disposes items when removed or the collection is cleared.
/// </summary>
public class DisposableCollection<T> : Collection<T>, IDisposableObject where T : class
{
    #region Public Constructors

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public DisposableCollection()
    { }

    /// <summary>
    /// Creates an instance based on an existing list.
    /// </summary>
    public DisposableCollection(IList<T> list) : base(list) { }

    #endregion Public Constructors

    #region Private Destructors

    /// <summary>
    /// Overrides the finalizer to ensure any available dispose logic is called.
    /// </summary>
    ~DisposableCollection()
    {
        // Dispose only once
        if (IsDisposing || IsDisposed)
            return;

        IsDisposing = true;

        // Dispose only unmanaged resources
        Dispose(false);
    }

    #endregion Private Destructors

    #region Public Events

    /// <summary>
    /// Fires after this object has been Disposed. Use this event to ensure all references are invalidated and any dependent objects are also Disposed or released.
    /// </summary>
    public event EventHandler? Disposed;

    /// <summary>
    /// Fires when the Dispose method is called on this object (except when garbage collected).
    /// </summary>
    public event EventHandler? Disposing;

    #endregion Public Events

    #region Public Properties

    /// <summary>
    /// Indicated that this object has been Disposed. When this flag is true, do not use this object in any way.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Indicates that this object is committed to the process of Disposing. When this flag is true, do not pass any references or queue it for processing.
    /// </summary>
    public bool IsDisposing { get; private set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Frees all resources held by this object, immediately (rather than waiting for Garbage Collection). This can provide a performance increase and avoid
    /// memory congestion on objects that hold many or "expensive" (large) external resources.
    /// </summary>
    /// <remarks>
    /// First fires the Disposing event, which could cancel this operation. When not canceled, calls the <see cref="Dispose(bool)"/> method, which should be
    /// overridden in inheriting classes to release their local resources. Finally fires the Disposed event. Use the IsDisposing and IsDisposed properties to
    /// avoid using objects that are about to or have been disposed.
    /// </remarks>
    public void Dispose()
    {
        // Dispose only once
        if (IsDisposing || IsDisposed)
            return;

        IsDisposing = true;
        try
        {
            // Fire disposing event
            Disposing?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Do not finalize
                GC.SuppressFinalize(this);
            }
        }
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    /// Disposes then clears all items.
    /// </summary>
    protected override void ClearItems()
    {
        try
        {
            // Dispose items
            foreach (IDisposable item in Items)
                item.Dispose();
        }
        finally
        {
            // Release references
            base.ClearItems();
        }
    }

    /// <summary>
    /// Inheritors implement the <see cref="Dispose(bool)"/> method to dispose resources accordingly, depending on whether they have been called proactively
    /// or automatically via the finalizer.
    /// </summary>
    /// <param name="disposing">
    /// True when called proactively, i.e. Not during garbage collection. Managed resources should not be accessed when this is False, just references and
    /// unmanaged resources released.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        // Set second flag to indicate Disposed state
        IsDisposed = true;
        try
        {
            // Dispose or release references according to dispose type.
            if (disposing)
                ClearItems();
            else
                base.ClearItems();
        }
        finally
        {
            // Fire Disposed event
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Disposes then removes an item.
    /// </summary>
    protected override void RemoveItem(int index)
    {
        try
        {
            var item = (IDisposable)Items[index];
            item?.Dispose();
        }
        finally
        {
            base.RemoveItem(index);
        }
    }

    /// <summary>
    /// Disposes then replaces an item.
    /// </summary>
    protected override void SetItem(int index, T item)
    {
        try
        {
            var oldItem = (IDisposable)Items[index];
            oldItem?.Dispose();
        }
        finally
        {
            base.SetItem(index, item);
        }
    }

    #endregion Protected Methods
}
