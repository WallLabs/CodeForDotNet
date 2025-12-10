using System;
using CodeForDotNet.ComponentModel;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace CodeForDotNet.WindowsUniversal.UI.Controls;

/// <summary>
/// Control implementing <see cref="IEventCache"/> for efficient event handling, thread safety and increased performance.
/// </summary>
public abstract class EventControl : Control, IEventCache
{
    #region Private Fields

    /// <summary>
    /// Reference counter for Suspend/Resume events.
    /// </summary>
    private int _suspendEventsCount;

    #endregion Private Fields

    #region Protected Constructors

    /// <summary>
    /// Creates a stand-alone instance.
    /// </summary>
    protected EventControl()
    {
        // Initialize members
        SyncRoot = new object();
        EventsAreEnabled = true;
    }

    #endregion Protected Constructors

    #region Public Events

    /// <summary>
    /// Fired when events are suspended the first time, i.e. is not fired when nested.
    /// </summary>
    public event EventHandler? EventsResumed;

    /// <summary>
    /// Fired when events are suspended the first time, i.e. is not fired when nested.
    /// </summary>
    public event EventHandler? EventsSuspended;

    #endregion Public Events

    #region Public Properties

    /// <summary>
    /// Flags that events are current enabled, and will be fired immediately. This can be used by inheriting classes to determine whether to cache or fire
    /// events immediately, in conjunction with the ResumeEvents() override.
    /// </summary>
    public bool EventsAreEnabled { get; private set; }

    /// <summary>
    /// Thread synchronization object.
    /// </summary>
    /// <remarks>
    /// Lock this object when you read or write properties of this object which must be complete as a batch before any other threads enter the section, e.g.
    /// during data load or save operations.
    /// </remarks>
    public object SyncRoot { get; private set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Resumes all events after SuspendEvents. Automatically fires any pending events.
    /// </summary>
    public virtual void ResumeEvents()
    {
        lock (SyncRoot)
        {
            // Resume events (with reference counting)
            if (--_suspendEventsCount < 1)
            {
                // Finally re-enable events
                _suspendEventsCount = 0;            // Level out any excessive ResumeEvent calls

                // Re-enable events
                EventsAreEnabled = true;

                // Call event handlers before enabling events to cache any recursion
                OnEventsResumed();
            }
        }
    }

    /// <summary>
    /// Disables all events until ResumeEvents is called.
    /// </summary>
    public virtual void SuspendEvents()
    {
        lock (SyncRoot)
        {
            // Suspend events (with reference counting)
            if (++_suspendEventsCount < 2)
            {
                // Disable events the first time
                EventsAreEnabled = false;
                _suspendEventsCount = 1;            // Level out any excessive ResumeEvent calls

                // Suspend
                OnEventsSuspended();
            }
        }
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    /// Called when events are resumed the last time, i.e. is not fired when nested. Fires the <see cref="EventsResumed"/> event.
    /// </summary>
    protected virtual void OnEventsResumed()
    {
        // Fire event
        EventsResumed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when events are suspended the first time, i.e. is not fired when nested. Fires the <see cref="EventsSuspended"/> event.
    /// </summary>
    protected virtual void OnEventsSuspended()
    {
        // Fire event
        EventsSuspended?.Invoke(this, EventArgs.Empty);
    }

    #endregion Protected Methods
}
