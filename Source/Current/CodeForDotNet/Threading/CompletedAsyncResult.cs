using System;

namespace CodeForDotNet.Threading
{
    /// <summary>
    /// An <see cref="AsyncResult"/> which completes instantly (synchronous result wrapped in <see cref="IAsyncResult"/>).
    /// </summary>
    public class CompletedAsyncResult : AsyncResult
    {
        /// <summary>
        /// Creates an instance with the specified callback and state,
        /// then completes it immediately.
        /// </summary>
        public CompletedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(true);
        }

        /// <summary>
        /// Waits for the asynchronous operation to end.
        /// </summary>
        public void End()
        {
            End<CompletedAsyncResult>(this);
        }
    }

    /// <summary>
    /// An <see cref="AsyncResult"/> which completes instantly (synchronous result wrapped in <see cref="IAsyncResult"/>).
    /// </summary>
    public class CompletedAsyncResult<T> : AsyncResult
    {
        /// <summary>
        /// State.
        /// </summary>
        readonly T _data;

        /// <summary>
        /// Creates an instance with the specified callback and state,
        /// then completes it immediately.
        /// </summary>
        public CompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _data = data;
            Complete(true);
        }

        /// <summary>
        /// Waits for the asynchronous operation to end.
        /// </summary>
        public T End()
        {
            var completedResult = End<CompletedAsyncResult<T>>(this);
            return completedResult._data;
        }
    }
}
