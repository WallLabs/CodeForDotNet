using System;
using System.Diagnostics;

namespace CodeForDotNet.Diagnostics
{
	/// <summary>
	/// Filters trace events for a specific activity ID.
	/// </summary>
	public class ActivityIdTraceFilter : TraceFilter
	{
		#region Public Constructors

		/// <summary>
		/// Creates an instance to filter all events except the specified activity ID.
		/// </summary>
		public ActivityIdTraceFilter(Guid id)
		{
			ActivityId = id;
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// Activity ID to capture (all others are filtered).
		/// </summary>
		public Guid ActivityId { get; set; }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Filters events.
		/// </summary>
		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return Trace.CorrelationManager.ActivityId == ActivityId;
		}

		#endregion Public Methods
	}
}
