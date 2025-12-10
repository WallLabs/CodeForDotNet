using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging;

/// <summary>
/// Managed <see cref="WiaImageFile"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WiaImageFile : DisposableObject
{
    #region Private Fields

    /// <summary>
    /// Unmanaged <see cref="Wia.ImageFile"/>.
    /// </summary>
    private readonly Wia.ImageFile _wiaImageFile;
    private WiaPropertyCollection? _properties;

    #endregion Private Fields

    #region Internal Constructors

    /// <summary>
    /// Creates an instance to wrap the specified unmanaged object.
    /// </summary>
    internal WiaImageFile(Wia.ImageFile vector)
    {
        _wiaImageFile = vector;
    }

    #endregion Internal Constructors

    #region Public Properties

    /// <summary>
    /// Thread synchronization object.
    /// </summary>
    public static object SyncRoot { get; } = new object();

    /// <summary>
    /// Active frame.
    /// </summary>
    public int ActiveFrame
    {
        get => _wiaImageFile.ActiveFrame; set => _wiaImageFile.ActiveFrame = value;
    }

    /// <summary>
    /// Color palette.
    /// </summary>
    public WiaVector ArgbData
    {
        get
        {
            // Create wrapper first time
            if (field == null)
            {
                lock (SyncRoot)
                {
                    field ??= new WiaVector(_wiaImageFile.ARGBData);
                }
            }

            // Return wrapped value
            return field;
        }
    }

    /// <summary>
    /// File data.
    /// </summary>
    public WiaVector FileData
    {
        get
        {
            // Create wrapper first time
            if (field == null)
            {
                lock (SyncRoot)
                {
                    field ??= new WiaVector(_wiaImageFile.FileData);
                }
            }

            // Return wrapped value
            return field;
        }
    }

    /// <summary>
    /// File extension.
    /// </summary>
    public string FileExtension => _wiaImageFile.FileExtension;

    /// <summary>
    /// Image format ID.
    /// </summary>
    public string FormatId => _wiaImageFile.FormatID;

    /// <summary>
    /// Frame count.
    /// </summary>
    public int FrameCount => _wiaImageFile.FrameCount;

    /// <summary>
    /// Height in pixels.
    /// </summary>
    public int Height => _wiaImageFile.Height;

    /// <summary>
    /// Horizontal resolution.
    /// </summary>
    public double HorizontalResolution => _wiaImageFile.HorizontalResolution;

    /// <summary>
    /// Indicates pixel alpha component (transparency) is used.
    /// </summary>
    public bool IsAlphaPixelFormat => _wiaImageFile.IsAlphaPixelFormat;

    /// <summary>
    /// Indicates the picture is animated (contains multiple frames).
    /// </summary>
    public bool IsAnimated => _wiaImageFile.IsAnimated;

    /// <summary>
    /// Indicates the pixel extended format.
    /// </summary>
    public bool IsExtendedPixelFormat => _wiaImageFile.IsExtendedPixelFormat;

    /// <summary>
    /// Indicates the pixel indexed (color palette) format.
    /// </summary>
    public bool IsIndexedPixelFormat => _wiaImageFile.IsIndexedPixelFormat;

    /// <summary>
    /// Pixel color depth.
    /// </summary>
    public int PixelDepth => _wiaImageFile.PixelDepth;

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
                    _properties ??= new WiaPropertyCollection(_wiaImageFile.Properties);
                }
            }

            // Return wrapped value
            return _properties;
        }
    }

    /// <summary>
    /// Vertical resolution.
    /// </summary>
    public double VerticalResolution => _wiaImageFile.VerticalResolution;

    /// <summary>
    /// Width in pixels.
    /// </summary>
    public int Width => _wiaImageFile.Width;

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Loads the image from a file.
    /// </summary>
    public void LoadFile(string fileName)
    {
        _wiaImageFile.LoadFile(fileName);
    }

    /// <summary>
    /// Saves the image to a file.
    /// </summary>
    public void SaveFile(string fileName)
    {
        _wiaImageFile.SaveFile(fileName);
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
            }

            // Dispose unmanaged resources.
            if (_wiaImageFile != null)
                _ = Marshal.ReleaseComObject(_wiaImageFile);
        }
        finally
        {
            // Call base class method to fire events and set status properties.
            base.Dispose(disposing);
        }
    }

    #endregion Protected Methods
}
