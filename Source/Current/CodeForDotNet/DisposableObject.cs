using System;

namespace CodeForDotNet
{
    /// <summary>
    /// Provides a base object which provides greater control over object lifetime and memory management.
    /// </summary>
    public abstract class DisposableObject : IDisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Overrides the finalizer to ensure any available dispose logic is called.
        /// </summary>
        ~DisposableObject()
        {
            // Dispose only once
            if (_isDisposing || _isDisposed)
                return;
            _isDisposing = true;

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
            if (_isDisposing || _isDisposed)
                return;
            _isDisposing = true;
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
            // Set second flag to indicate Disposed state
            _isDisposed = true;

            // Fire Disposed event
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates that this object is committed to the process of Disposing.
        /// When this flag is true, do not pass any references or queue it for processing.
        /// </summary>
        public bool IsDisposing
        {
            get
            {
                return _isDisposing;
            }
        }
        bool _isDisposing;

        /// <summary>
        /// Indicated that this object has been Disposed.
        /// When this flag is true, do not use this object in any way.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }
        bool _isDisposed;

        #endregion

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

        #endregion
    }
}
