using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CodeForDotNet.Windows.Native
{
    /// <summary>
    /// Native method calls which are safe to call without a stack walk.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native code naming standards.")]
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Native code place holder.")]
    internal abstract class SafeNativeMethods
    {
        #region User32.dll clipboard functions

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenClipboard(IntPtr windowHandleNewOwner);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        internal static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseClipboard();

        #endregion

        #region GDI32.dll graphics functions

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CopyEnhMetaFile(IntPtr hemetafileSrc, IntPtr hNULL);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteEnhMetaFile(IntPtr hemetafile);

        #endregion

        #region Shell32.dll functions

        /// <summary>
        /// Managed interface to the Windows shell IShellLink COM interface.
        /// </summary>
        [ComImport()]
        [Guid("000214EE-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellLinkA
        {
            /// <summary>
            /// Retrieves the path and filename of a shell link object.
            /// </summary>
            void GetPath(
                [Out(), MarshalAs(UnmanagedType.LPStr)]
            StringBuilder pszFile,
                int cchMaxPath,
                ref _WIN32_FIND_DATAW pfd,
                uint fFlags);

            /// <summary>
            /// Retrieves the list of shell link item identifiers.
            /// </summary>
            void GetIDList(out IntPtr ppidl);

            /// <summary>
            /// Sets the list of shell link item identifiers.
            /// </summary>
            void SetIDList(IntPtr pidl);

            /// <summary>
            /// Retrieves the shell link description string.
            /// </summary>
            void GetDescription(
                [Out(), MarshalAs(UnmanagedType.LPStr)]
            StringBuilder pszFile,
                int cchMaxName);

            /// <summary>
            /// Sets the shell link description string.
            /// </summary>
            void SetDescription(
                [MarshalAs(UnmanagedType.LPStr)] string pszName);

            /// <summary>
            /// Retrieves the name of the shell link working directory.
            /// </summary>
            void GetWorkingDirectory(
                [Out(), MarshalAs(UnmanagedType.LPStr)]
            StringBuilder pszDir,
                int cchMaxPath);

            /// <summary>
            /// Sets the name of the shell link working directory.
            /// </summary>
            void SetWorkingDirectory(
                [MarshalAs(UnmanagedType.LPStr)] string pszDir);

            /// <summary>
            /// Retrieves the shell link command-line arguments.
            /// </summary>
            void GetArguments(
                [Out(), MarshalAs(UnmanagedType.LPStr)]
            StringBuilder pszArgs,
                int cchMaxPath);

            /// <summary>
            /// Sets the shell link command-line arguments.
            /// </summary>
            void SetArguments(
                [MarshalAs(UnmanagedType.LPStr)] string pszArgs);

            /// <summary>
            /// Retrieves the shell link hot key.
            /// </summary>
            void GetHotkey(out short pwHotkey);

            /// <summary>
            /// Sets the shell link hot key.
            /// </summary>
            void SetHotkey(short pwHotkey);

            /// <summary>
            /// Retrieves the shell link show command.
            /// </summary>
            void GetShowCmd(out uint piShowCmd);

            /// <summary>
            /// Sets the shell link show command.
            /// </summary>
            void SetShowCmd(uint piShowCmd);

            /// <summary>
            /// Retrieves the location (path and index) of the shell link icon.
            /// </summary>
            void GetIconLocation(
                [Out(), MarshalAs(UnmanagedType.LPStr)]
            StringBuilder pszIconPath,
                int cchIconPath,
                out int piIcon);

            /// <summary>
            /// Sets the location (path and index) of the shell link icon.
            /// </summary>
            void SetIconLocation(
                [MarshalAs(UnmanagedType.LPStr)] string pszIconPath,
                int iIcon);

            /// <summary>
            /// Sets the shell link relative path.
            /// </summary>
            void SetRelativePath(
                [MarshalAs(UnmanagedType.LPStr)]
            string pszPathRel,
                uint dwReserved);

            /// <summary>
            /// Resolves a shell link. The system searches for the shell link object and updates the shell link path and its list of identifiers (if necessary).
            /// </summary>
            void Resolve(
                IntPtr hWnd,
                uint fFlags);

            /// <summary>
            /// Sets the shell link path and filename.
            /// </summary>
            void SetPath(
                [MarshalAs(UnmanagedType.LPStr)]
            string pszFile);
        }

        /// <summary>
        /// Managed class to the Windows shell ShellLink COM object class.
        /// Provided by: http://www.vbaccelerator.com/home/NET/Code/Libraries/Shell_Projects/Creating_and_Modifying_Shortcuts/article.asp
        /// </summary>
        [Guid("00021401-0000-0000-C000-000000000046")]
        [ClassInterface(ClassInterfaceType.None)]
        [ComImport()]
        internal class Shortcut { }

        [StructLayout(LayoutKind.Sequential,
             Pack = 4, Size = 0, CharSet = CharSet.Unicode)]
        internal struct _WIN32_FIND_DATAW
        {
            internal uint dwFileAttributes;
            internal _FILETIME ftCreationTime;
            internal _FILETIME ftLastAccessTime;
            internal _FILETIME ftLastWriteTime;
            internal uint nFileSizeHigh;
            internal uint nFileSizeLow;
            internal uint dwReserved0;
            internal uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternateFileName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
        internal struct _FILETIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;
        }

        /// <summary>
        /// Managed interface to the Windows shell IPersistFile COM interface.
        /// Provided by: http://www.vbaccelerator.com/home/NET/Code/Libraries/Shell_Projects/Creating_and_Modifying_Shortcuts/article.asp
        /// </summary>
        [ComImport()]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IPersistFile
        {
            /// <summary>
            /// Can't get this to go if I extend IPersist, so put it here.
            /// </summary>
            [PreserveSig]
            int GetClassID(out Guid pClassID);

            /// <summary>
            /// Checks for changes since last file write.
            /// </summary>
            [PreserveSig]
            int IsDirty();

            /// <summary>
            /// Opens the specified file and initializes the object from its contents.
            /// </summary>
            void Load(
                [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                uint dwMode);

            /// <summary>
            /// Saves the object into the specified file.
            /// </summary>
            void Save(
                [MarshalAs(UnmanagedType.LPWStr)]
            string pszFileName,
                [MarshalAs(UnmanagedType.Bool)]
            bool fRemember);

            /// <summary>
            /// Notifies the object that save is completed.
            /// </summary>
            void SaveCompleted(
                [MarshalAs(UnmanagedType.LPWStr)]
            string pszFileName);

            /// <summary>
            /// Gets the current name of the file associated with the object.
            /// </summary>
            void GetCurFile(
                [MarshalAs(UnmanagedType.LPWStr)]
            out string ppszFileName);
        }

        internal abstract class Shell32
        {
            internal const int CSIDL_DESKTOP = 0x0000;        // <desktop>
            internal const int CSIDL_INTERNET = 0x0001;        // Internet Explorer (icon on desktop)
            internal const int CSIDL_PROGRAMS = 0x0002;        // Start Menu\Programs
            internal const int CSIDL_CONTROLS = 0x0003;        // My Computer\Control Panel
            internal const int CSIDL_PRINTERS = 0x0004;        // My Computer\Printers
            internal const int CSIDL_PERSONAL = 0x0005;        // My Documents
            internal const int CSIDL_FAVORITES = 0x0006;        // <user name>\Favorites
            internal const int CSIDL_STARTUP = 0x0007;        // Start Menu\Programs\Startup
            internal const int CSIDL_RECENT = 0x0008;        // <user name>\Recent
            internal const int CSIDL_SENDTO = 0x0009;        // <user name>\SendTo
            internal const int CSIDL_BITBUCKET = 0x000a;        // <desktop>\Recycle Bin
            internal const int CSIDL_STARTMENU = 0x000b;        // <user name>\Start Menu
            internal const int CSIDL_MYDOCUMENTS = 0x000c;        // logical "My Documents" desktop icon
            internal const int CSIDL_MYMUSIC = 0x000d;        // "My Music" folder
            internal const int CSIDL_MYVIDEO = 0x000e;        // "My Videos" folder
            internal const int CSIDL_DESKTOPDIRECTORY = 0x0010;        // <user name>\Desktop
            internal const int CSIDL_DRIVES = 0x0011;        // My Computer
            internal const int CSIDL_NETWORK = 0x0012;        // Network Neighborhood (My Network Places)
            internal const int CSIDL_NETHOOD = 0x0013;        // <user name>\nethood
            internal const int CSIDL_FONTS = 0x0014;        // windows\fonts
            internal const int CSIDL_TEMPLATES = 0x0015;
            internal const int CSIDL_COMMON_STARTMENU = 0x0016;        // All Users\Start Menu
            internal const int CSIDL_COMMON_PROGRAMS = 0x0017;        // All Users\Start Menu\Programs
            internal const int CSIDL_COMMON_STARTUP = 0x0018;        // All Users\Startup
            internal const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019;        // All Users\Desktop
            internal const int CSIDL_APPDATA = 0x001a;        // <user name>\Application Data
            internal const int CSIDL_PRINTHOOD = 0x001b;        // <user name>\PrintHood

            internal const int CSIDL_LOCAL_APPDATA = 0x001c;        // <user name>\Local Settings\Applicaiton Data (non roaming)

            internal const int CSIDL_ALTSTARTUP = 0x001d;        // non localized startup
            internal const int CSIDL_COMMON_ALTSTARTUP = 0x001e;        // non localized common startup
            internal const int CSIDL_COMMON_FAVORITES = 0x001f;

            internal const int CSIDL_INTERNET_CACHE = 0x0020;
            internal const int CSIDL_COOKIES = 0x0021;
            internal const int CSIDL_HISTORY = 0x0022;
            internal const int CSIDL_COMMON_APPDATA = 0x0023;        // All Users\Application Data
            internal const int CSIDL_WINDOWS = 0x0024;        // GetWindowsDirectory()
            internal const int CSIDL_SYSTEM = 0x0025;        // GetSystemDirectory()
            internal const int CSIDL_PROGRAM_FILES = 0x0026;        // C:\Program Files
            internal const int CSIDL_MYPICTURES = 0x0027;        // C:\Program Files\My Pictures

            internal const int CSIDL_PROFILE = 0x0028;        // USERPROFILE
            internal const int CSIDL_SYSTEMX86 = 0x0029;        // x86 system directory on RISC
            internal const int CSIDL_PROGRAM_FILESX86 = 0x002a;        // x86 C:\Program Files on RISC

            internal const int CSIDL_PROGRAM_FILES_COMMON = 0x002b;        // C:\Program Files\Common

            internal const int CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c;        // x86 Program Files\Common on RISC
            internal const int CSIDL_COMMON_TEMPLATES = 0x002d;        // All Users\Templates

            internal const int CSIDL_COMMON_DOCUMENTS = 0x002e;        // All Users\Documents
            internal const int CSIDL_COMMON_ADMINTOOLS = 0x002f;        // All Users\Start Menu\Programs\Administrative Tools
            internal const int CSIDL_ADMINTOOLS = 0x0030;        // <user name>\Start Menu\Programs\Administrative Tools

            internal const int CSIDL_CONNECTIONS = 0x0031;        // Network and Dial-up Connections
            internal const int CSIDL_COMMON_MUSIC = 0x0035;        // All Users\My Music
            internal const int CSIDL_COMMON_PICTURES = 0x0036;        // All Users\My Pictures
            internal const int CSIDL_COMMON_VIDEO = 0x0037;        // All Users\My Video
            internal const int CSIDL_RESOURCES = 0x0038;        // Resource Direcotry

            internal const int CSIDL_RESOURCES_LOCALIZED = 0x0039;        // Localized Resource Direcotry

            internal const int CSIDL_COMMON_OEM_LINKS = 0x003a;        // Links to All Users OEM specific apps
            internal const int CSIDL_CDBURN_AREA = 0x003b;        // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
            // unused                               = 0x003c
            internal const int CSIDL_COMPUTERSNEARME = 0x003d;        // Computers Near Me (computered from Workgroup membership)

            internal const int CSIDL_FLAG_CREATE = 0x8000;        // combine with CSIDL_ value to force folder creation in SHGetFolderPath()

            internal const int CSIDL_FLAG_DONT_VERIFY = 0x4000;        // combine with CSIDL_ value to return an unverified folder path
            internal const int CSIDL_FLAG_NO_ALIAS = 0x1000;        // combine with CSIDL_ value to insure non-alias versions of the pidl
            internal const int CSIDL_FLAG_PER_USER_INIT = 0x0800;        // combine with CSIDL_ value to indicate per-user init (eg. upgrade)
            internal const int CSIDL_FLAG_MASK = 0xFF00;        // mask for all possible flag values

            /// <summary>
            ///	BOOL SHGetSpecialFolderPath(          HWND hwndOwner,
            ///		LPTSTR lpszPath,
            ///		int nFolder,
            ///		BOOL fCreate
            ///	);
            ///	</summary>
            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder,
                [MarshalAs(UnmanagedType.Bool)] bool fCreate);
        }

        #endregion

        #region .NET Fusion.dll functions

        [DllImport("mscoree.dll")]
        internal static extern int GetCORSystemDirectory([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder buffer, int bufferLength, ref int length);

        #endregion
    }
}
