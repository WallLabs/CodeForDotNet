using System;
using System.Data;

namespace CodeForDotNet.Data;

/// <summary>
/// Contains information about a change in the Change Log.
/// </summary>
/// <remarks>
/// Creates a new instance of this structure containing the specified data.
/// </remarks>
public class DataSetChangeLogEntry(DateTime timeStamp, string name, DataSet changes) : DisposableObject
{
    /// <summary>
    /// Snapshot of the changes, including DataRowVersion.Original data needed to Rollback, and DataRowVersion.Current needed to roll-forward.
    /// </summary>
    public DataSet Changes { get; private set; } = changes;

    /// <summary>
    /// Short name of the action.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Time-stamp at which the action occurred.
    /// </summary>
    public DateTime TimeStamp { get; private set; } = timeStamp;

    /// <summary>
    /// Frees resources used by this object.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Disposed managed resources during dispose
            if (disposing)
            {
                Changes?.Dispose();
            }
        }
        finally
        {
            // Dispose base class
            base.Dispose(disposing);
        }
    }
}
