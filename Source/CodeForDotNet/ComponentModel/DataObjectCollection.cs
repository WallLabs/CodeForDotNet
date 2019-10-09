using CodeForDotNet.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CodeForDotNet.ComponentModel
{
	/// <summary>
	/// Collection of <see cref="DataObject"/> items supporting ID index and central event notification.
	/// </summary>
	public class DataObjectCollection<T> : ObservableCollection<T>, IEventCache, IDisposable
		where T : DataObject
	{
		#region Private Fields

		private static readonly object _syncRoot = new object();

		/// <summary>
		/// Counts the number of times the <see cref="SuspendEvents"/> method was called to decide whether events are currently suspended, when greater than
		/// zero. The value is decremented on each call to <see cref="ResumeEvents"/>.
		/// </summary>
		private int _suspendEventsCount;

		#endregion Private Fields

		#region Public Constructors

		/// <summary>
		/// Creates an empty instance.
		/// </summary>
		public DataObjectCollection()
		{
		}

		/// <summary>
		/// Creates an instance based on an existing enumeration.
		/// </summary>
		public DataObjectCollection(IEnumerable<T> enumeration)
			: base(enumeration)
		{
		}

		/// <summary>
		/// Creates an instance based on an existing list.
		/// </summary>
		public DataObjectCollection(List<T> list)
			: base(list)
		{
		}

		#endregion Public Constructors

		#region Private Destructors

		/// <summary>
		/// Frees unmanaged resources during finalization.
		/// </summary>
		~DataObjectCollection()
		{
			// Unmanaged dispose
			Dispose(false);
		}

		#endregion Private Destructors

		#region Public Events

		/// <summary>
		/// Fired when events are suspended the first time, i.e. is not fired when nested.
		/// </summary>
		public event EventHandler? EventsResumed;

		/// <summary>
		/// Fired when events are suspended the first time, i.e. is not fired when nested.
		/// </summary>
		public event EventHandler? EventsSuspended;

		/// <summary>
		/// Fired when an item <see cref="DataObject.DataChanged"/> event occurs. The sender remains as the item which originated the event.
		/// </summary>
		public event EventHandler<DataObjectChangeEventArgs>? ItemDataChanged;

		#endregion Public Events

		#region Public Properties

		/// <summary>
		/// Flags that events are current enabled, and will be fired immediately. This can be used by inheriting classes to determine whether to cache or fire
		/// events immediately, in conjunction with the ResumeEvents() override.
		/// </summary>
		public bool EventsAreEnabled { get { return _suspendEventsCount <= 0; } }

		/// <summary>
		/// Thread synchronization object.
		/// </summary>
		public object SyncRoot { get { return _syncRoot; } }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Compares two instances of this type for equality by value.
		/// </summary>
		public static bool operator !=(DataObjectCollection<T> object1, DataObjectCollection<T> object2)
		{
			return !(object1 is null) ? !object1.Equals(object2) : !(object2 is null);
		}

		/// <summary>
		/// Compares two instances of this type for equality by value.
		/// </summary>
		public static bool operator ==(DataObjectCollection<T> object1, DataObjectCollection<T> object2)
		{
			return !(object1 is null) ? object1.Equals(object2) : object2 is null;
		}

		/// <summary>
		/// Proactively frees resources owned by this object.
		/// </summary>
		public void Dispose()
		{
			// Full managed dispose
			Dispose(true);

			// Suppress finalization (no longer necessary)
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Compares this instance with another object by value.
		/// </summary>
		public override bool Equals(object obj)
		{
			// Lock changes to this object
			lock (_syncRoot)
			{
				// Compare type and nullability
				if (!(obj is DataObjectCollection<T> other))
					return false;

				// Compare values (with lock against changes)
				lock (other.SyncRoot)
					return ArrayExtensions.AreEqual(this, other);
			}
		}

		/// <summary>
		/// Gets a hash code based on the current values of this object.
		/// </summary>
		public override int GetHashCode()
		{
			// Lock changes
			lock (_syncRoot)
			{
				// Return hash
				return ArrayExtensions.GetHashCode(this);
			}
		}

		/// <summary>
		/// Resumes all events after SuspendEvents. Automatically fires any pending events when appropriate. Implements reference counting to detect when to
		/// finally resume events, at which point the Resume event is fired.
		/// </summary>
		public void ResumeEvents()
		{
			// Lock changes
			lock (_syncRoot)
			{
				// Decrement counter, do nothing when still suspended
				if (--_suspendEventsCount > 0)
					return;

				// Call method on all items
				foreach (var item in Items)
					item.ResumeEvents();

				// Fire event
				EventsResumed?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Disables all events until ResumeEvents is called. Implements reference counting to detect when to finally resume events, increasing performance by
		/// preventing un-necessary event handling.
		/// </summary>
		public void SuspendEvents()
		{
			// Lock changes
			lock (_syncRoot)
			{
				// Increment counter, do nothing when already suspended
				if (_suspendEventsCount++ > 0)
					return;

				// Call method on all items
				foreach (var item in Items)
					item.SuspendEvents();

				// Fire event
				EventsSuspended?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Proactively frees resources owned by this object.
		/// </summary>
		/// <remarks>Full dispose pattern is not implemented because there is no need to un-hook events during finalization.</remarks>
		protected virtual void Dispose(bool disposing)
		{
			// Free managed resources during dispose (not finalization)
			if (disposing)
			{
				// Un-hook all item events
				foreach (var item in Items)
					UnhookEvents(item);
			}
		}

		/// <summary>
		/// Hooks all monitored events of an item.
		/// </summary>
		protected virtual void HookEvents(T item)
		{
			// Validate.
			if (item is null) throw new ArgumentNullException(nameof(item));

			// Hook event.
			item.DataChanged += OnItemDataChanged;
		}

		/// <summary>
		/// Handles changes to the collection.
		/// </summary>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs @event)
		{
			// Validate.
			if (@event is null) throw new ArgumentNullException(nameof(@event));

			// Call base class method.
			base.OnCollectionChanged(@event);

			// Un-hook events from removed items.
			switch (@event.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
				case NotifyCollectionChangedAction.Replace:
					foreach (T item in @event.OldItems)
						UnhookEvents(item);
					break;
			}

			// Hook events to new items.
			switch (@event.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Replace:
					foreach (T item in @event.NewItems)
						HookEvents(item);
					break;
			}
		}

		/// <summary>
		/// Bubbles the <see cref="DataObject.DataChanged"/> event of an item in this collection.
		/// </summary>
		protected virtual void OnItemDataChanged(object sender, DataObjectChangeEventArgs e)
		{
			// Fire event
			ItemDataChanged?.Invoke(sender, e);
		}

		/// <summary>
		/// Un-hooks all monitored events of an item.
		/// </summary>
		protected virtual void UnhookEvents(T item)
		{
			// Validate.
			if (item is null) throw new ArgumentNullException(nameof(item));

			// Un-hook event.
			item.DataChanged -= OnItemDataChanged;
		}

		#endregion Protected Methods
	}
}
