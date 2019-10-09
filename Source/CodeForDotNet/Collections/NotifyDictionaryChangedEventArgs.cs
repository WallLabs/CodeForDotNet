using System;
using System.Collections.Specialized;

namespace CodeForDotNet.Collections
{
	/// <summary>
	/// Event arguments for the <see cref="ObservableDictionary{TKey,TValue}.DictionaryChanged"/> event.
	/// </summary>
	public class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
	{
		#region Public Constructors

		/// <summary>
		/// Creates an instance with specific values.
		/// </summary>
		public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction change, TKey key, TValue value)
		{
			CollectionChange = change;
			Key = key;
			Value = value;
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// Change details.
		/// </summary>
		public NotifyCollectionChangedAction CollectionChange { get; private set; }

		/// <summary>
		/// Changed entry key.
		/// </summary>
		public TKey Key { get; private set; }

		/// <summary>
		/// Changed entry key.
		/// </summary>
		public TValue Value { get; private set; }

		#endregion Public Properties
	}
}
