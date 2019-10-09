using System;

namespace CodeForDotNet
{
	/// <summary>
	/// Event arguments for a changed value.
	/// </summary>
	public class ValueChangedEventArgs<T> : EventArgs
	{
		#region Public Constructors

		/// <summary>
		/// Creates an instance with the specified values.
		/// </summary>
		/// <param name="oldValue">Old value.</param>
		/// <param name="newValue">New value.</param>
		public ValueChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// New value.
		/// </summary>
		public T NewValue { get; set; }

		/// <summary>
		/// Old value.
		/// </summary>
		public T OldValue { get; set; }

		#endregion Public Properties
	}
}
