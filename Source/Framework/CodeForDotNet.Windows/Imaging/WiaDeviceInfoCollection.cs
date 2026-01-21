using System;
using System.Runtime.Versioning;
using CodeForDotNet.Collections;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.DeviceInfos"/>.
/// </summary>
[SupportedOSPlatform("windows")]
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
        ArgumentNullException.ThrowIfNull(interopCollection);

        // Add unmanaged collection items with managed wrappers
        foreach (Wia.DeviceInfo? interopItem in interopCollection)
            Add(new WiaDeviceInfo(interopItem!));
    }

    #endregion Lifetime
}
