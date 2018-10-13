using CodeForDotNet.Collections;
using System;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceEvents"/>.
    /// </summary>
    public class WiaDeviceEventCollection : DisposableCollection<WiaDeviceEvent>
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
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.DeviceEvent interopItem in interopCollection)
                Add(new WiaDeviceEvent(interopItem));
        }

        #endregion Lifetime
    }
}