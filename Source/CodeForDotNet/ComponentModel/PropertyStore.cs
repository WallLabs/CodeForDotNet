using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Object containing uniquely identifiable properties, with intelligent change notification and event caching.
    /// </summary>
    public abstract class PropertyStore : EventCache, IPropertyStore
    {
        #region Private Fields

        /// <summary>
        /// List of all property IDs which changed during <see cref="IEventCache.SuspendEvents"/>.
        /// </summary>
        private readonly List<Guid> _changedProperties;

        /// <summary>
        /// List of property IDs which will be automatically disposed when changed or this instance is disposed.
        /// </summary>
        private readonly List<Guid> _disposePropertyIDs;

        /// <summary>
        /// Property value storage.
        /// </summary>
        private readonly Dictionary<Guid, object> _properties;

        /// <summary>
        /// Property ID to name mappings, for compatibility with <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        private readonly Dictionary<Guid, string> _propertyNames;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected PropertyStore()
        {
            // Initialize members
            _properties = [];
            _propertyNames = [];
            _changedProperties = [];
            _disposePropertyIDs = [];
        }

        #endregion Protected Constructors

        #region Private Destructors

        /// <summary>
        /// Overrides the finalizer to ensure any available dispose logic is called.
        /// </summary>
        ~PropertyStore()
        {
            // Dispose only once
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;

            // Dispose only un-managed resources
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Events

        /// <summary>
        /// Fires after this object has been Disposed. Use this event to ensure all references are invalidated and any dependent objects are also Disposed or released.
        /// </summary>
        public event EventHandler? Disposed;

        /// <summary>
        /// Fires when the Dispose method is called on this object (except when garbage collected).
        /// </summary>
        public event EventHandler? Disposing;

        /// <summary>
        /// Fired when properties of this object are changed, used to notify the standard .NET component model.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fired when properties of this object are changed, used to notify our enhanced component model.
        /// </summary>
        public event EventHandler<PropertyStoreChangeEventArgs>? PropertyStoreChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Indicated that this object has been disposed. When this flag is TRUE, do not use this object in any way.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Indicates that this object is committed to the process of disposing. When this flag is TRUE, do not pass any references or queue it for processing.
        /// </summary>
        public bool IsDisposing { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Clears a property value if it exists, disposing any current value when flagged.
        /// </summary>
        /// <param name="id">Property ID.</param>
        public void ClearProperty(Guid id)
        {
            lock (SyncRoot)
            {
                // Check if property exists
                if (_properties.ContainsKey(id))
                {
                    // Suspend events
                    SuspendEvents();
                    try
                    {
                        // Dispose old value if necessary
                        DisposeProperty(id);

                        // Clear value
                        _properties[id] = null!;

                        // Fire event
                        DoPropertyChanged(id);
                    }
                    finally
                    {
                        // Resume events
                        ResumeEvents();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the property value exists.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <returns>True when exists, otherwise false.</returns>
        public bool ContainsProperty(Guid id)
        {
            lock (SyncRoot)
                return _properties.ContainsKey(id);
        }

        /// <summary>
        /// Checks if the property has a value.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <returns>True when present, otherwise false.</returns>
        public bool ContainsPropertyValue(Guid id)
        {
            lock (SyncRoot)
                return _properties[id] != null;
        }

        /// <summary>
        /// Frees all resources held by this object, immediately (rather than waiting for Garbage Collection). This can provide a performance increase and avoid
        /// memory congestion on objects that hold many or "expensive" (large) external resources.
        /// </summary>
        /// <remarks>
        /// First fires the Disposing event, which could cancel this operation. When not canceled, calls the <see cref="Dispose(bool)"/> method, which should be
        /// overridden in inheriting classes to release their local resources. Finally fires the Disposed event. Use the IsDisposing and IsDisposed properties to
        /// avoid using objects that are about to or have been disposed.
        /// </remarks>
        public void Dispose()
        {
            // Dispose only once
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;
            try
            {
                // Fire disposing event
                Disposing?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                try
                {
                    // Full managed dispose
                    Dispose(true);
                }
                finally
                {
                    // Do not finalize
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the ID does not exist.</exception>
        public T GetProperty<T>(Guid id)
        {
            lock (SyncRoot)
            {
                if (_properties.ContainsKey(id))
                    return (T)_properties[id];
                throw new ArgumentOutOfRangeException(nameof(id));
            }
        }

        /// <summary>
        /// Gets a property value, or a default when it doesn't exist.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Property value or the default when it doesn't exist.</returns>
        public T GetProperty<T>(Guid id, T defaultValue)
        {
            lock (SyncRoot)
            {
                if (_properties.ContainsKey(id))
                    return (T)_properties[id];
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets all property IDs.
        /// </summary>
        public Collection<Guid> GetPropertyIds()
        {
            lock (SyncRoot)
            {
                return new Collection<Guid>(_properties.Keys.ToList());
            }
        }

        /// <summary>
        /// Notifies this object that a related <see cref="IPropertyStore"/> has changed.
        /// </summary>
        /// <param name="changed">Changed object instance.</param>
        /// <param name="change">Change details.</param>
        public virtual void NotifyChange(IPropertyStore changed, PropertyStoreChangeEventArgs change)
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Notify subclass
                    OnChangeNotification(changed, change);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Sets multiple properties.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="properties"/> parameter is null or empty.</exception>
        public void SetProperties(IDictionary<Guid, object> properties)
        {
            lock (SyncRoot)
            {
                // Validate
                if (properties == null || properties.Count == 0) throw new ArgumentNullException(nameof(properties));

                // Suspend events
                SuspendEvents();
                try
                {
                    // Set each property
                    foreach (var property in properties)
                        SetProperty(property.Key, property.Value);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Sets or adds a property value.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <param name="value">Property value.</param>
        public void SetProperty<T>(Guid id, T value)
        {
            lock (SyncRoot)
            {
                // Validate
                if (!_properties.ContainsKey(id))
                    throw new ArgumentOutOfRangeException(nameof(id));

                // Dispose old value if necessary
                DisposeProperty(id);

                // Set new value
                _properties[id] = value!;

                // Fire event
                DoPropertyChanged(id);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Inheritors implement the <see cref="Dispose(bool)"/> method to dispose resources accordingly, depending on whether they have been called proactively
        /// or automatically via the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// True when called proactively, i.e. Not during garbage collection. Managed resources should not be accessed when this is False, just references and
        /// unmanaged resources released.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                // Set second flag to indicate Disposed state
                IsDisposed = true;

                // Dispose properties
                if (_properties != null)
                {
                    if (_disposePropertyIDs != null)
                    {
                        foreach (var propertyId in _disposePropertyIDs)
                            DisposeProperty(propertyId);
                    }
                }
            }
            finally
            {
                // Fire Disposed event
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles change notification events, when this object is notified that a related view object has changed via the <see cref="NotifyChange"/> method.
        /// </summary>
        /// <remarks>
        /// Thread safe locking, <see cref="EventCache.SuspendEvents"/> and <see cref="EventCache.ResumeEvents"/> are handled by the caller. The base class
        /// implementation does nothing.
        /// </remarks>
        /// <param name="changed">Changed view object instance.</param>
        /// <param name="change">Change details.</param>
        protected virtual void OnChangeNotification(IPropertyStore changed, PropertyStoreChangeEventArgs change) { }

        /// <summary>
        /// Called when events are resumed the last time, i.e. is not fired when nested. Fires the <see cref="PropertyStoreChanged"/> event when changes are pending.
        /// </summary>
        protected override void OnEventsResumed()
        {
            lock (SyncRoot)
            {
                // Fire pending PropertyChanged event
                if (_changedProperties.Count > 0)
                {
                    // Build event
                    var args = new PropertyStoreChangeEventArgs(_changedProperties.ToArray());

                    // Clear cache
                    _changedProperties.Clear();

                    // Fire events
                    OnPropertyChanged(args);
                }

                // Call base class method
                base.OnEventsResumed();
            }
        }

        /// <summary>
        /// Fires the <see cref="PropertyStoreChanged"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyStoreChangeEventArgs change)
        {
            // Validate
            if (change == null) throw new ArgumentNullException(nameof(change));

            // Fire events
            PropertyStoreChanged?.Invoke(this, change);
            if (PropertyChanged != null)
            {
                foreach (var propertyId in change.Keys)
                {
                    var propertyName = _propertyNames[propertyId];
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Registers a new property and default value and optional automatic call to <see cref="IDisposable.Dispose()"/> when the value is changed or this
        /// object is disposed.
        /// </summary>
        /// <param name="id">Property ID for usage with this class.</param>
        /// <param name="name">Property name for compatibility with <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="dispose">Enables the automatic dispose option.</param>
        /// <remarks>Called by inheritors during initialization to register their properties. Does not trigger any events.</remarks>
        protected void RegisterProperty<T>(Guid id, string name, T defaultValue, bool dispose)
        {
            lock (SyncRoot)
            {
                // Validate
                if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

                // Check if already exists
                if (_properties.ContainsKey(id))
                    throw new ArgumentOutOfRangeException(nameof(id));
                if (_propertyNames.ContainsValue(name))
                    throw new ArgumentOutOfRangeException(nameof(name));

                // Add new property
                _properties.Add(id, defaultValue!);
                _propertyNames.Add(id, name);

                // Add to dispose list when option set
                if (dispose)
                    _disposePropertyIDs.Add(id);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Disposes and clears property if flagged.
        /// </summary>
        /// <param name="id">Property ID.</param>
        private void DisposeProperty(Guid id)
        {
            // Check if flagged
            if (_disposePropertyIDs.Contains(id))
            {
                // Get current value
                if (_properties[id] is IDisposable oldValue)
                {
                    // Dispose when exists
                    oldValue.Dispose();
                    _properties[id] = null!;
                }
            }
        }

        /// <summary>
        /// Fires or caches the <see cref="PropertyStoreChanged"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        private void DoPropertyChanged(Guid propertyId)
        {
            lock (SyncRoot)
            {
                // Fire or cache event
                if (EventsAreEnabled)
                {
                    // Fire event
                    OnPropertyChanged(new PropertyStoreChangeEventArgs(new[] { propertyId }));
                }
                else
                {
                    // Cache event
                    _changedProperties.Add(propertyId);
                }
            }
        }

        #endregion Private Methods
    }
}
