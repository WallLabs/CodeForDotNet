using System;

namespace CodeForDotNet.Collections
{
	/// <summary>
	/// Notifies listeners of changes to a dictionary, such as when items are added, removed, changed or the whole collection reset.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	public interface INotifyDictionaryChanged<TKey, TValue>
	{
		#region Public Events

		/// <summary>
		/// Occurs when the dictionary changes.
		/// </summary>
		event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>>? DictionaryChanged;

		#endregion Public Events
	}
}
