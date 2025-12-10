using System;
using System.Drawing.Imaging;

namespace CodeForDotNet.Windows.Drawing;

/// <summary>
/// Extensions for working with image classes.
/// </summary>
public static class ImageExtensions
{
    #region Public Fields

    /// <summary>
    /// BMP file content type.
    /// </summary>
    public const string BmpContentType = "image/bmp";

    /// <summary>
    /// EMF file content type.
    /// </summary>
    public const string EmfContentType = "image/x-emf";

    /// <summary>
    /// GIF file content type.
    /// </summary>
    public const string GifContentType = "image/gif";

    /// <summary>
    /// ICO file content type.
    /// </summary>
    public const string IcoContentType = "image/ico";

    /// <summary>
    /// JPG file content type.
    /// </summary>
    public const string JpgContentType = "image/jpg";

    /// <summary>
    /// PNG file content type.
    /// </summary>
    public const string PngContentType = "image/png";

    /// <summary>
    /// TIF file content type.
    /// </summary>
    public const string TifContentType = "image/tif";

    /// <summary>
    /// WMF file content type.
    /// </summary>
    public const string WmfContentType = "image/wmf";

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    /// Gets the MIME content type of an <see cref="ImageFormat"/>.
    /// </summary>
    /// <param name="format">Image format.</param>
    /// <returns>Content type or null when not supported.</returns>
    public static string? GetContentType(this ImageFormat format)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(format);

        // Call overloaded method
        return GetContentType(format.Guid);
    }

    /// <summary>
    /// Gets the MIME content type of an <see cref="ImageFormat.Guid"/>.
    /// </summary>
    /// <param name="formatId">Image format Id.</param>
    /// <returns>Content type or null when not supported.</returns>
    public static string? GetContentType(Guid formatId)
    {
        if (formatId == ImageFormat.Bmp.Guid ||
            formatId == ImageFormat.MemoryBmp.Guid) return BmpContentType;
        if (formatId == ImageFormat.Emf.Guid) return EmfContentType;
        return formatId == ImageFormat.Wmf.Guid
            ? WmfContentType
            : formatId == ImageFormat.Gif.Guid
            ? GifContentType
            : formatId == ImageFormat.Exif.Guid ||
            formatId == ImageFormat.Tiff.Guid
            ? TifContentType
            : formatId == ImageFormat.Icon.Guid
            ? IcoContentType
            : formatId == ImageFormat.Png.Guid ? PngContentType : formatId == ImageFormat.Jpeg.Guid ? JpgContentType : null;
    }

    /// <summary>
    /// Gets the <see cref="ImageFormat"/> for a MIME content type.
    /// </summary>
    /// <param name="contentType">MIME content type.</param>
    /// <returns>Image format or null when unsupported.</returns>
    public static ImageFormat? GetFormat(string contentType)
    {
        if (string.Equals(contentType, BmpContentType, StringComparison.OrdinalIgnoreCase)) return ImageFormat.Bmp;
        if (string.Equals(contentType, EmfContentType, StringComparison.OrdinalIgnoreCase)) return ImageFormat.Emf;
        return string.Equals(contentType, WmfContentType, StringComparison.OrdinalIgnoreCase)
            ? ImageFormat.Wmf
            : string.Equals(contentType, GifContentType, StringComparison.OrdinalIgnoreCase)
            ? ImageFormat.Gif
            : string.Equals(contentType, TifContentType, StringComparison.OrdinalIgnoreCase) ? ImageFormat.Tiff
            : string.Equals(contentType, IcoContentType, StringComparison.OrdinalIgnoreCase) ? ImageFormat.Icon
            : string.Equals(contentType, PngContentType, StringComparison.OrdinalIgnoreCase) ? ImageFormat.Png
            : string.Equals(contentType, JpgContentType, StringComparison.OrdinalIgnoreCase) ? ImageFormat.Jpeg : null;
    }

    #endregion Public Methods
}
