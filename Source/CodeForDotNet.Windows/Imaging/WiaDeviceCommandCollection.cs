﻿using CodeForDotNet.Collections;
using System;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="Wia.DeviceCommands"/>.
    /// </summary>
    public class WiaDeviceCommandCollection : DisposableCollection<WiaDeviceCommand>
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
        public WiaDeviceCommandCollection(Wia.DeviceCommands interopCollection)
        {
            // Validate
            if (interopCollection == null) throw new ArgumentNullException(nameof(interopCollection));

            // Add unmanaged collection items with managed wrappers
            foreach (Wia.DeviceCommand interopItem in interopCollection)
                Add(new WiaDeviceCommand(interopItem));
        }

        #endregion Lifetime
    }
}