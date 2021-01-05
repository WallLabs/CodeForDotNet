using CodeForDotNet.Collections;
using System;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceInfos"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public class WiaDeviceInfoCollection : DisposableCollection<WiaDeviceInfo>
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public WiaDeviceInfoCollection()
        {
        }

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        public WiaDeviceInfoCollection(Wia.DeviceInfos interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.DeviceInfo? interopItem in interopCollection)
                Add(new WiaDeviceInfo(interopItem!));
        }

        #endregion Lifetime
    }
}
