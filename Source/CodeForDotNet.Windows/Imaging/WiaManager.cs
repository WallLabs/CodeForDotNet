using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a WIA source, e.g. scanner or camera.
    /// </summary>
    public class WiaManager : DisposableObject
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaManager()
        {
            _wiaManager = new Wia.DeviceManager();
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
                if (_wiaManager != null)
                    Marshal.ReleaseComObject(_wiaManager);
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
        /// WIA device manager.
        /// </summary>
        private Wia.DeviceManager _wiaManager;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Gets information about currently available devices.
        /// </summary>
        public WiaDeviceInfoCollection GetDevices()
        {
            return new WiaDeviceInfoCollection(_wiaManager.DeviceInfos);
        }

        #endregion Public Methods
    }
}