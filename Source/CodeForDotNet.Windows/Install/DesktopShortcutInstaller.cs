using CodeForDotNet.Windows.Native;
using CodeForDotNet.Windows.Properties;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeForDotNet.Install
{
    /// <summary>
    /// Generates native images on assemblies and their assemblyList during setup to decrease start-up times and increase performance.
    /// </summary>
    [RunInstaller(false)]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DesktopShortcutInstaller), "DesktopShortcutInstallerToolboxIcon.bmp")]
    public partial class DesktopShortcutInstaller : Installer
    {
        /// <summary>
        /// Shell shortcut file extension including dot.
        /// </summary>
        const string ShortcutFileExtension = ".lnk";

        /// <summary>
        /// Creates the component.
        /// </summary>
        public DesktopShortcutInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Target path for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("Installation")]
        [Description("Target path for the desktop shortcut.")]
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Condition required to activate the feature.
        /// </summary>
        [Browsable(true)]
        [Category("Installation")]
        [Description("Condition required to activate the feature.")]
        public string ConditionArgument { get; set; }

        /// <summary>
        /// Name for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description("Name for the desktop shortcut.")]
        public string ShortcutName { get; set; }

        /// <summary>
        /// Description for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description("Description for the desktop shortcut.")]
        public string ShortcutDescription { get; set; }

        /// <summary>
        /// Path for the desktop shortcut. Use %AssemblyDir% for the setup target folder. %AllUsersDesktop%, %CurrentUserDesktop%, %AllUsersPrograms%, %CurrentUserPrograms%, %MyDocuments%, or any other environment variable.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description(
            "Path for the desktop shortcut. Use %AssemblyDir% for the setup target folder. %AllUsersDesktop%, %CurrentUserDesktop%, %AllUsersPrograms%, %CurrentUserPrograms%, %MyDocuments%, or any other environment variable."
            )]
        public string ShortcutPath { get; set; }

        /// <summary>
        /// Arguments for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description("Arguments for the desktop shortcut.")]
        public string ShortcutArguments { get; set; }

        /// <summary>
        /// Working directory for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description("Working directory for the desktop shortcut.")]
        public string ShortcutWorkingDirectory { get; set; }

        /// <summary>
        /// Window show style for the desktop shortcut.
        /// </summary>
        [Browsable(true)]
        [Category("SafeNativeMethods.Shortcut")]
        [Description("Window show style for the desktop shortcut.")]
        [DefaultValue(ShowWindowOption.Default)]
        public ShowWindowOption ShortcutShowCommand { get; set; }

        /// <summary>
        /// Checks all installers in the stack for the specified parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool IsParameterTrue(string parameter)
        {
            Installer installer = this;
            while (installer != null)
            {
                if (installer.Context.Parameters.ContainsKey(parameter))
                {
                    string parameterValue = installer.Context.Parameters[parameter];
                    return (!string.IsNullOrEmpty(parameterValue) &&
                        (string.Compare(parameterValue.Trim(), "false", StringComparison.OrdinalIgnoreCase) != 0) &&
                        (parameterValue.Trim() != "0"));
                }
                installer = installer.Parent;
            }
            return false;
        }

        /// <summary>
        /// Installs the component.
        /// </summary>
        public override void Install(IDictionary stateSaver)
        {
            // Call base class implementation
            base.Install(stateSaver);

            // Do nothing when conditional argument not satisfied
            if (string.IsNullOrWhiteSpace(ConditionArgument))
                throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture,
                    Resources.ErrorMissingConditionProperty, "DesktopShortcutInstaller"));
            if (!IsParameterTrue(ConditionArgument))
                return;

            // Display status
            Context.LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.StatusInstall,
                "DesktopShortcutInstaller", ConditionArgument));

            // Create desktop shortcut
            string shortcutTargetPath = ExpandVariables(TargetDirectory) + Path.DirectorySeparatorChar + ShortcutName + ShortcutFileExtension;
            Context.LogMessage("\t" + shortcutTargetPath);
            try
            {
                var shortcut = (SafeNativeMethods.IShellLinkA)new SafeNativeMethods.Shortcut();
                shortcut.SetDescription(ExpandVariables(ShortcutDescription));
                shortcut.SetArguments(ExpandVariables(ShortcutArguments));
                shortcut.SetWorkingDirectory(ExpandVariables(ShortcutWorkingDirectory));
                shortcut.SetPath(ExpandVariables(ShortcutPath));
                shortcut.SetShowCmd((uint)ShortcutShowCommand);
                if (File.Exists(shortcutTargetPath))
                    File.Delete(shortcutTargetPath);
                ((SafeNativeMethods.IPersistFile)shortcut).Save(shortcutTargetPath, false);
            }
            catch (ExternalException error)
            {
                // Log error
                Context.LogMessage(error.ToString());

                // Throw any errors as installation errors
                throw new InstallException(error.Message, error);
            }

            // Save state
            if (stateSaver.Contains(ConditionArgument))
                stateSaver.Remove(ConditionArgument);
            stateSaver.Add(ConditionArgument, true);
        }

        /// <summary>
        /// Uninstalls the component.
        /// </summary>
        public override void Uninstall(IDictionary savedState)
        {
            // Call base class implementation
            base.Uninstall(savedState);

            // Do nothing when conditional argument not satisfied
            if ((savedState == null) || !savedState.Contains(ConditionArgument))
                return;

            // Display status
            Context.LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.StatusUninstall,
                "DesktopShortcutInstaller", ConditionArgument));

            // Remove desktop shortcut
            try
            {
                // Delete icon (if exists)
                string shortcutTargetPath = ExpandVariables(TargetDirectory) + Path.DirectorySeparatorChar + ShortcutName + ".lnk";
                if (File.Exists(shortcutTargetPath))
                {
                    Context.LogMessage(string.Format(CultureInfo.CurrentCulture,
                        Resources.StatusDeletingFile, shortcutTargetPath));
                    File.Delete(shortcutTargetPath);
                }
            }
            catch (IOException error)
            {
                // Log error
                Context.LogMessage(error.ToString());

                // Ignore errors during uninstall, else app may not be possible to remove.
            }

            // Clean up state
            savedState.Remove(ConditionArgument);
        }

        /// <summary>
        /// Expands all supported special variables and all user and system environment variables in the input text.
        /// </summary>
        /// <param name="input">Input text with any variables.</param>
        /// <returns>Output text with all supported variables expanded.</returns>
        private string ExpandVariables(string input)
        {
            string output = input;
            if (input == null)
                return string.Empty;

            // Translate %AssemblyDir% as installation target folder.
            string installPath = Path.GetDirectoryName(Context.Parameters["assemblypath"].Trim('"')).TrimEnd(Path.DirectorySeparatorChar);
            output = output.Replace("%AssemblyDir%", installPath);

            // Translate special folders
            bool allUsers = (GetSpecialFolderPath(SafeNativeMethods.Shell32.CSIDL_COMMON_PROGRAMS).Length > 0);
            output = output.Replace("%CurrentUserPrograms%", GetSpecialFolderPath(SafeNativeMethods.Shell32.CSIDL_PROGRAMS));
            output = output.Replace("%AllUsersPrograms%",
                GetSpecialFolderPath(allUsers ? SafeNativeMethods.Shell32.CSIDL_COMMON_PROGRAMS : SafeNativeMethods.Shell32.CSIDL_PROGRAMS));
            output = output.Replace("%CurrentUserDesktop%", GetSpecialFolderPath(SafeNativeMethods.Shell32.CSIDL_DESKTOPDIRECTORY));
            output = output.Replace("%AllUsersDesktop%",
                GetSpecialFolderPath(allUsers ? SafeNativeMethods.Shell32.CSIDL_COMMON_DESKTOPDIRECTORY : SafeNativeMethods.Shell32.CSIDL_DESKTOPDIRECTORY));
            output = output.Replace("%MyDocuments%", GetSpecialFolderPath(SafeNativeMethods.Shell32.CSIDL_PERSONAL));

            // Translate any other environment variables
            output = Environment.ExpandEnvironmentVariables(output);

            // Return result
            return output;
        }

        /// <summary>
        /// Gets the path to a system special folder. If the folder does not exist, it is NOT created.
        /// </summary>
        /// <param name="nFolder">CSIDL of the special folder.</param>
        /// <returns>Path to the special folder, if it exists.</returns>
        private static string GetSpecialFolderPath(int nFolder)
        {
            return GetSpecialFolderPath(nFolder, false);
        }

        /// <summary>
        /// Gets the path to a system special folder. If the folder does not exist, it can be created (optional).
        /// </summary>
        /// <param name="nFolder">CSIDL of the special folder.</param>
        /// <param name="create">Set true to create the special folder if it does not exist.</param>
        /// <returns>Path to the special folder, if it exists.</returns>
        private static string GetSpecialFolderPath(int nFolder, bool create)
        {
            StringBuilder pathBuffer = new StringBuilder(500);
            SafeNativeMethods.Shell32.SHGetSpecialFolderPath(IntPtr.Zero, pathBuffer, nFolder, create);
            return pathBuffer.ToString();
        }
    }

    /// <summary>
    /// Options for the ShortcutShowCommand.
    /// </summary>
    public enum ShowWindowOption
    {
        /// <summary>
        /// None specified.
        /// </summary>
        None,

        /// <summary>
        /// Default.
        /// </summary>
        Default = 1,

        /// <summary>
        /// Minimized.
        /// </summary>
        Minimized = 2,

        /// <summary>
        /// Maximized.
        /// </summary>
        Maximized = 3
    }
}
