using System;
using System.Collections.ObjectModel;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceEvents"/>.
    /// </summary>
    public class WiaDeviceEventCollection : Collection<WiaDeviceEvent>, IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaDeviceEventCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        [CLSCompliant(false)]
        public WiaDeviceEventCollection(Wia.DeviceEvents interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException("interopCollection");

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.DeviceEvent interopItem in interopCollection)
                Add(new WiaDeviceEvent(interopItem));
        }

        #region IDisposable

        /// <summary>
        /// Calls dispose during finalization (if it has not been called already).
        /// </summary>
        ~WiaDeviceEventCollection()
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
