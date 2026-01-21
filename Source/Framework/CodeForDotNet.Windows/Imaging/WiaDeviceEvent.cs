using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.DeviceEvent"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaDeviceEvent : DisposableObject
{
    #region Lifetime

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    internal WiaDeviceEvent(Wia.DeviceEvent deviceEvent)
    {
        _wiaDeviceEvent = deviceEvent;
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
            _ = Marshal.ReleaseComObject(_wiaDeviceEvent);
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
    /// Unmanaged <see cref="Wia.DeviceEvent"/>.
    /// </summary>
    private readonly Wia.DeviceEvent _wiaDeviceEvent;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    /// ID.
    /// </summary>
    public string Id => _wiaDeviceEvent.EventID;

    /// <summary>
    /// Name.
    /// </summary>
    public string Name => _wiaDeviceEvent.Name;

    /// <summary>
    /// Event type.
    /// </summary>
    public WiaDeviceEventType EventType => (WiaDeviceEventType)(int)_wiaDeviceEvent.Type;

    /// <summary>
    /// Description.
    /// </summary>
    public string Description => _wiaDeviceEvent.Description;

    #endregion Public Properties
}
