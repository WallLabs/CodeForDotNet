using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace CodeForDotNet.ComponentModel
{
	/// <summary>
	/// Base object which provides a compact <see cref="INotifyPropertyChanged"/> implementation. Use the
	/// <see cref="SetProperty&lt;T&gt;(ref T, T, string, SetPropertyEventHandler&lt;T&gt;, SetPropertyEventHandler&lt;T&gt;)"/> method in your property setters
	/// to automatically test for equality, then set the new value and fire the <see cref="PropertyChanged"/> event when changed.
	/// </summary>
	public abstract class NotifyPropertyChangedBase : EventCache, INotifyPropertyChanged
	{
		#region Private Fields

		/// <summary>
		/// Pending events which will be fired then cleared when events are resumed.
		/// </summary>
		private readonly List<string> _pendingPropertyChangeEvents;

		/// <summary>
		/// Synchronization context under which this object was created, e.g. parent UI dispatcher such as a DependencyObject context in XAML.
		/// </summary>
		private readonly SynchronizationContext _synchronization;

		#endregion Private Fields

		#region Protected Constructors

		/// <summary>
		/// Creates an empty instance using the current synchronization context.
		/// </summary>
		protected NotifyPropertyChangedBase() : this(SynchronizationContext.Current)
		{
		}

		/// <summary>
		/// Creates an empty instance with the specified synchronization context.
		/// </summary>
		protected NotifyPropertyChangedBase(SynchronizationContext synchronization)
		{
			_pendingPropertyChangeEvents = new List<string>();
			_synchronization = synchronization;
		}

		#endregion Protected Constructors

		#region Protected Delegates

		/// <summary>
		/// Method signature used to call
		/// <see cref="SetProperty&lt;T&gt;(ref T, T, string, SetPropertyEventHandler&lt;T&gt;, SetPropertyEventHandler&lt;T&gt;)"/> before and after callbacks.
		/// </summary>
		protected delegate void SetPropertyEventHandler<T>(INotifyPropertyChanged sender, ref T target,
		  T oldValue, T newValue, string propertyName);

		#endregion Protected Delegates

		#region Public Events

		/// <summary>
		/// Fired when a property changes.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		#endregion Public Events

		#region Protected Methods

		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event or caches it when events are suspended.
		/// </summary>
		protected void InvokePropertyChanged(string propertyName)
		{
			lock (SyncRoot)
			{
				if (EventsAreEnabled)
				{
					// Call event handler to fire event
					OnPropertyChanged(propertyName);
				}
				else
				{
					// Cache event
					if (!_pendingPropertyChangeEvents.Contains(propertyName))
						_pendingPropertyChangeEvents.Add(propertyName);
				}
			}
		}

		/// <summary>
		/// Fires pending events when resumed.
		/// </summary>
		protected override void OnEventsResumed()
		{
			lock (SyncRoot)
			{
				// Fire pending property change events
				var propertyChangedHandler = PropertyChanged;
				if (propertyChangedHandler != null)
				{
					var resumeEvents = _pendingPropertyChangeEvents.ToArray();
					_pendingPropertyChangeEvents.Clear();
					foreach (var propertyName in resumeEvents)
						OnPropertyChanged(propertyName);
				}

				// Resume base class events
				base.OnEventsResumed();
			}
		}

		/// <summary>
		/// Called when the <see cref="PropertyChanged"/> event occurs during resume events.
		/// </summary>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			// Fire event
			var handler = PropertyChanged;
			if (handler != null)
				_synchronization.Post(delegate { handler(this, new PropertyChangedEventArgs(propertyName)); }, null);
		}

		/// <summary>
		/// Sets a property with change checking and change notification.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		/// <param name="target">Target reference, i.e. a backing field.</param>
		/// <param name="value">Value to set.</param>
		/// <param name="propertyName">Property name, must match public property name else bindings will not see change.</param>
		/// <returns>True when changed.</returns>
		protected bool SetProperty<T>(ref T target, T value, string propertyName)
		{
			return SetProperty(ref target, value, propertyName, null, null);
		}

		/// <summary>
		/// Sets a property with change checking and change notification.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		/// <param name="target">Target reference, i.e. a backing field.</param>
		/// <param name="value">Value to set.</param>
		/// <param name="propertyName">Property name, must match public property name else bindings will not see change.</param>
		/// <param name="beforeAssign">
		/// Optional callback which is invoked before assignment and before firing the changed event, with the old and new values, e.g. to un-hook events or
		/// deactivate the old value.
		/// </param>
		/// <param name="afterAssign">
		/// Optional callback which is invoked after assignment but before firing the changed event, with the old and new values, e.g. to hook events or clean-up
		/// the old value.
		/// </param>
		/// <returns>True when changed.</returns>
		protected bool SetProperty<T>(ref T target, T value, string propertyName,
			SetPropertyEventHandler<T>? beforeAssign, SetPropertyEventHandler<T>? afterAssign)
		{
			// Do nothing when not changed
			if (EqualityComparer<T>.Default.Equals(target, value))
				return false;

			// Call optional "before assignment" method
			var oldValue = target;
			beforeAssign?.Invoke(this, ref target, oldValue, value, propertyName);

			// Set value
			target = value;

			// Call optional "after assignment" method
			afterAssign?.Invoke(this, ref target, oldValue, value, propertyName);

			// Fire event
			InvokePropertyChanged(propertyName);

			// Return changed
			return true;
		}

		#endregion Protected Methods
	}
}
