using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.WiaEventFlag"/>.
/// </summary>
public enum WiaDeviceEventType
{
    /// <summary>
    /// Notification event.
    /// </summary>
    Notification = 1,

    /// <summary>
    /// Action event.
    /// </summary>
    ActionEvent = 2
}
