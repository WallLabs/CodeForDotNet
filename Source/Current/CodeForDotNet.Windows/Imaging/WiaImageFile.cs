using System;
using System.Runtime.InteropServices;
using Wia = Interop.Wia;

namespace CodeForDotNet.Windows.Imaging
{
    /// <summary>
    /// Managed <see cref="WiaImageFile"/>.
    /// </summary>
    public class WiaImageFile : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance to wrap the specified unmanaged object.
        /// </summary>
        internal WiaImageFile(Wia.ImageFile vector)
        {
            _wiaImageFile = vector;
        }

        #region IDisposable

        /// <summary>
        /// Disposes unmanaged resources during finalization.
        /// </summary>
        ~WiaImageFile()
        {
            // Unmanaged only dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalization as no longer necessary
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Frees resources owned by this object.
        /// </summary>
        /// <param name="disposing">True when called via <see cref="Dispose()"/>.</param>
        void Dispose(bool disposing)
        {
            try
            {
                // Dispose managed resources
                if (disposing)
                {
                    if (_properties != null) _properties.Dispose();
                }
            }
            finally
            {
                // Dispose unmanaged resources
                if (_wiaImageFile != null)
                    Marshal.ReleaseComObject(_wiaImageFile);
            }
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Unmanaged <see cref="Wia.ImageFile"/>.
        /// </summary>
        readonly Wia.ImageFile _wiaImageFile;

        #endregion

        #region Public Properties

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }
        static readonly object _syncRoot = new object();

        /// <summary>
        /// Active frame.
        /// </summary>
        public int ActiveFrame
        {
            get { return _wiaImageFile.ActiveFrame; }
            set { _wiaImageFile.ActiveFrame = value; }
        }

        /// <summary>
        /// Color palette.
        /// </summary>
        public WiaVector ArgbData
        {
            get
            {
                // Create wrapper first time
                if (_argbData == null)
                {
                    lock (_syncRoot)
                    {
                        if (_argbData == null)
                            _argbData = new WiaVector(_wiaImageFile.ARGBData);
                    }
                }

                // Return wrapped value
                return _argbData;
            }
        }
        WiaVector _argbData;

        /// <summary>
        /// File data.
        /// </summary>
        public WiaVector FileData
        {
            get
            {
                // Create wrapper first time
                if (_fileData == null)
                {
                    lock (_syncRoot)
                    {
                        if (_fileData == null)
                            _fileData = new WiaVector(_wiaImageFile.FileData);
                    }
                }

                // Return wrapped value
                return _fileData;
            }
        }
        WiaVector _fileData;

        /// <summary>
        /// File extension.
        /// </summary>
        public string FileExtension { get { return _wiaImageFile.FileExtension; } }

        /// <summary>
        /// Image format ID.
        /// </summary>
        public string FormatId { get { return _wiaImageFile.FormatID; } }

        /// <summary>
        /// Frame count.
        /// </summary>
        public int FrameCount { get { return _wiaImageFile.FrameCount; } }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width { get { return _wiaImageFile.Width; } }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height { get { return _wiaImageFile.Height; } }

        /// <summary>
        /// Horizontal resolution.
        /// </summary>
        public double HorizontalResolution { get { return _wiaImageFile.HorizontalResolution; } }

        /// <summary>
        /// Vertical resolution.
        /// </summary>
        public double VerticalResolution { get { return _wiaImageFile.VerticalResolution; } }

        /// <summary>
        /// Indicates pixel alpha component (transparency) is used.
        /// </summary>
        public bool IsAlphaPixelFormat { get { return _wiaImageFile.IsAlphaPixelFormat; } }

        /// <summary>
        /// Indicates the picture is animated (contains multiple frames).
        /// </summary>
        public bool IsAnimated { get { return _wiaImageFile.IsAnimated; } }

        /// <summary>
        /// Indicates the pixel extended format.
        /// </summary>
        public bool IsExtendedPixelFormat { get { return _wiaImageFile.IsExtendedPixelFormat; } }

        /// <summary>
        /// Indicates the pixel indexed (color palette) format.
        /// </summary>
        public bool IsIndexedPixelFormat { get { return _wiaImageFile.IsIndexedPixelFormat; } }

        /// <summary>
        /// Pixel color depth.
        /// </summary>
        public int PixelDepth { get { return _wiaImageFile.PixelDepth; } }

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
                    lock (_syncRoot)
                    {
                        if (_properties == null)
                            _properties = new WiaPropertyCollection(_wiaImageFile.Properties);
                    }
                }

                // Return wrapped value
                return _properties;
            }
        }
        WiaPropertyCollection _properties;

        #endregion

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

        #endregion
    }
}
