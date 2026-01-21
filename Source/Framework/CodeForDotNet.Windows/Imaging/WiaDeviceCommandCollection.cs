using System;
using System.Runtime.Versioning;
using CodeForDotNet.Collections;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.DeviceCommands"/>.
/// </summary>
[SupportedOSPlatform("windows")]
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
    public WiaDeviceCommandCollection(Wia.DeviceCommands interopCollection)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(interopCollection);

        // Add unmanaged collection items with managed wrappers
        foreach (Wia.DeviceCommand? interopItem in interopCollection)
            Add(new WiaDeviceCommand(interopItem!));
    }

    #endregion Lifetime
}
