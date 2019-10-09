using System;
using System.Collections.ObjectModel;
using System.Data;

namespace CodeForDotNet.Data
{
	/// <summary>
	/// Extensions to the <see cref="DataSet"/> class.
	/// </summary>
	public static class DataSetExtensions
	{
		#region Public Methods

		/// <summary>
		/// Gets all errors recorded within a DataSet.
		/// </summary>
		/// <remarks>
		/// Useful because the default behavior of operations such as DataAdapter failed updates is to return a generic error and not the specifics. This method
		/// can be used to provide full error detail in logs or messages, or as a static call during debugging when no error handling exists.
		/// </remarks>
		/// <returns>Collection of error messages from all tables and rows in the DataSet.</returns>
		public static Collection<string> GetErrors(this DataSet dataSet)
		{
			// Validate
			if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));

			// Get errors...
			var errors = new Collection<string>();
			foreach (DataTable table in dataSet.Tables)
			{
				foreach (var row in table.GetErrors())
					errors.Add(table.TableName + ": " + row.RowError);
			}
			return errors;
		}

		#endregion Public Methods
	}
}
