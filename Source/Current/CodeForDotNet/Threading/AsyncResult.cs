using System;
using System.Threading;
using CodeForDotNet.Properties;

namespace CodeForDotNet.Threading
{
    /// <summary>
    /// <see cref="IAsyncResult"/> implementation.
    /// </summary>
    public abstract class AsyncResult : DisposableObject, IAsyncResult
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected AsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            AsyncState = state;
            Lock = new object();
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Dispose only once
            if (IsDisposed)
                return;

            // Dispose
            try
            {
                // Free resources during dispose
                if (disposing)
                {
                    if (_waitHandle != null)
                    {
                        _waitHandle.Dispose();
                        _waitHandle = null;
                    }
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Optional callback method, invoked upon completion.
        /// </summary>
        readonly AsyncCallback _callback;

        /// <summary>
        /// Indicates that <see cref="End{TAsyncResult}"/> has already been called.
        /// </summary>
        bool _endCalled;

        /// <summary>
        /// Stores any error which occurred during completion.
        /// </summary>
        Exception _exception;

        /// <summary>
        /// Handle used to wait for completion.
        /// </summary>
        ManualResetEvent _waitHandle;

        #endregion

        #region Public Properties

        /// <summary>
        /// Optional state relating to the operation.
        /// </summary>
        public object AsyncState { get; private set; }

        /// <summary>
        /// Wait handle.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                // Check if handle exists (double-check lock for performance)
                if (_waitHandle == null)
                {
                    // Lock then...
                    lock (Lock)
                    {
                        // ...double check if still doesn't exist
                        if (_waitHandle == null)
                        {
                            // Create handle first time
                            var manualResetEvent = new ManualResetEvent(IsCompleted);

                            // Avoid multi-processor issues with double-check locking assignment
                            Interlocked.MemoryBarrier();

                            // Assign handle, processor safe
                            _waitHandle = manualResetEvent;
                        }
                    }
                }

                // Return current handle
                return _waitHandle;
            }
        }

        /// <summary>
        /// Indicates the action was completed synchronously.
        /// </summary>
        public bool CompletedSynchronously { get; private set; }

        /// <summary>
        /// Indicates a completion callback is present.
        /// </summary>
        public bool HasCallback { get { return _callback != null; } }

        /// <summary>
        /// Indicates the operation has completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Locking object.
        /// </summary>
        protected object Lock { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Completes the operation.
        /// </summary>
        /// <param name="completedSynchronously">True when completed synchronously.</param>
        protected void Complete(bool completedSynchronously)
        {
            // Ensure only called once
            if (IsCompleted)
                throw new InvalidOperationException(Resources.AsyncCompleteCalledTwice);

            // Set synchronous flag as specified
            CompletedSynchronously = completedSynchronously;

            // Set complete
            if (completedSynchronously)
            {
                // If we completedSynchronously, no wait handle was created so
                // we don't need to worry about a race
                IsCompleted = true;
            }
            else
            {
                // Set completed during asychronous operation
                lock (Lock)
                {
                    IsCompleted = true;
                    if (_waitHandle != null)
                        _waitHandle.Set();
                }
            }

            // Execute callback when specified
            if (_callback != null)
            {
                try
                {
                    // Invoke caller's specified callback to notify end
                    _callback(this);
                }
                catch (Exception e)
                {
                    // Throw callback errors directly (the caller has a problem, this error is not part of the result)
                    throw new InvalidOperationException("Async callback threw an Exception", e);
                }
            }
        }

        /// <summary>
        /// Completes the operation with an error.
        /// </summary>
        /// <param name="completedSynchronously">True when completed synchronously.</param>
        /// <param name="exception">Error.</param>
        protected void Complete(bool completedSynchronously, Exception exception)
        {
            // Set error
            _exception = exception;

            // Complete
            Complete(completedSynchronously);
        }

        /// <summary>
        /// Ends the operation.
        /// </summary>
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            // Validate
            if (result == null)
                throw new ArgumentNullException("result");
            var asyncResult = result as TAsyncResult;
            if (asyncResult == null)
                throw new ArgumentOutOfRangeException("result");

            // Ensure end is only called once
            if (asyncResult._endCalled)
                throw new InvalidOperationException(Resources.AsyncEndCalledTwice);
            asyncResult._endCalled = true;

            // Wait for completion if not finished
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne();

            // Free resources
            if (asyncResult._waitHandle != null)
            {
                asyncResult._waitHandle.Dispose();
                asyncResult._waitHandle = null;
            }

            // Throw exception when failed
            if (asyncResult._exception != null)
                throw asyncResult._exception;

            // Return result when successful
            return asyncResult;
        }

        #endregion
    }
}
