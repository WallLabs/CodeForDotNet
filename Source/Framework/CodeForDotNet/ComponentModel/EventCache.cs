using System;
using System.Threading;

namespace CodeForDotNet.ComponentModel;

/// <summary>
/// Provides methods and events to implement suspend/resume and caching of events. Events are cached when they are not relevant until an atomic operation has
/// completed. This can give great performance gains, especially in graphics related applications.
/// </summary>
public abstract class EventCache : IEventCache
{
    /// <summary>
    /// Reference counter for Suspend/Resume events.
    /// </summary>
    private int _suspendEventsCount;

    /// <summary>
    /// Creates a stand-alone instance.
    /// </summary>
    protected EventCache()
    {
        // Initialize members
        SyncRoot = new();
        EventsAreEnabled = true;
    }

    /// <summary>
    /// Fired when events are suspended the first time, i.e. is not fired when nested.
    /// </summary>
    public event EventHandler? EventsResumed;

    /// <summary>
    /// Fired when events are suspended the first time, i.e. is not fired when nested.
    /// </summary>
    public event EventHandler? EventsSuspended;

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
    public Lock SyncRoot { get; private set; }

    /// <summary>
    /// Resumes all events after SuspendEvents. Automatically fires any pending events.
    /// </summary>
    public virtual void ResumeEvents()
    {
        lock (SyncRoot)
        {
            // Resume events (with reference counting).
            if (--_suspendEventsCount < 1)
            {
                // Level out any excessive ResumeEvent calls.
                _suspendEventsCount = 0;

                // Re-enable events.
                EventsAreEnabled = true;

                // Call event handlers.
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
            // Suspend events (with reference counting).
            if (++_suspendEventsCount < 2)
            {
                // Disable events the first time.
                EventsAreEnabled = false;

                // Level out any excessive ResumeEvent calls.
                _suspendEventsCount = 1;

                // Suspend
                OnEventsSuspended();
            }
        }
    }

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
}
