using System;
using Microsoft.Data.SqlClient;

namespace CodeForDotNet.Data.Sql.Adapters
{
	/// <summary>
	/// Data adapter for the <see cref="GuidDataKey"/> entity.
	/// </summary>
	public static class GuidDataKeyAdapter
	{
		#region Public Methods

		/// <summary>
		/// Reads properties of a <see cref="GuidDataKey"/> from a data reader.
		/// </summary>
		public static void Read(this GuidDataKey entity, Microsoft.Data.SqlClient.SqlDataReader reader)
		{
			// Validate.
			if (entity is null) throw new ArgumentNullException(nameof(entity));

			// Read entity.
			entity.Id = reader.Get<Guid>("Id");
		}

		/// <summary>
		/// Sets data command parameter values for a <see cref="GuidDataKey"/>.
		/// </summary>
		public static void Set(this GuidDataKey entity, Microsoft.Data.SqlClient.SqlParameterCollection parameters)
		{
			// Validate.
			if (entity is null) throw new ArgumentNullException(nameof(entity));
			if (parameters is null) throw new ArgumentNullException(nameof(parameters));

			// Set parameters.
			parameters["@id"].Value = entity.Id;
		}

		#endregion Public Methods
	}
}
