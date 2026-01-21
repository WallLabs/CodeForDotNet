namespace CodeForDotNet.ComponentModel;

/// <summary>
/// State of a <see cref="IDataObject"/>.
/// </summary>
public enum DataObjectState
{
    /// <summary>
    /// No state has been loaded yet. Objects have this state when they are created, before <see cref="IDataObject.Create"/> or
    /// <see cref="IDataObject.Read"/> is called.
    /// </summary>
    None,

    /// <summary>
    /// The object has all properties loaded, with no uncommitted changes.
    /// </summary>
    Current,

    /// <summary>
    /// The object has uncommitted changes.
    /// </summary>
    Changed,

    /// <summary>
    /// The object has been deleted.
    /// </summary>
    Deleted
}
