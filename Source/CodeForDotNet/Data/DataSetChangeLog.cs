using CodeForDotNet.Properties;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace CodeForDotNet.Data
{
	/// <summary>
	/// DataSet change log - monitors DataSet changes to facilitate, roll-back and roll-forward functionality plus a descriptive change list, e.g. to support
	/// Undo/Redo capability.
	/// </summary>
	public class DataSetChangeLog : DisposableObject
	{
		#region Private Fields

		/// <summary>
		/// The change log. Contains the details of all previous actions, available for Rollback, and any future action, available for roll-forward.
		/// </summary>
		private readonly Collection<DataSetChangeLogEntry> _changeLog;

		/// <summary>
		/// Current entry in the change log. Unless there are any redo actions available, this will always be the last entry in the array. When the change log is
		/// empty, the index is -1.
		/// </summary>
		private int _logIndex;

		#endregion Private Fields

		#region Public Constructors

		/// <summary>
		/// Creates an instance to monitor the specified DataSet.
		/// </summary>
		public DataSetChangeLog(DataSet data)
		{
			Data = data;
			_changeLog = new Collection<DataSetChangeLogEntry>();
			_logIndex = -1;
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// DataSet which this change log belongs to.
		/// </summary>
		public DataSet Data { get; private set; }

		/// <summary>
		/// Thread synchronization object.
		/// </summary>
		public object SyncRoot { get; } = new object();

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Commits the current changes since the last checkpoint to the ChangeLog, making it available for Rollback as a unit.
		/// </summary>
		public void Checkpoint(string name)
		{
			lock (SyncRoot)
			{
				// Get changes
				var changes = Data.GetChanges();

				// Do nothing when no changes
				if (changes == null)
					return;

				// Do not check constraints on changes (may be orphaned)
				changes.EnforceConstraints = false;

				// Truncate existing log (remove any roll-forwards entries)
				if (_logIndex < _changeLog.Count - 1)
				{
					while (_changeLog.Count > (_logIndex + 1))
						_changeLog.RemoveAt(_changeLog.Count - 1);
				}

				// Add change log entry
				var logEntry = new DataSetChangeLogEntry(DateTime.UtcNow, name, changes);
				_changeLog.Add(logEntry);
				_logIndex++;

				// Commit changes
				Data.AcceptChanges();
			}
		}

		/// <summary>
		/// Gets a list of names for the changes available to roll-forwards. The list is in chronological order, with the next change at the beginning and last
		/// change at the end.
		/// </summary>
		public Collection<string> GetRedoList()
		{
			lock (SyncRoot)
			{
				var result = new Collection<string>();
				for (var i = _logIndex; (i >= 0) && (i < _changeLog.Count); i++)
					result.Add(_changeLog[i].Name);
				return result;
			}
		}

		/// <summary>
		/// Gets a list of names for the changes available to roll-back. The list is in chronological order, with the oldest change at the beginning and last
		/// change at the end.
		/// </summary>
		public Collection<string> GetUndoList()
		{
			lock (SyncRoot)
			{
				var result = new Collection<string>();
				for (var i = _logIndex + 1; (i >= 0) && (i < _changeLog.Count); i--)
					result.Add(_changeLog[i].Name);
				return result;
			}
		}

		/// <summary>
		/// Rolls the DataSet forwards by the specified number of checkpoint "steps".
		/// </summary>
		public void Redo(int steps)
		{
			lock (SyncRoot)
			{
				// Validate request
				if ((steps < 1) || (steps > (_changeLog.Count - _logIndex)))
					throw new InvalidOperationException();

				// Disable constraints during update (if enabled)
				var enforceConstraints = Data.EnforceConstraints;
				if (enforceConstraints)
					Data.EnforceConstraints = false;

				// Undo any outstanding changes before Checkpoint
				Data.RejectChanges();

				// Rollforward the specified number of steps...
				while (steps-- > 0)
				{
					// Get change log entry
					var logEntry = _changeLog[_logIndex + 1];
					var changes = logEntry.Changes;

					// Re-apply changes to DataSet
					foreach (DataTable changedTable in changes.Tables)
					{
						foreach (DataRow changedRow in changedTable.Rows)
						{
							// Re-apply action
							switch (changedRow.RowState)
							{
								case DataRowState.Added:
								{
									// Re-apply INSERT...

									// Insert the changed row
									Data.Tables[changedTable.TableName].Rows.Add(changedRow.ItemArray);
									break;
								}

								case DataRowState.Modified:
								{
									// Re-apply UPDATE...

									// Find the original row
									var primaryKeyStatement = GetPrimaryKeyFilterExpression(changedRow);
									var existingRows = Data.Tables[changedTable.TableName].Select(primaryKeyStatement);
									if (existingRows.Length == 0)
										throw new InvalidOperationException(Resources.DataSetChangeLogRedoUpdateError);
									var row = existingRows[0];

									// Update row values
									row.ItemArray = changedRow.ItemArray;
									break;
								}

								case DataRowState.Deleted:
								{
									// Re-apply DELETE...

									// Temporarily un-delete change row to allow access to key information
									changedRow.RejectChanges();

									// Find the original row
									var primaryKeyStatement = GetPrimaryKeyFilterExpression(changedRow);
									var existingRows = Data.Tables[changedTable.TableName].Select(primaryKeyStatement);
									if (existingRows.Length == 0)
										throw new InvalidOperationException(Resources.DataSetChangeLogRedoDeleteError);
									var row = existingRows[0];

									// Restore change row to original state
									changedRow.Delete();

									// Delete the row
									row.Delete();
									break;
								}
							}
						}
					}

					// Commit changes
					Data.AcceptChanges();

					// Move change log pointer forwards
					_logIndex++;
				}

				// Restore constraints after update (if previously enabled)
				if (enforceConstraints)
					Data.EnforceConstraints = true;
			}
		}

		/// <summary>
		/// Rolls-back the DataSet by 1 step.
		/// </summary>
		public void Undo()
		{
			// Call overloaded function
			Undo(1);
		}

		/// <summary>
		/// Rolls-back the DataSet by the specified number of checkpoint "steps".
		/// </summary>
		public void Undo(int steps)
		{
			lock (SyncRoot)
			{
				// Validate request
				if ((steps < 1) || (steps > (_logIndex + 1)))
					throw new InvalidOperationException();

				// Disable constraints during update (if enabled)
				var enforceConstraints = Data.EnforceConstraints;
				if (enforceConstraints)
					Data.EnforceConstraints = false;

				// Undo any outstanding changes before Checkpoint
				Data.RejectChanges();

				// Rollback the specified number of steps...
				while (steps-- > 0)
				{
					// Get change log entry
					var logEntry = _changeLog[_logIndex];
					var changes = logEntry.Changes;

					// Reverse changes to DataSet
					foreach (DataTable changedTable in changes.Tables)
					{
						foreach (DataRow changedRow in changedTable.Rows)
						{
							// Reverse action
							switch (changedRow.RowState)
							{
								case DataRowState.Added:
								{
									// Reverse INSERT...

									// Find the original row
									var primaryKeyStatement = GetPrimaryKeyFilterExpression(changedRow);
									var existingRows = Data.Tables[changedTable.TableName].Select(primaryKeyStatement);
									if (existingRows.Length == 0)
										throw new InvalidOperationException(Resources.DataSetChangeLogUndoInsertError);
									var row = existingRows[0];

									// Delete the row
									row.Delete();
									break;
								}

								case DataRowState.Modified:
								{
									// Reverse UPDATE...

									// Find the original row
									var primaryKeyStatement = GetPrimaryKeyFilterExpression(changedRow);
									var existingRows = Data.Tables[changedTable.TableName].Select(primaryKeyStatement);
									if (existingRows.Length == 0)
										throw new InvalidOperationException(Resources.DataSetChangeLogUndoUpdateError);
									var row = existingRows[0];

									// Restore the original row values
									for (var i = 0; i < changedTable.Columns.Count; i++)
										row[i] = changedRow[i, DataRowVersion.Original];
									break;
								}

								case DataRowState.Deleted:
								{
									// Reverse DELETE...

									// Insert the original row
									var table = Data.Tables[changedTable.TableName];
									var row = table.NewRow();
									for (var i = 0; i < changedTable.Columns.Count; i++)
										row[i] = changedRow[i, DataRowVersion.Original];
									table.Rows.Add(row);
									break;
								}
							}
						}
					}

					// Commit changes
					Data.AcceptChanges();

					// Move change log pointer backwards (do not remove forward entries until next Checkpoint, to allow roll-forward until that time)
					_logIndex--;
				}

				// Restore constraints after update (if previously enabled)
				if (enforceConstraints)
					Data.EnforceConstraints = true;
			}
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Frees resources used by this object.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				// Dispose managed resources
				if (disposing)
				{
					Data.Dispose();
					if (_changeLog != null)
					{
						foreach (var change in _changeLog)
							change.Dispose();
						_changeLog.Clear();
					}
				}
			}
			finally
			{
				// Dispose base class
				base.Dispose(disposing);
			}
		}

		#endregion Protected Methods

		#region Private Methods

		/// <summary>
		/// Builds a DataRow filter expression statement that can be used to Select() the row uniquely within it's DataTable. Requires a primary key to be
		/// defined in the table schema.
		/// </summary>
		/// <param name="row">Row to use as a model for the filter expression. Primary key values will be taken from here.</param>
		/// <returns>Filter expression.</returns>
		private static string GetPrimaryKeyFilterExpression(DataRow row)
		{
			// Initialize
			var table = row.Table;
			var result = string.Empty;

			// Validate request
			if (table.PrimaryKey.Length == 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.DataSetChangeLogGetPrimaryKeyFilterExpressionNoPrimaryKey, table.TableName));
			}

			// Build selection expression
			for (var index = 0; index < table.PrimaryKey.Length; index++)
			{
                // Add logical AND operator when multiple.
				if (index > 0)
					result += " AND ";

                // Add column name.
                var keyColumn = table.PrimaryKey[index];
                result += "[" + keyColumn.ColumnName + "] = ";

                // Add value with any quotes escaped.
                var value = row[keyColumn.Ordinal];
                var valueText = Convert.ToString(value, CultureInfo.InvariantCulture);
                if ((keyColumn.DataType == typeof(string)) || (keyColumn.DataType == typeof(Guid)))
                {
#if !NETSTANDARD2_0
                    valueText = $"'{valueText.Replace("'", "''", StringComparison.OrdinalIgnoreCase)}'";
#else
                    valueText = $"'{valueText.Replace("'", "''")}'";
#endif
                }
                result += valueText;
			}

			// Return result
			return result;
		}

#endregion Private Methods
	}

	/// <summary>
	/// Contains information about a change in the Change Log.
	/// </summary>
	public class DataSetChangeLogEntry : DisposableObject
	{
#region Public Constructors

		/// <summary>
		/// Creates a new instance of this structure containing the specified data.
		/// </summary>
		public DataSetChangeLogEntry(DateTime timeStamp, string name, DataSet changes)
		{
			TimeStamp = timeStamp;
			Name = name;
			Changes = changes;
		}

#endregion Public Constructors

#region Public Properties

		/// <summary>
		/// Snapshot of the changes, including DataRowVersion.Original data needed to Rollback, and DataRowVersion.Current needed to roll-forward.
		/// </summary>
		public DataSet Changes { get; private set; }

		/// <summary>
		/// Short name of the action.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Time-stamp at which the action occurred.
		/// </summary>
		public DateTime TimeStamp { get; private set; }

#endregion Public Properties

#region Protected Methods

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

#endregion Protected Methods
	}
}
