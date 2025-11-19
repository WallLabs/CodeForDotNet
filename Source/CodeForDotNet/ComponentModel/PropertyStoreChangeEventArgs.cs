using System;
using System.Collections.ObjectModel;

namespace CodeForDotNet.ComponentModel
{
	/// <summary>
	/// Event arguments for the <see cref="IPropertyStore.PropertyStoreChanged"/> event.
	/// </summary>
	public class PropertyStoreChangeEventArgs : EventArgs
	{
		#region Public Constructors

		/// <summary>
		/// Creates an empty instance.
		/// </summary>
		public PropertyStoreChangeEventArgs()
		{
			Keys = [];
		}

		/// <summary>
		/// Creates an instance with the specified values.
		/// </summary>
		public PropertyStoreChangeEventArgs(Guid[] keys)
		{
			Keys = new Collection<Guid>(keys);
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// Names of the properties or relations which changed.
		/// </summary>
		public Collection<Guid> Keys { get; private set; }

		#endregion Public Properties
	}
}
