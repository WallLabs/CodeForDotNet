using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceEvent"/>.
    /// </summary>
    public class WiaDeviceEvent : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaDeviceEvent(Wia.DeviceEvent deviceEvent)
        {
            _wiaDeviceEvent = deviceEvent;
        }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called from <see cref="IDisposable.Dispose()"/>,
        /// false when called during finalization.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                // Dispose unmanaged resources.
                Marshal.ReleaseComObject(_wiaDeviceEvent);
            }
            finally
            {
                // Call base class method to fire events and set status properties.
                base.Dispose(disposing);
            }
        }

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Wia.DeviceEvent"/>.
        /// </summary>
        private readonly Wia.DeviceEvent _wiaDeviceEvent;

        #endregion Private Fields

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

        #endregion Public Properties
    }
}