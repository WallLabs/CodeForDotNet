using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Encapsulates a <see cref="Item"/> in managed code.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaItem : DisposableObject
{
    #region Private Fields

    /// <summary>
    /// Unmanaged <see cref="Wia.Item"/>.
    /// </summary>
    private readonly Wia.Item _wiaItem;

    private WiaDeviceCommandCollection? _commands;

    private WiaFormatCollection? _formats;

    private WiaPropertyCollection? _properties;

    #endregion Private Fields

    #region Internal Constructors

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    internal WiaItem(Wia.Item item)
    {
        _wiaItem = item;
    }

    #endregion Internal Constructors

    #region Public Properties

    /// <summary>
    /// Thread synchronization object.
    /// </summary>
    public static object SyncRoot { get; } = new object();

    /// <summary>
    /// Commands.
    /// </summary>
    public WiaDeviceCommandCollection Commands
    {
        get
        {
            // Create wrapper first time
            if (_commands == null)
            {
                lock (SyncRoot)
                {
                    _commands ??= new WiaDeviceCommandCollection(_wiaItem.Commands);
                }
            }

            // Return wrapped value
            return _commands;
        }
    }

    /// <summary>
    /// Formats.
    /// </summary>
    public WiaFormatCollection Formats
    {
        get
        {
            // Create wrapper first time
            if (_formats == null)
            {
                lock (SyncRoot)
                {
                    _formats ??= new WiaFormatCollection(_wiaItem.Formats);
                }
            }

            // Return wrapped value
            return _formats;
        }
    }

    /// <summary>
    /// Item identifier.
    /// </summary>
    public string Id => _wiaItem.ItemID;

    /// <summary>
    /// Item.
    /// </summary>
    public object Item => _wiaItem.WiaItem;

    /// <summary>
    /// Properties.
    /// </summary>
    public WiaPropertyCollection Properties
    {
        get
        {
            // Create wrapper first time
            if (_properties == null)
            {
                lock (SyncRoot)
                {
                    _properties ??= new WiaPropertyCollection(_wiaItem.Properties);
                }
            }

            // Return wrapped value
            return _properties;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Executes a command.
    /// </summary>
    public WiaItem ExecuteCommand(string commandId)
    {
        var wiaItem = _wiaItem.ExecuteCommand(commandId);
        return new WiaItem(wiaItem);
    }

    /// <summary>
    /// Transfers using the default format ID.
    /// </summary>
    /// <returns></returns>
    public object Transfer()
    {
        // Call overloaded method
        return Transfer(WiaConstants.DefaultFormatId);
    }

    /// <summary>
    /// Transfers using a specific format ID.
    /// </summary>
    public object Transfer(string formatId)
    {
        return _wiaItem.Transfer(formatId);
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    /// Frees resources owned by this instance.
    /// </summary>
    /// <param name="disposing">True when called from <see cref="IDisposable.Dispose()"/>, false when called during finalization.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Dispose managed resources.
            if (disposing)
            {
                _properties?.Dispose();
                _formats?.Dispose();
                _commands?.Dispose();
            }

            // Dispose unmanaged resources.
            if (_wiaItem != null)
                _ = Marshal.ReleaseComObject(_wiaItem);
        }
        finally
        {
            // Call base class method to fire events and set status properties.
            base.Dispose(disposing);
        }
    }

    #endregion Protected Methods
}
