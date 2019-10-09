using System;
using System.Data.SqlClient;

namespace CodeForDotNet.Data.Sql.Adapters
{
	/// <summary>
	/// Data adapter for the <see cref="ErrorReportData"/> entity.
	/// </summary>
	public static class ErrorReportDataAdapter
	{
		#region Public Methods

		/// <summary>
		/// Reads properties of a <see cref="ErrorReportData"/> from a data reader.
		/// </summary>
		public static void Read(this ErrorReportData entity, SqlDataReader reader)
		{
			Read(entity, reader, false);
		}

		/// <summary>
		/// Reads properties of a <see cref="ErrorReportData"/> from a data reader.
		/// </summary>
		public static void Read(this ErrorReportData entity, SqlDataReader reader, bool skipKeys)
		{
			// Validate.
			if (entity is null) throw new ArgumentNullException(nameof(entity));

			// Read entity.
			if (!skipKeys) GuidDataKeyAdapter.Read(entity, reader);
			entity.SourceId = reader.Get<Guid>("SourceId");
			entity.SourceAssemblyName = reader.GetString("SourceAssemblyName");
			entity.EventDate = reader.Get<DateTimeOffset>("EventDate");
			entity.Message = reader.Get<string>("Message");
			entity.ErrorTypeFullName = reader.GetNullable<string>("ErrorTypeFullName");
			entity.StackTrace = reader.GetNullable<string>("StackTrace");
		}

		/// <summary>
		/// Sets data command parameter values for a <see cref="ErrorReportData"/>.
		/// </summary>
		public static void Set(
			this ErrorReportData entity, SqlParameterCollection parameters)
		{
			Set(entity, parameters, false);
		}

		/// <summary>
		/// Sets data command parameter values for a <see cref="ErrorReportData"/>.
		/// </summary>
		public static void Set(this ErrorReportData entity, SqlParameterCollection parameters, bool skipKeys)
		{
			// Validate.
			if (entity is null) throw new ArgumentNullException(nameof(entity));
			if (parameters is null) throw new ArgumentNullException(nameof(parameters));

			// Set parameters.
			if (!skipKeys) GuidDataKeyAdapter.Set(entity, parameters);
			parameters["@sourceId"].Value = entity.SourceId;
			parameters["@sourceAssemblyName"].Value = entity.SourceAssemblyName;
			parameters["@eventDate"].Value = entity.EventDate;
			parameters["@message"].Value = entity.Message;
			parameters["@errorTypeFullName"].Value = entity.ErrorTypeFullName;
			parameters["@stackTrace"].Value = entity.StackTrace;
		}

		#endregion Public Methods
	}
}
