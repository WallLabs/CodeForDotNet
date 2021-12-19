using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CodeForDotNet.Windows.Native
{
    /// <summary>
    /// Provides helper functions for the Windows clipboard. For metafile functions, see source article: http://www.dotnet247.com/247reference/msgs/23/118514.aspx
    /// </summary>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class ClipboardHelper
	{
        #region Public Methods

        /// <summary>
        /// Retrieves a .NET <see cref="Metafile"/> from the clipboard correctly.
        /// </summary>
        /// <returns>.NET <see cref="Metafile"/> or null when no EMF is on the clipboard.</returns>
        public static Metafile? GetMetafileFromClipboard(IntPtr windowHandle)
		{
			Metafile? metafile = null;
			if (SafeNativeMethods.OpenClipboard(windowHandle))
			{
				var emfHandle = SafeNativeMethods.GetClipboardData(14 /* CF_ENHMETAFILE */);
				if (emfHandle != IntPtr.Zero)
				{
					var emfCloneHandle = SafeNativeMethods.CopyEnhMetaFile(emfHandle, IntPtr.Zero);
					metafile = new Metafile(emfCloneHandle, false);
				}
				SafeNativeMethods.CloseClipboard();
			}
			return metafile;
		}

		/// <summary>
		/// Places a .NET <see cref="Metafile"/> onto the clipboard correctly, otherwise it is not visible by other applications due to the new format used in .NET.
		/// </summary>
		/// <param name="windowHandle">Form.Handle used as owner of Clipboard data.</param>
		/// <param name="metafile">.NET <see cref="Metafile"/> to copy to clipboard.</param>
		/// <returns>TRUE when successful.</returns>
		public static bool PutMetafileOnClipboard(IntPtr windowHandle, Metafile metafile)
		{
			// Validate arguments
			if (metafile == null)
				throw new ArgumentNullException(nameof(metafile));

			var success = false;
			using (var gfx = Graphics.FromHwnd(IntPtr.Zero))
			{
				var hdc = gfx.GetHdc();
				using var metafileClone = new Metafile(new MemoryStream(), hdc);
				using (var gfx2 = Graphics.FromImage(metafileClone))
				{
					gfx2.DrawImage(metafile, 0, 0, metafile.Width, metafile.Height);
					gfx.ReleaseHdc(hdc);
				}
				var emfHandle = metafileClone.GetHenhmetafile();
				if (!emfHandle.Equals(IntPtr.Zero))
				{
					var emfCloneHandle = SafeNativeMethods.CopyEnhMetaFile(emfHandle, IntPtr.Zero);
					if (!emfCloneHandle.Equals(IntPtr.Zero))
					{
						try
						{
							if (SafeNativeMethods.OpenClipboard(windowHandle))
							{
								if (SafeNativeMethods.EmptyClipboard())
								{
									var hResult = SafeNativeMethods.SetClipboardData(14 /* CF_ENHMETAFILE */, emfCloneHandle);
									success = hResult.Equals(emfCloneHandle);
									SafeNativeMethods.CloseClipboard();
								}
							}
						}
						finally
						{
							SafeNativeMethods.DeleteEnhMetaFile(emfHandle);
						}
					}
				}
			}
			return success;
		}

		#endregion Public Methods
	}
}
