using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace CodeForDotNet.Data
{
	/// <summary>
	/// Extensions for <see cref="IDataReader"/> based data readers, including the associated <see cref="IDataRecord"/> interface.
	/// </summary>
	public static class DataReaderExtensions
	{
		#region Public Methods

		/// <summary>
		/// Reads a <see parmref="T"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		/// <remarks>The type must match or have a cast operator, because no explicit conversion is performed.</remarks>
		public static T Get<T>(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.Get<T>(ordinal);
		}

		/// <summary>
		/// Reads a <see parmref="T"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <param name="conversionLocale">
		/// Optional <see cref="CultureInfo"/> locale override to use when type conversion is necessary. Only relevant when the types don't match and a specific
		/// locale is required. Leave null or unspecified to use the default thread locale.
		/// </param>
		/// <returns>Value of the correct type.</returns>
		/// <remarks>The type must match or have a cast operator, because no explicit conversion is performed.</remarks>
		public static T Get<T>(this IDataRecord reader, int ordinal, CultureInfo? conversionLocale = null)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read value and return when same type
			var value = reader.GetValue(ordinal);
			var valueType = value.GetType();
			var returnType = typeof(T);
			if (valueType == returnType)
				return (T)value;

			// Try to convert type when different
			var converter = TypeDescriptor.GetConverter(returnType);
			return conversionLocale != null
				? (T)converter.ConvertFrom(null, conversionLocale, value)
				: (T)converter.ConvertFrom(value);
		}

		/// <summary>
		/// Reads a <see cref="bool"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static bool GetBoolean(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetBoolean(ordinal);
		}

		/// <summary>
		/// Read a DateTime from a reader.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static DateTime GetDateTime(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetDateTime(ordinal);
		}

		/// <summary>
		/// Reads a nullable <see cref="DateTimeOffset"/> by column name with conversion to a <see cref="DateTimeOffset"/>, using either a specific or system
		/// time zone.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <param name="offset">
		/// Optional UTC offset to use during conversion. The system time zone is used when not specified, i.e. to covert from a source time which is local, do
		/// not specify an offset. To convert from a source time which is UTC, an offset of <see cref="TimeSpan.Zero"/> must be used.
		/// </param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		/// <remarks>
		/// <para>This function facilitates standardization of dates with offset, when reading databases which do not take time zones into account.</para>
		/// <para>
		/// When the local system time zone is used it will only work reliably when running on systems in the same time zone as the original database system.
		/// Hence it should only be used during migration or when the geographical location is fixed.
		/// </para>
		/// </remarks>
		public static DateTimeOffset GetDateTimeAsOffset(this IDataRecord reader, string column, TimeSpan? offset = null)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return GetDateTimeAsOffset(reader, ordinal, offset);
		}

		/// <summary>
		/// Reads a <see cref="DateTime"/> with conversion to a <see cref="DateTimeOffset"/>, using either a specific or system time zone.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Index of the column to read.</param>
		/// <param name="offset">
		/// Optional UTC offset to use during conversion. The system time zone is used when not specified, i.e. to covert from a source time which is local, do
		/// not specify an offset. To convert from a source time which is UTC, an offset of <see cref="TimeSpan.Zero"/> must be used.
		/// </param>
		/// <remarks>
		/// <para>This function facilitates standardization of dates with offset, when reading databases which do not take time zones into account.</para>
		/// <para>
		/// When the local system time zone is used it will only work reliably when running on systems in the same time zone as the original database system.
		/// Hence it should only be used during migration or when the geographical location is fixed.
		/// </para>
		/// </remarks>
		public static DateTimeOffset GetDateTimeAsOffset(this IDataRecord reader, int ordinal, TimeSpan? offset = null)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var value = reader.GetDateTime(ordinal);
			return offset.HasValue
				? new DateTimeOffset(value, offset.Value)   // Use specified offset
				: new DateTimeOffset(value);                // Use system time zone offset
		}

		/// <summary>
		/// Reads a <see cref="DateTimeOffset"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static DateTimeOffset GetDateTimeOffset(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return (DateTimeOffset)reader.GetValue(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="double"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static double GetDouble(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetFloat(ordinal);
		}

		/// <summary>
		/// Reads an <see cref="short"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static short GetInt16(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetInt16(ordinal);
		}

		/// <summary>
		/// Reads an <see cref="int"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static int GetInt32(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetInt32(ordinal);
		}

		/// <summary>
		/// Reads an <see cref="long"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static long GetInt64(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetInt64(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;T&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <param name="defaultValue">
		/// Value to return when the column is <see cref="System.DBNull"/>. When not specified returns the default type value, e.g. empty value such as zero for
		/// int types, or null for reference types.
		/// </param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		/// <remarks>The type must match or have a cast operator, because no explicit conversion is performed.</remarks>
		public static T GetNullable<T>(this IDataRecord reader, string column, T defaultValue = default)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullable(ordinal, defaultValue);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;T&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <param name="defaultValue">
		/// Value to return when the column is <see cref="System.DBNull"/>. When not specified returns the default type value, e.g. empty value such as zero for
		/// int types, or null for reference types.
		/// </param>
		/// <returns>Value of the correct type or default when <see cref="System.DBNull"/>.</returns>
		/// <remarks>The type must match or have a cast operator, because no explicit conversion is performed.</remarks>
		public static T GetNullable<T>(this IDataRecord reader, int ordinal, T defaultValue = default)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? defaultValue : (T)reader.GetValue(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Boolean&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static bool? GetNullableBoolean(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableBoolean(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Boolean&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static bool? GetNullableBoolean(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTime&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static DateTime? GetNullableDateTime(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableDateTime(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTime&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static DateTime? GetNullableDateTime(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTime&gt;"/> from a record by column name, with conversion to a <see cref="Nullable&lt;DateTimeOffset&gt;"/>, using
		/// either a specific or system time zone.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <param name="offset">
		/// Optional UTC offset to use during conversion. The system time zone is used when not specified, i.e. to covert from a source time which is local, do
		/// not specify an offset. To convert from a source time which is UTC, an offset of <see cref="TimeSpan.Zero"/> must be used.
		/// </param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		/// <remarks>
		/// <para>This function facilitates standardization of dates with offset, when reading databases which do not take time zones into account.</para>
		/// <para>
		/// When the local system time zone is used it will only work reliably when running on systems in the same time zone as the original database system.
		/// Hence it should only be used during migration or when the geographical location is fixed.
		/// </para>
		/// </remarks>
		public static DateTimeOffset? GetNullableDateTimeAsOffset(this IDataRecord reader, string column, TimeSpan? offset = null)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return GetNullableDateTimeAsOffset(reader, ordinal, offset);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTime&gt;"/> with conversion to a <see cref="Nullable&lt;DateTimeOffset&gt;"/>, using either a specific or system
		/// time zone.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <param name="offset">
		/// Optional UTC offset to use during conversion. The system time zone is used when not specified, i.e. to covert from a source time which is local, do
		/// not specify an offset. To convert from a source time which is UTC, an offset of <see cref="TimeSpan.Zero"/> must be used.
		/// </param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		/// <remarks>
		/// <para>This function facilitates standardization of dates with offset, when reading databases which do not take time zones into account.</para>
		/// <para>
		/// When the local system time zone is used it will only work reliably when running on systems in the same time zone as the original database system.
		/// Hence it should only be used during migration or when the geographical location is fixed.
		/// </para>
		/// </remarks>
		public static DateTimeOffset? GetNullableDateTimeAsOffset(this IDataRecord reader, int ordinal, TimeSpan? offset = null)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : GetDateTimeAsOffset(reader, ordinal, offset);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTimeOffset&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static DateTimeOffset? GetNullableDateTimeOffset(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableDateTimeOffset(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;DateTimeOffset&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static DateTimeOffset? GetNullableDateTimeOffset(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : (DateTimeOffset)reader.GetValue(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Double&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static double? GetNullableDouble(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableDouble(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Double&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static double? GetNullableDouble(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (double?)null : reader.GetFloat(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int16&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static short? GetNullableInt16(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableInt16(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int16&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static short? GetNullableInt16(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (short?)null : reader.GetInt16(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int32&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static int? GetNullableInt32(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableInt32(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int32&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static int? GetNullableInt32(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int64&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static long? GetNullableInt64(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableInt64(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Int64&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static long? GetNullableInt64(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (long?)null : reader.GetInt64(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Single&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static float? GetNullableSingle(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableSingle(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;Single&gt;"/> from a record.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static float? GetNullableSingle(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;String&gt;"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static string? GetNullableString(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetNullableString(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="Nullable&lt;String&gt;"/> from a record
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="ordinal">Column ordinal to read.</param>
		/// <returns>Value of the correct type or null when <see cref="System.DBNull"/>.</returns>
		public static string? GetNullableString(this IDataRecord reader, int ordinal)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="float"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static float GetSingle(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetFloat(ordinal);
		}

		/// <summary>
		/// Reads a <see cref="string"/> from a record by column name.
		/// </summary>
		/// <param name="reader"><see cref="IDataRecord"/> to read from.</param>
		/// <param name="column">Column name to read.</param>
		/// <returns>Value of the correct type.</returns>
		public static string GetString(this IDataRecord reader, string column)
		{
			// Validate.
			if (reader is null) throw new ArgumentNullException(nameof(reader));

			// Read and return value.
			var ordinal = reader.GetOrdinal(column);
			return reader.GetString(ordinal);
		}

		#endregion Public Methods
	}
}
