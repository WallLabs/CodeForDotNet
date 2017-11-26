using System;

namespace CodeForDotNet
{
    /// <summary>
    /// Provides greater control over object lifetime and memory management.
    /// </summary>
    public interface IDisposableObject : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Indicates if the object is in the process of or has already disposed.
        /// </summary>
        bool IsDisposing { get; }

        /// <summary>
        /// Indicates if the object has finished disposing.
        /// </summary>
        bool IsDisposed { get; }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the object starts to dispose.
        /// </summary>
        event EventHandler Disposing;

        /// <summary>
        /// Fired when the object has finished disposing.
        /// </summary>
        event EventHandler Disposed;

        #endregion
    }
}