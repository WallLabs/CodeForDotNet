using System;
using System.Runtime.Versioning;
using CodeForDotNet.Collections;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.Items"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaItemCollection : DisposableCollection<WiaItem>
{
    #region Lifetime

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public WiaItemCollection()
    {
    }

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    public WiaItemCollection(Wia.Items interopCollection)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(interopCollection);

        // Add unmanaged collection items with managed wrappers
        foreach (Wia.Item? interopItem in interopCollection)
            Add(new WiaItem(interopItem!));
    }

    #endregion Lifetime
}
