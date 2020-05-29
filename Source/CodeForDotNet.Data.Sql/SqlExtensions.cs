using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.Data.Sql
{
	/// <summary>
	/// Extensions and helper methods for working with SQL.
	/// </summary>
	public static class SqlExtensions
	{
		#region Private Fields

		/// <summary>
		/// Cache of <see cref="SqlParameter"/> items for a given command hash-code.
		/// </summary>
		private static readonly Dictionary<int, SqlParameter[]> ParameterCache = new Dictionary<int, SqlParameter[]>();

		#endregion Private Fields

		#region Public Methods

		/// <summary>
		/// Creates an SQL Server connection string from common pieces, e.g. server name, database, user &amp; password (if not integrated security).
		/// </summary>
		/// <param name="serverName">Database server name or (local) for shared memory local access.</param>
		/// <param name="databaseName">Default database name.</param>
		/// <param name="userName">User name or null/empty for Windows integrated security.</param>
		/// <param name="password">Password or null/empty for Windows integrated security.</param>
		/// <returns>SQL connection string.</returns>
		public static string BuildConnectionString(string serverName, string databaseName, string userName, string password)
		{
			var builder = new SqlConnectionStringBuilder
			{
				DataSource = serverName,
				InitialCatalog = databaseName
			};
			if (string.IsNullOrEmpty(userName))
			{
				// Use integrated security when no credentials provided
				builder.IntegratedSecurity = true;
			}
			else
			{
				// ...else use SQL Server security
				builder.UserID = userName;
				builder.Password = password;
			}
			return builder.ToString();
		}

		/// <summary>
		/// Creates a command object for the specified stored procedure, automatically populating the parameters. Parameter sets are cached for performance.
		/// </summary>
		/// <param name="connection">Connection used to derive parameters (first time only) or create the command.</param>
		/// <param name="procedureName">Name of the stored procedure for which to create the command and parameters.</param>
		[SuppressMessage("Microsoft.Security", "CA2100", Justification = "String parameter is only used in isolation as a stored procedure name.")]
		public static SqlCommand CreateCommandWithParameters(this SqlConnection connection, string procedureName)
		{
			// Validate
			if (connection is null) throw new ArgumentNullException(nameof(connection));
			if (procedureName is null) throw new ArgumentNullException(nameof(procedureName));

			// Open connection if necessary
			if (connection.State != ConnectionState.Open)
				connection.Open();

			// Create the command
			var cmd = connection.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = procedureName;

            // Get parameter set with caching
#if !NETSTANDARD2_0
            var cacheKey = HashCode.Combine(connection.ConnectionString.GetHashCode(StringComparison.OrdinalIgnoreCase),
            procedureName.GetHashCode(StringComparison.OrdinalIgnoreCase));
#else
            var cacheKey = connection.ConnectionString.GetHashCode() ^ procedureName.GetHashCode();
#endif
            var cached = true;
			if (!ParameterCache.ContainsKey(cacheKey))
			{
				lock (ParameterCache)
				{
					if (!ParameterCache.ContainsKey(cacheKey))
					{
						cached = false;

						// Use CommandBuilder to query database parameter schema from database
						SqlCommandBuilder.DeriveParameters(cmd);

						// Save parameter schema to cache
						var parameterCache = new List<SqlParameter>();
						foreach (SqlParameter parameter in cmd.Parameters)
						{
							var cacheParameter = new SqlParameter(
								parameter.ParameterName, parameter.SqlDbType,
								parameter.Size, parameter.Direction,
								parameter.Precision, parameter.Scale,
								parameter.SourceColumn, parameter.SourceVersion,
								parameter.SourceColumnNullMapping,
								null, parameter.XmlSchemaCollectionDatabase,
								parameter.XmlSchemaCollectionOwningSchema,
								parameter.XmlSchemaCollectionName)
							{
								Offset = parameter.Offset,
								TypeName = parameter.TypeName
							};
							parameterCache.Add(cacheParameter);
						}
						ParameterCache.Add(cacheKey, parameterCache.ToArray());
					}
				}
			}
			if (cached)
			{
				// Set parameters from cache
				var parameterCache = ParameterCache[cacheKey];
				foreach (var cacheParameter in parameterCache)
				{
					var parameter = new SqlParameter(
						cacheParameter.ParameterName, cacheParameter.SqlDbType,
						cacheParameter.Size, cacheParameter.Direction,
						cacheParameter.Precision, cacheParameter.Scale,
						cacheParameter.SourceColumn, cacheParameter.SourceVersion,
						cacheParameter.SourceColumnNullMapping,
						null, cacheParameter.XmlSchemaCollectionDatabase,
						cacheParameter.XmlSchemaCollectionOwningSchema,
						cacheParameter.XmlSchemaCollectionName)
					{
						Offset = cacheParameter.Offset,
						TypeName = cacheParameter.TypeName
					};
					cmd.Parameters.Add(parameter);
				}
			}

			// Return command with parameters
			return cmd;
		}

#endregion Public Methods
	}
}
