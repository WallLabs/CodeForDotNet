using System;
using CodeForDotNet.Collections;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="Wia.Formats"/>.
/// </summary>
public class WiaFormatCollection : DisposableCollection<object>
{
    #region Lifetime

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public WiaFormatCollection()
    {
    }

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    public WiaFormatCollection(Wia.Formats interopCollection)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(interopCollection);

        // Add unmanaged collection items with managed wrappers
        foreach (var interopItem in interopCollection)
            Add(interopItem!);
    }

    #endregion Lifetime
}
