using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet
{
    /// <summary>
    /// Provides a base object which provides greater control over object lifetime and memory management.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063", Justification = "This is the correct disposable implementation.")]
    public abstract class DisposableObject : IDisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Overrides the finalizer to ensure any available dispose logic is called.
        /// </summary>
        ~DisposableObject()
        {
            // Dispose only once
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;

            // Dispose only unmanaged resources
            Dispose(false);
        }

        /// <summary>
        /// Frees all resources held by this object, immediately (rather than waiting for Garbage Collection).
        /// This can provide a performance increase and avoid memory congestion on objects that hold
        /// many or "expensive" (large) external resources.
        /// </summary>
        /// <remarks>
        /// First fires the Disposing event, which could cancel this operation.
        /// When not canceled, calls the <see cref="Dispose(bool)"/> method,
        /// which should be overridden in inheriting classes to release
        /// their local resources.
        /// Finally fires the Disposed event.
        /// Use the IsDisposing and IsDisposed properties to avoid using objects
        /// that are about to or have been disposed.
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

        /// <summary>
        /// Inheritors implement the <see cref="Dispose(bool)"/> method to dispose resources accordingly,
        /// depending on whether they have been called proactively or automatically via
        /// the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// True when called proactively, i.e. Not during garbage collection.
        /// Managed resources should not be accessed when this is False,
        /// just references and unmanaged resources released.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Set second flag to indicate disposed state
            IsDisposed = true;

            // Fire Disposed event
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion Lifetime

        #region Public Properties

        /// <summary>
        /// Indicates that this object is committed to the process of disposing.
        /// When this flag is true, do not pass any references or queue it for processing.
        /// </summary>
        public bool IsDisposing { get; private set; }

        /// <summary>
        /// Indicated that this object has been disposed.
        /// When this flag is true, do not use this object in any way.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion Public Properties

        #region Events

        /// <summary>
        /// Fires when the Dispose method is called on this object (except when garbage collected).
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Fires after this object has been Disposed.
        /// Use this event to ensure all references are invalidated and any dependent objects are also Disposed
        /// or released.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events
    }
}