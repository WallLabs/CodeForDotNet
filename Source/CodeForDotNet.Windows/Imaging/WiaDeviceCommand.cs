using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.DeviceCommand"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaDeviceCommand : DisposableObject
{
    #region Lifetime

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    internal WiaDeviceCommand(Wia.DeviceCommand deviceCommand)
    {
        _wiaDeviceCommand = deviceCommand;
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
            _ = Marshal.ReleaseComObject(_wiaDeviceCommand);
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
    /// Unmanaged <see cref="Wia.DeviceCommand"/>.
    /// </summary>
    private readonly Wia.DeviceCommand _wiaDeviceCommand;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    /// ID.
    /// </summary>
    public string Id => _wiaDeviceCommand.CommandID;

    /// <summary>
    /// Name.
    /// </summary>
    public string Name => _wiaDeviceCommand.Name;

    /// <summary>
    /// Description.
    /// </summary>
    public string Description => _wiaDeviceCommand.Description;

    #endregion Public Properties
}
