using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceEvent"/>.
    /// </summary>
    public class WiaDeviceEvent : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceEvent(Wia.DeviceEvent deviceEvent)
        {
            _wiaDeviceEvent = deviceEvent;
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaDeviceEvent()
        {
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose
            Dispose(true);

            // Suppress finalization (it is no longer necessary)
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        /// <param name="disposing">
        /// True when called from <see cref="Dispose()"/>,
        /// false when called during finalization.</param>
        void Dispose(bool disposing)
        {
            // Dispose unmanaged resources
            Marshal.ReleaseComObject(_wiaDeviceEvent);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Wia.DeviceEvent"/>.
        /// </summary>
        readonly Wia.DeviceEvent _wiaDeviceEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// ID.
        /// </summary>
        public string Id { get { return _wiaDeviceEvent.EventID; } }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get { return _wiaDeviceEvent.Name; } }

        /// <summary>
        /// Event type.
        /// </summary>
        public WiaDeviceEventType EventType
        {
            get { return (WiaDeviceEventType)(int)_wiaDeviceEvent.Type; }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get { return _wiaDeviceEvent.Description; } }

        #endregion
    }
}
