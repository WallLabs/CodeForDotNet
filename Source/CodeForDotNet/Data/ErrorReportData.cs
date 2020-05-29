using System;
using System.Runtime.Serialization;

namespace CodeForDotNet.Data
{
	/// <summary>
	/// Error data used to report failures to a service.
	/// </summary>
	[DataContract]
	public class ErrorReportData : GuidDataKey
	{
		#region Public Properties

		/// <summary>
		/// Optional full type name of the exception which occurred.
		/// </summary>
		[DataMember]
		public string? ErrorTypeFullName { get; set; }

		/// <summary>
		/// Date and time when the event occurred.
		/// </summary>
		[DataMember(IsRequired = true)]
		public DateTimeOffset EventDate { get; set; }

		/// <summary>
		/// Message.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string? Message { get; set; }

		/// <summary>
		/// Source assembly name including fully qualified name, version and any other attributes such as public key.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string? SourceAssemblyName { get; set; }

		/// <summary>
		/// Unique identifier used to group reports from the same source.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid SourceId { get; set; }

		/// <summary>
		/// Optional stack trace.
		/// </summary>
		[DataMember]
		public string? StackTrace { get; set; }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Tests two objects of this type for in-equality by value.
		/// </summary>
		public static bool operator !=(ErrorReportData left, ErrorReportData right)
		{
			return !(left?.Equals(right) ?? right is null);
		}

		/// <summary>
		/// Tests two objects of this type for equality by value.
		/// </summary>
		public static bool operator ==(ErrorReportData left, ErrorReportData right)
		{
			return left?.Equals(right) ?? right is null;
		}

		/// <summary>
		/// Compares this object with another by value.
		/// </summary>
		public override bool Equals(object unknown)
		{
			// Compare null and type
			if (!(unknown is ErrorReportData other) || other is null)
				return false;

			return
				base.Equals(other) &&
				other.SourceId == SourceId &&
				other.SourceAssemblyName == SourceAssemblyName &&
				other.EventDate == EventDate &&
				other.Message == Message &&
				other.ErrorTypeFullName == ErrorTypeFullName &&
				other.StackTrace == StackTrace;
		}

		/// <summary>
		/// Gets an XOR based hash code based on the contents of this object.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^
				   SourceId.GetHashCode() ^
#if !NETSTANDARD2_0
				   (SourceAssemblyName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0) ^
				   EventDate.GetHashCode() ^
				   (Message != null ? Message.GetHashCode(StringComparison.OrdinalIgnoreCase) : 0) ^
				   (ErrorTypeFullName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0) ^
				   (StackTrace?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0);
#else
                   (SourceAssemblyName?.GetHashCode() ?? 0) ^
                   EventDate.GetHashCode() ^
                   (Message != null ? Message.GetHashCode() : 0) ^
                   (ErrorTypeFullName?.GetHashCode() ?? 0) ^
                   (StackTrace?.GetHashCode() ?? 0);
#endif
        }

#endregion Public Methods
    }
}
