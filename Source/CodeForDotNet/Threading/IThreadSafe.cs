namespace CodeForDotNet.Threading;

/// <summary>
/// Interface which supports thread safety by providing a synchronization locking object.
/// </summary>
public interface IThreadSafe
{
    #region Public Properties

    /// <summary>
    /// Thread synchronization object.
    /// </summary>
    /// <remarks>
    /// Lock this object when you read or write properties of this object which must be complete as a batch before any other threads enter the section, e.g.
    /// during data load or save operations.
    /// </remarks>
    object SyncRoot { get; }

    #endregion Public Properties
}
