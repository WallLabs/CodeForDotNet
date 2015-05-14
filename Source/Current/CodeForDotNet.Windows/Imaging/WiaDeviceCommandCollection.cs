using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Interop.Wia.DeviceCommands"/>.
    /// </summary>
    public class WiaDeviceCommandCollection : Collection<WiaDeviceCommand>, IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaDeviceCommandCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        [CLSCompliant(false)]
        public WiaDeviceCommandCollection(Interop.Wia.DeviceCommands interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException("interopCollection");

            // Add unmanaged collection items with managed wrappers
            foreach (Interop.Wia.DeviceCommand interopItem in interopCollection)
                Add(new WiaDeviceCommand(interopItem));
        }
 
        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaDeviceCommandCollection()
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
            try
            {
                // Dispose managed resources
                if (disposing)
                {
                    foreach (var item in Items)
                        item.Dispose();
                }
            }
            finally
            {
                // Release references to aid garbage collection
                Clear();
            }
        }

        #endregion

        #endregion
    }
}
