using System;
using CodeForDotNet.Threading;

namespace CodeForDotNet.ComponentModel
{
    /// <summary>
    /// Provides methods and events to implement suspend/resume and caching of events. Events are
    /// cached when they are not relevant until an atomic operation has completed. This can give
    /// great performance gains, especially in graphics related applications.
    /// </summary>
    public interface IEventCache : IThreadSafe
    {
        #region Public Events

        /// <summary>
        /// Fired when events are suspended the first time, i.e. is not fired when nested.
        /// </summary>
        event EventHandler EventsResumed;

        /// <summary>
        /// Fired when events are suspended the first time, i.e. is not fired when nested.
        /// </summary>
        event EventHandler EventsSuspended;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Flags that events are current enabled, and will be fired immediately. This can be used by
        /// inheriting classes to determine whether to cache or fire events immediately, in
        /// conjunction with the ResumeEvents() override.
        /// </summary>
        bool EventsAreEnabled { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resumes all events after SuspendEvents. Automatically fires any pending events when
        /// appropriate. Implements reference counting to detect when to finally resume events, at
        /// which point the Resume event is fired.
        /// </summary>
        void ResumeEvents();

        /// <summary>
        /// Disables all events until ResumeEvents is called. Implements reference counting to detect
        /// when to finally resume events, increasing performance by preventing un-necessary event handling.
        /// </summary>
        void SuspendEvents();

        #endregion Public Methods
    }
}
