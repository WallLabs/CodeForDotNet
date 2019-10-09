using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeForDotNet.ComponentModel
{
	/// <summary>
	/// View component dependency object with intelligent property change event caching.
	/// </summary>
	public interface IPropertyStore : IEventCache, IDisposableObject, INotifyPropertyChanged
	{
		#region Public Events

		/// <summary>
		/// Fired when properties of this object are changed.
		/// </summary>
		event EventHandler<PropertyStoreChangeEventArgs>? PropertyStoreChanged;

		#endregion Public Events

		#region Public Methods

		/// <summary>
		/// Clears a property value if it exists, disposing any current value when flagged.
		/// </summary>
		/// <param name="id">Property ID.</param>
		void ClearProperty(Guid id);

		/// <summary>
		/// Checks if the property value exists.
		/// </summary>
		/// <param name="id">Property ID.</param>
		/// <returns>True when exists, otherwise false.</returns>
		bool ContainsProperty(Guid id);

		/// <summary>
		/// Checks if the property has a value.
		/// </summary>
		/// <param name="id">Property ID.</param>
		/// <returns>True when present, otherwise false.</returns>
		bool ContainsPropertyValue(Guid id);

		/// <summary>
		/// Gets a property value.
		/// </summary>
		/// <param name="id">Property ID.</param>
		/// <returns>Property value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the ID does not exist.</exception>
		T GetProperty<T>(Guid id);

		/// <summary>
		/// Gets a property value, or a default when it doesn't exist.
		/// </summary>
		/// <param name="id">Property ID.</param>
		/// <param name="defaultValue">Default value.</param>
		/// <returns>Property value or the default when it doesn't exist.</returns>
		T GetProperty<T>(Guid id, T defaultValue);

		/// <summary>
		/// Gets all property IDs.
		/// </summary>
		Collection<Guid> GetPropertyIds();

		/// <summary>
		/// Notifies this object that a related view object has changed.
		/// </summary>
		/// <param name="changed">Changed view object instance.</param>
		/// <param name="change">Change details.</param>
		void NotifyChange(IPropertyStore changed, PropertyStoreChangeEventArgs change);

		/// <summary>
		/// Sets multiple properties.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="properties"/> parameter is null or empty.</exception>
		void SetProperties(IDictionary<Guid, object> properties);

		/// <summary>
		/// Sets a property value.
		/// </summary>
		/// <param name="id">Property ID.</param>
		/// <param name="value">Property value.</param>
		void SetProperty<T>(Guid id, T value);

		#endregion Public Methods
	}
}
