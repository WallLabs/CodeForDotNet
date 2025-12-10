using System;

namespace CodeForDotNet.Threading;

/// <summary>
/// An <see cref="AsyncResult"/> which completes instantly (synchronous result wrapped in <see cref="IAsyncResult"/>).
/// </summary>
public class CompletedAsyncResult : AsyncResult
{
    #region Public Constructors

    /// <summary>
    /// Creates an instance with the specified callback and state, then completes it immediately.
    /// </summary>
    public CompletedAsyncResult(AsyncCallback callback, object state)
        : base(callback, state)
    {
        Complete(true);
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Waits for the asynchronous operation to end.
    /// </summary>
    public void End()
    {
        _ = End<CompletedAsyncResult>(this);
    }

    #endregion Public Methods
}

/// <summary>
/// An <see cref="AsyncResult"/> which completes instantly (synchronous result wrapped in <see cref="IAsyncResult"/>).
/// </summary>
public class CompletedAsyncResult<T> : AsyncResult
{
    #region Private Fields

    /// <summary>
    /// State.
    /// </summary>
    private readonly T _data;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Creates an instance with the specified callback and state, then completes it immediately.
    /// </summary>
    public CompletedAsyncResult(T data, AsyncCallback callback, object state)
        : base(callback, state)
    {
        _data = data;
        Complete(true);
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Waits for the asynchronous operation to end.
    /// </summary>
    public T End()
    {
        var completedResult = End<CompletedAsyncResult<T>>(this);
        return completedResult._data;
    }

    #endregion Public Methods
}
