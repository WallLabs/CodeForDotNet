using System.Data;
using Microsoft.Data.SqlClient;

namespace CodeForDotNet.Data.Sql
{
    /// <summary>
    /// Disposable base class for a SQL Data Access Layer which holds connection information and helper methods.
    /// </summary>
    public abstract class SqlDataAccessLayer : DisposableObject
    {
        #region Protected Constructors

        /// <summary>
        /// Initializes the database connection (but does not open it). Ensure this object is disposed to close the connection if it was opened.
        /// </summary>
        protected SqlDataAccessLayer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion Protected Constructors

        #region Protected Properties

        /// <summary>
        /// Current connection, for the lifetime of this object. Opened on the first call to OpenConnection(), closed during Dispose.
        /// </summary>
        protected Microsoft.Data.SqlClient.SqlConnection? Connection { get; private set; }

        /// <summary>
        /// Connection string for this instance.
        /// </summary>
        protected string ConnectionString { get; private set; }

        #endregion Protected Properties

        #region Protected Methods

        /// <summary>
        /// Disposes resources either proactively via <see cref="Dispose"/> or during finalization.
        /// </summary>
        /// <param name="disposing">True when disposing, false when finalizing.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose only once
            if (IsDisposed)
                return;

            // Dispose
            try
            {
                // Dispose managed resources
                if (disposing)
                {
                    if (Connection != null)
                        Connection.Dispose();
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Opens or re-opens the connection if closed or broken.
        /// </summary>
        protected SqlConnection OpenConnection()
        {
            // Open the connection if closed or re-open if broken
            if ((Connection == null) || (Connection.State == ConnectionState.Closed) || (Connection.State == ConnectionState.Broken))
            {
                if (Connection != null)
                    Connection.Dispose();
                Connection = new SqlConnection(ConnectionString);
                Connection.Open();
            }

            // Return opened connection
            return Connection;
        }

        #endregion Protected Methods
    }
}
