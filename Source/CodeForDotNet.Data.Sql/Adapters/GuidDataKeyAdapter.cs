using System;

namespace CodeForDotNet.Data.Sql.Adapters;

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
        ArgumentNullException.ThrowIfNull(entity);

        // Read entity.
        entity.Id = reader.Get<Guid>("Id");
    }

    /// <summary>
    /// Sets data command parameter values for a <see cref="GuidDataKey"/>.
    /// </summary>
    public static void Set(this GuidDataKey entity, Microsoft.Data.SqlClient.SqlParameterCollection parameters)
    {
        // Validate.
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(parameters);

        // Set parameters.
        parameters["@id"].Value = entity.Id;
    }

    #endregion Public Methods
}
