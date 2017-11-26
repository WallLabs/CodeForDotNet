using System;
using System.Data.SqlClient;

namespace CodeForDotNet.Data.Adapters
{
    /// <summary>
    /// Data adapter for the <see cref="GuidDataKey"/> entity.
    /// </summary>
    public static class GuidDataKeyAdapter
    {
        /// <summary>
        /// Reads properties of a <see cref="GuidDataKey"/> from a data reader.
        /// </summary>
        public static void Read(this GuidDataKey entity, SqlDataReader reader)
        {
            entity.Id = reader.Get<Guid>("Id");
        }

        /// <summary>
        /// Sets data command parameter values for a <see cref="GuidDataKey"/>.
        /// </summary>
        public static void Set(this GuidDataKey entity, SqlParameterCollection parameters)
        {
            parameters["@id"].Value = entity.Id;
        }
    }
}
