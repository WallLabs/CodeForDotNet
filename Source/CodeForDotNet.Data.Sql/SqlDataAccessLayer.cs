using System.Data;
using System.Data.SqlClient;

namespace CodeForDotNet.Data.Sql
{
    /// <summary>
    /// Disposable base class for a SQL Data Access Layer which holds connection
    /// information and helper methods.
    /// </summary>
    public abstract class SqlDataAccessLayer : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Initializes the database connection (but does not open it).
        /// Ensure this object is disposed to close the connection if it was opened.
        /// </summary>
        protected SqlDataAccessLayer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #region IDisposable Members

        /// <summary>
        /// Diposes resources either proactively via Dipose() or during finalization.
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

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Connection string for this instance.
        /// </summary>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// Current connection, for the lifetime of this object.
        /// Opened on the first call to OpenConnection(), closed during Dispose.
        /// </summary>
        protected SqlConnection Connection { get; private set; }

        #endregion

        #region Private Methods

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

        #endregion
    }
}
