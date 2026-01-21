using System;

namespace CodeForDotNet.ComponentModel;

/// <summary>
/// Event arguments for the <see cref="IDataObject.DataChanged"/> event.
/// </summary>
public class DataObjectChangeEventArgs : PropertyStoreChangeEventArgs
{
    #region Public Constructors

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public DataObjectChangeEventArgs()
    {
    }

    /// <summary>
    /// Creates an instance with the specified values.
    /// </summary>
    public DataObjectChangeEventArgs(DataObjectChangeAction action, Guid[] keys)
        : base(keys)
    {
        Action = action;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Action which occurred.
    /// </summary>
    public DataObjectChangeAction Action { get; set; }

    #endregion Public Properties
}
