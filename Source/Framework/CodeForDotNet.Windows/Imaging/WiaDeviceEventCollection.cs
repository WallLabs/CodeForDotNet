using System;
using System.Runtime.Versioning;
using CodeForDotNet.Collections;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.DeviceEvents"/>.
/// </summary>
[SupportedOSPlatform("windows")]
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
    public WiaDeviceEventCollection(Wia.DeviceEvents interopCollection)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(interopCollection);

        // Add unmanaged collection items with managed wrappers
        foreach (Wia.DeviceEvent? interopItem in interopCollection)
            Add(new WiaDeviceEvent(interopItem!));
    }

    #endregion Lifetime
}
