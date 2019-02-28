using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// View object with intelligent property, data and event caching.
    /// </summary>
    public class ViewObject : PropertyStore
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with no <see cref="Parent"/>.
        /// </summary>
        public ViewObject()
        {
            _children = new ObservableCollection<IViewObject>();
            _children.CollectionChanged += OnChildrenChanged;
        }

        /// <summary>
        /// Creates and instance with the specified <see cref="Parent"/>.
        /// </summary>
        /// <param name="parent">Parent.</param>
        public ViewObject(IViewObject parent)
            : this()
        {
            _parent = parent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        public IViewObject Parent
        {
            get
            {
                lock (SyncRoot)
                    return _parent;
            }
            set
            {
                lock (SyncRoot)
                {
                    // Suspend events
                    SuspendEvents();
                    try
                    {
                        // Set new value, saving old
                        var oldParent = _parent;
                        _parent = value;

                        // Queue or update cached event
                        if (_parentChangeEventCache == null)
                        {
                            // Queue new event
                            _parentChangeEventCache = new ViewObjectParentChangedEventArgs(oldParent, value);
                        }
                        else
                        {
                            // Update queued event with new parent
                            _parentChangeEventCache.NewParent = value;
                        }
                    }
                    finally
                    {
                        // Resume events
                        ResumeEvents();
                    }
                }
            }
        }
        private IViewObject _parent;
        private ViewObjectParentChangedEventArgs _parentChangeEventCache;

        /// <summary>
        /// Gets a collection of child objects.
        /// </summary>
        public ObservableCollection<IViewObject> Children
        {
            get
            {
                lock (SyncRoot)
                    return _children;
            }
        }
        private readonly ObservableCollection<IViewObject> _children;

        #endregion

        #region Public Methods

        /// <summary>
        /// Invalidates the view, causing it to be re-rendered.
        /// </summary>
        /// <param name="includeChildren">True to call <see cref="InvalidateView"/> on all children, recursively.</param>
        /// <remarks>
        /// If events are suspended the request is cached, then rendering will be delayed until events are resumed.
        /// </remarks>
        public virtual void InvalidateView(bool includeChildren)
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Invalidate children when specified
                    if (includeChildren)
                    {
                        foreach (var child in _children)
                            child.InvalidateView(true);
                    }
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Invalidates the layout of this object, optionally cascading to child objects.
        /// </summary>
        /// <param name="includeChildren">True to call <see cref="InvalidateLayout"/> on all children, recursively.</param>
        /// <remarks>
        /// Property cache is invalidated immediately, including children when specifeid, also when events are suspended.
        /// </remarks>
        public virtual void InvalidateLayout(bool includeChildren)
        {
            lock (SyncRoot)
            {
                // Suspend events
                SuspendEvents();
                try
                {
                    // Invalidate children when specified
                    if (includeChildren)
                    {
                        foreach (var child in _children)
                            child.InvalidateLayout(true);
                    }

                    // Invalidate view (redraw necessary when layout changed)
                    InvalidateView(false);
                }
                finally
                {
                    // Resume events
                    ResumeEvents();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the <see cref="Parent"/> is changed.
        /// </summary>
        public event EventHandler<ViewObjectParentChangedEventArgs> ParentChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Hooks events and performs initialization when <see cref="Children"/> are added or removed.
        /// </summary>
        /// <param name="sender">Event initiator.</param>
        /// <param name="args">Event arguments.</param>
        /// <remarks>
        /// Inheritors must call this base class method first.
        /// </remarks>
        public virtual void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            // Handle specific cases
            lock (SyncRoot)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        // Hook events of new items
                        foreach (var newChild in args.NewItems.Cast<IViewObject>())
                        {
                            newChild.PropertyStoreChanged += OnChildPropertyChanged;
                            newChild.Disposed += OnChildDisposed;
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:

                        // Un-hook events of old items
                        foreach (var oldChild in args.OldItems.Cast<IViewObject>())
                        {
                            oldChild.PropertyStoreChanged -= OnChildPropertyChanged;
                            oldChild.Disposed -= OnChildDisposed;
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:

                        // Un-hook events of old items
                        foreach (var oldChild in args.OldItems.Cast<IViewObject>())
                        {
                            oldChild.PropertyStoreChanged -= OnChildPropertyChanged;
                            oldChild.Disposed -= OnChildDisposed;
                        }

                        // Hook events of new items
                        foreach (var newChild in args.NewItems.Cast<IViewObject>())
                        {
                            newChild.PropertyStoreChanged += OnChildPropertyChanged;
                            newChild.Disposed += OnChildDisposed;
                        }

                        break;

                    case NotifyCollectionChangedAction.Move:

                        // No change necessary
                        break;
                }
            }

            // Invalidate layout including children
            InvalidateLayout(true);
        }

        /// <summary>
        /// Removes children when they are disposed.
        /// </summary>
        protected virtual void OnChildDisposed(object sender, EventArgs e)
        {
            // Suspend events
            SuspendEvents();
            try
            {
                // Remove from child list (if present)
                var child = (IViewObject)sender;
                _children.Remove(child);
            }
            finally
            {
                // Resume events
                ResumeEvents();
            }
        }

        /// <summary>
        /// Called when the <see cref="IPropertyStore.PropertyStoreChanged"/> event is fired
        /// on any of the <see cref="Children"/>.
        /// </summary>
        /// <param name="sender">Event initiator.</param>
        /// <param name="arguments">Event arguments.</param>
        /// <remarks>
        /// Inheritors must call this base class method first.
        /// </remarks>
        [CLSCompliant(false)]
        public virtual void OnChildPropertyChanged(object sender, PropertyStoreChangeEventArgs arguments)
        {
            // Currently empty but still required to call for future base code support
        }

        /// <summary>
        /// Fires any cached events when events are resumed.
        /// </summary>
        protected override void OnEventsResumed()
        {
            lock (SyncRoot)
            {
                // Fire any cached base class events
                base.OnEventsResumed();

                // Do nothing more when no events are cached for this class
                if (_parentChangeEventCache == null) return;

                // Clear cache
                var args = _parentChangeEventCache;
                _parentChangeEventCache = null;

                // Fire event
                ParentChanged?.Invoke(this, args);
            }
        }

        #endregion
    }
}