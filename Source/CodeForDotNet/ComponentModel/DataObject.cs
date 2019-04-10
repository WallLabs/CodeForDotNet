using System;
using System.Collections.Generic;
using CodeForDotNet.Properties;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// CRUD data object with intelligent data change event caching, able to distinguish between
    /// instance and store changes, e.g. clearing pending instance events when store is read (which
    /// would overwrite any pending changes).
    /// </summary>
    public abstract class DataObject : PropertyStore, IDataObject
    {
        #region Private Fields

        /// <summary>
        /// List of all committed (i.e. <see cref="Create"/> or <see cref="Update"/>) property names
        /// which changed during <see cref="IEventCache.SuspendEvents"/>.
        /// </summary>
        private readonly List<Guid> _committedPropertiesChanged;

        /// <summary>
        /// List of all property names which changed during <see cref="IEventCache.SuspendEvents"/>
        /// since the last <see cref="Create"/> or <see cref="Update"/>.
        /// </summary>
        private readonly List<Guid> _instancePropertiesChanged;

        /// <summary>
        /// Changed property storage.
        /// </summary>
        private Dictionary<Guid, object> _changedProperties;

        /// <summary>
        /// Caches the last <see cref="DataChanged"/> event action when events are suspended. Only
        /// the most significant action is cached.
        /// </summary>
        private DataObjectChangeAction? _lastChangeAction;

        /// <summary>
        /// Original property storage.
        /// </summary>
        private Dictionary<Guid, object> _originalProperties;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected DataObject()
        {
            // Initialize members
            _originalProperties = new Dictionary<Guid, object>();
            _changedProperties = new Dictionary<Guid, object>();
            _committedPropertiesChanged = new List<Guid>();
            _instancePropertiesChanged = new List<Guid>();
        }

        #endregion Protected Constructors

        #region Public Events

        /// <summary>
        /// Fired when data for this object has changed.
        /// </summary>
        public event EventHandler<DataObjectChangeEventArgs> DataChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Indicates the current state of the data represented by this instance.
        /// </summary>
        public DataObjectState DataState { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the object in storage.
        /// </summary>
        /// <remarks>
        /// Only valid when the <see cref="DataState"/> is <see cref="DataObjectState.None"/>. Sets
        /// the <see cref="DataState"/> to <see cref="DataObjectState.Current"/> once successful
        /// </remarks>
        public void Create()
        {
            lock (SyncRoot)
            {
                // Assert state
                if (DataState != DataObjectState.None)
                    throw new InvalidOperationException(Resources.DataObjectCreateStateInvalid);

                // Suspend events
                SuspendEvents();
                try
                {
                    // Call implementor method to create object with current properties
                    _originalProperties = OnCreate(_changedProperties);
                    _changedProperties = new Dictionary<Guid, object>();

                    // Update state
                    DataState = DataObjectState.Current;

                    // Fire event
                    DoDataChanged(DataObjectChangeAction.Create, null);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Deletes the object from storage.
        /// </summary>
        /// <remarks>
        /// Only valid when the current <see cref="DataState"/> is
        /// <see cref="DataObjectState.Current"/> or <see cref="DataObjectState.Changed"/>. Sets the
        /// <see cref="DataState"/> to <see cref="DataObjectState.Deleted"/> once successful.
        /// </remarks>
        public virtual void Delete()
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Validate state
                    if (DataState != DataObjectState.Current &&
                        DataState != DataObjectState.Changed)
                        throw new InvalidOperationException(Resources.DataObjectDeleteStateInvalid);

                    // Call implementor method to delete object
                    OnDelete(_originalProperties);
                    _originalProperties = new Dictionary<Guid, object>();
                    _changedProperties = new Dictionary<Guid, object>();

                    // Update state
                    DataState = DataObjectState.Deleted;

                    // Fire event
                    DoDataChanged(DataObjectChangeAction.Delete, null);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Notifies this object that a related data object has changed.
        /// </summary>
        /// <param name="changed">Changed data object instance.</param>
        /// <param name="change">Change details.</param>
        public void NotifyChange(IDataObject changed, DataObjectChangeEventArgs change)
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
        /// Reads the object from storage.
        /// </summary>
        /// <remarks>
        /// Only valid when the <see cref="DataState"/> is not <see cref="DataObjectState.Deleted"/>.
        /// Sets the <see cref="DataState"/> to <see cref="DataObjectState.Current"/> once successful.
        /// </remarks>
        public virtual void Read()
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Validate state
                    if (DataState == DataObjectState.Deleted)
                        throw new InvalidOperationException(Resources.DataObjectReadStateInvalid);

                    // Call implementor method to read properties
                    _originalProperties = OnRead(_originalProperties);
                    _changedProperties = new Dictionary<Guid, object>();

                    // Update state
                    DataState = DataObjectState.Current;

                    // Fire event
                    DoDataChanged(DataObjectChangeAction.Read, null);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Updates the object in storage.
        /// </summary>
        /// <remarks>
        /// Only valid when the current <see cref="DataState"/> is
        /// <see cref="DataObjectState.Changed"/>. Sets the <see cref="DataState"/> to
        /// <see cref="DataObjectState.Current"/> once successful.
        /// </remarks>
        public virtual void Update()
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Validate state
                    if (DataState != DataObjectState.Changed)
                        throw new InvalidOperationException(Resources.DataObjectUpdateStateInvalid);

                    // Call implementor method to update properties
                    _originalProperties = OnUpdate(_changedProperties, _originalProperties);
                    _changedProperties = new Dictionary<Guid, object>();

                    // Update state
                    DataState = DataObjectState.Current;

                    // Fire event
                    DoDataChanged(DataObjectChangeAction.Update, null);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Fires or caches the DataChanged event.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="DataState"/> property to <see cref="DataObjectState.Changed"/> when
        /// properties change.
        /// </remarks>
        protected void DoDataChanged(DataObjectChangeAction action, Guid? propertyId)
        {
            lock (SyncRoot)
            {
                // Validate and update state when properties change
                if (action == DataObjectChangeAction.Property)
                {
                    if (DataState == DataObjectState.Deleted)
                        throw new InvalidOperationException();
                    DataState = DataObjectState.Changed;
                }

                // Fire or cache event
                if (EventsAreEnabled)
                {
                    // Fire event immediately
                    OnDataChanged(new DataObjectChangeEventArgs(action, propertyId.HasValue ? new[] { propertyId.Value } : null));
                }
                else
                {
                    // Cache event...

                    // Add any chaned property to instance cache
                    if (propertyId.HasValue && !_instancePropertiesChanged.Contains(propertyId.Value))
                        _instancePropertiesChanged.Add(propertyId.Value);

                    // Process commit or clear instance property change cache depending on action
                    switch (action)
                    {
                        case DataObjectChangeAction.Create:
                        case DataObjectChangeAction.Update:

                            // Commit all instance property changes
                            foreach (var instancePropertyId in _instancePropertiesChanged)
                            {
                                if (!_committedPropertiesChanged.Contains(instancePropertyId))
                                    _committedPropertiesChanged.Add(instancePropertyId);
                            }
                            _instancePropertiesChanged.Clear();

                            // Update is ignored if Create is cached
                            if ((action == DataObjectChangeAction.Update && !_lastChangeAction.HasValue && _lastChangeAction != DataObjectChangeAction.Create) ||
                                (action == DataObjectChangeAction.Create))
                                _lastChangeAction = action;
                            break;

                        case DataObjectChangeAction.Read:

                            // Clear all non-committed instance property changes on read
                            _instancePropertiesChanged.Clear();
                            if (_lastChangeAction.HasValue && _lastChangeAction.Value == DataObjectChangeAction.Property)
                                _lastChangeAction = null;

                            // Read is weakest action
                            if (!_lastChangeAction.HasValue)
                                _lastChangeAction = action;
                            break;

                        case DataObjectChangeAction.Delete:

                            // Clear all instance and committed property changes on delete
                            _committedPropertiesChanged.Clear();
                            _instancePropertiesChanged.Clear();

                            // Delete is strongest action, unless preceded by Create
                            if (!_lastChangeAction.HasValue || _lastChangeAction.Value != DataObjectChangeAction.Create)
                            {
                                // Delete overrides any other value
                                _lastChangeAction = action;
                            }
                            else
                            {
                                // Delete after Create cancels out all changes
                                _committedPropertiesChanged.Clear();
                                _instancePropertiesChanged.Clear();
                                _lastChangeAction = null;
                            }
                            break;

                        case DataObjectChangeAction.Property:

                            // Add property to instance cache
                            if (!propertyId.HasValue)
                                throw new ArgumentNullException(nameof(propertyId));
                            if (!_instancePropertiesChanged.Contains(propertyId.Value))
                                _instancePropertiesChanged.Add(propertyId.Value);

                            // Instance changes only override read
                            if (!_lastChangeAction.HasValue || _lastChangeAction.Value == DataObjectChangeAction.Read)
                                _lastChangeAction = action;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles change notification events, when this object is notified that a related data
        /// object has changed via the <see cref="NotifyChange"/> method.
        /// </summary>
        /// <remarks>
        /// Thread safe locking, <see cref="EventCache.SuspendEvents"/> and
        /// <see cref="EventCache.ResumeEvents"/> are handled by the caller. The base class
        /// implementation does nothing.
        /// </remarks>
        /// <param name="changed">Changed data object instance.</param>
        /// <param name="change">Change details.</param>
        protected virtual void OnChangeNotification(IDataObject changed, DataObjectChangeEventArgs change) { }

        /// <summary>
        /// Overridden by inheritors to create the object in storage.
        /// </summary>
        /// <param name="newProperties">Current property values.</param>
        /// <returns>Updated properties.</returns>
        protected abstract Dictionary<Guid, object> OnCreate(Dictionary<Guid, object> newProperties);

        /// <summary>
        /// Fires the <see cref="DataChanged"/> event.
        /// </summary>
        protected virtual void OnDataChanged(DataObjectChangeEventArgs change)
        {
            // Fire event
            DataChanged?.Invoke(this, change);
        }

        /// <summary>
        /// Overridden by inheritors to delete the object from storage.
        /// </summary>
        /// <param name="originalProperties">Original properties for concurrency check.</param>
        protected abstract void OnDelete(Dictionary<Guid, object> originalProperties);

        /// <summary>
        /// Called when events are resumed the last time, i.e. is not fired when nested. Fires the
        /// <see cref="DataChanged"/> event when changes are pending.
        /// </summary>
        protected override void OnEventsResumed()
        {
            lock (SyncRoot)
            {
                // Fire pending DataChanged event
                if (_lastChangeAction.HasValue)
                {
                    // Build event
                    var properties = new List<Guid>(_committedPropertiesChanged);
                    foreach (var property in _instancePropertiesChanged)
                    {
                        if (!properties.Contains(property))
                            properties.Add(property);
                    }
                    var args = new DataObjectChangeEventArgs(_lastChangeAction.Value, properties.ToArray());

                    // Clear cache
                    _committedPropertiesChanged.Clear();
                    _instancePropertiesChanged.Clear();
                    _lastChangeAction = null;

                    // Fire event
                    if (DataChanged != null)
                    {
                        OnDataChanged(args);
                    }
                }

                // Call base class method
                base.OnEventsResumed();
            }
        }

        /// <summary>
        /// Overridden by inheritors to read the object from storage.
        /// </summary>
        /// <param name="originalProperties">Original properties.</param>
        /// <returns>Updated properties.</returns>
        protected abstract Dictionary<Guid, object> OnRead(Dictionary<Guid, object> originalProperties);

        /// <summary>
        /// Overridden by inheritors to update the object in storage.
        /// </summary>
        /// <param name="changedProperties">Changed properties.</param>
        /// <param name="originalProperties">Original properties.</param>
        /// <returns>Updated properties.</returns>
        protected abstract Dictionary<Guid, object> OnUpdate(Dictionary<Guid, object> changedProperties, Dictionary<Guid, object> originalProperties);

        #endregion Protected Methods
    }
}
