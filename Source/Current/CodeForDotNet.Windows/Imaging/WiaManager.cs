using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Encapsulates a WIA source, e.g. scanner or camera.
    /// </summary>
    public class WiaManager : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates the object.
        /// </summary>
        public WiaManager()
        {
            _wiaManager = new Interop.Wia.DeviceManager();
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaManager()
        {
            // Unmanaged only dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalization as no longer necessary
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Frees resources owned by this object.
        /// </summary>
        /// <param name="disposing">True when called via <see cref="Dispose()"/>.</param>
        void Dispose(bool disposing)
        {
            // Dispose unmanaged resources
            if (_wiaManager != null)
                Marshal.ReleaseComObject(_wiaManager);
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// WIA device manager.
        /// </summary>
        Interop.Wia.DeviceManager _wiaManager;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information about currently available devices.
        /// </summary>
        public WiaDeviceInfoCollection GetDevices()
        {
            return new WiaDeviceInfoCollection(_wiaManager.DeviceInfos);
        }

        #endregion
    }
}
