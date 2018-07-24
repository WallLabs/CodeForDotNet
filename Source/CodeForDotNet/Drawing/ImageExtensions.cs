using System;
using System.Drawing.Imaging;

namespace CodeForDotNet.Drawing
{
    /// <summary>
    /// Extensions for working with image classes.
    /// </summary>
    public static class ImageExtensions
    {
        #region Constants

        /// <summary>
        /// BMP file content type.
        /// </summary>
        public const string BmpContentType = "image/bmp";

        /// <summary>
        /// EMF file content type.
        /// </summary>
        public const string EmfContentType = "image/x-emf";

        /// <summary>
        /// WMF file content type.
        /// </summary>
        public const string WmfContentType = "image/wmf";

        /// <summary>
        /// GIF file content type.
        /// </summary>
        public const string GifContentType = "image/gif";

        /// <summary>
        /// TIF file content type.
        /// </summary>
        public const string TifContentType = "image/tif";

        /// <summary>
        /// ICO file content type.
        /// </summary>
        public const string IcoContentType = "image/ico";

        /// <summary>
        /// PNG file content type.
        /// </summary>
        public const string PngContentType = "image/png";

        /// <summary>
        /// JPG file content type.
        /// </summary>
        public const string JpgContentType = "image/jpg";

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the MIME content type of an <see cref="ImageFormat"/>.
        /// </summary>
        /// <param name="format">Image format.</param>
        /// <returns>Content type or null when not supported.</returns>
        public static string GetContentType(this ImageFormat format)
        {
            // Validate
            if (format == null) throw new ArgumentNullException("format");

            // Call overloaded method
            return GetContentType(format.Guid);
        }

        /// <summary>
        /// Gets the MIME content type of an <see cref="ImageFormat.Guid"/>.
        /// </summary>
        /// <param name="formatId">Image format Id.</param>
        /// <returns>Content type or null when not supported.</returns>
        public static string GetContentType(Guid formatId)
        {
            if (formatId == ImageFormat.Bmp.Guid ||
                formatId == ImageFormat.MemoryBmp.Guid) return BmpContentType;
            if (formatId == ImageFormat.Emf.Guid) return EmfContentType;
            if (formatId == ImageFormat.Wmf.Guid) return WmfContentType;
            if (formatId == ImageFormat.Gif.Guid) return GifContentType;
            if (formatId == ImageFormat.Exif.Guid ||
                formatId == ImageFormat.Tiff.Guid) return TifContentType;
            if (formatId == ImageFormat.Icon.Guid) return IcoContentType;
            if (formatId == ImageFormat.Png.Guid) return PngContentType;
            if (formatId == ImageFormat.Jpeg.Guid) return JpgContentType;
            return null;
        }

        /// <summary>
        /// Gets the <see cref="ImageFormat"/> for a MIME content type.
        /// </summary>
        /// <param name="contentType">MIME content type.</param>
        /// <returns>Image format or null when unsupported.</returns>
        public static ImageFormat GetFormat(string contentType)
        {
            if (String.CompareOrdinal(contentType, BmpContentType) == 0) return ImageFormat.Bmp;
            if (String.CompareOrdinal(contentType, EmfContentType) == 0) return ImageFormat.Emf;
            if (String.CompareOrdinal(contentType, WmfContentType) == 0) return ImageFormat.Wmf;
            if (String.CompareOrdinal(contentType, GifContentType) == 0) return ImageFormat.Gif;
            if (String.CompareOrdinal(contentType, TifContentType) == 0) return ImageFormat.Tiff;
            if (String.CompareOrdinal(contentType, IcoContentType) == 0) return ImageFormat.Icon;
            if (String.CompareOrdinal(contentType, PngContentType) == 0) return ImageFormat.Png;
            if (String.CompareOrdinal(contentType, JpgContentType) == 0) return ImageFormat.Jpeg;
            return null;
        }

        #endregion
    }
}
