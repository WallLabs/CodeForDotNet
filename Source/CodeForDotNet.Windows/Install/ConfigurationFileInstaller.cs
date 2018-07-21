using CodeForDotNet.Windows.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Install;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace CodeForDotNet.Install
{
    /// <summary>
    /// Processes configuration files upon installation.
    /// </summary>
    [RunInstaller(false)]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ConfigurationFileInstaller), "ConfigurationFileInstallerToolboxIcon.bmp")]
    public partial class ConfigurationFileInstaller : Installer
    {
        /// <summary>
        /// Creates the object.
        /// </summary>
        public ConfigurationFileInstaller()
        {
            ExpandVariables = true;
            DeleteAtUninstall = true;
            Variables = new Collection<string>();
            FileNames = new Collection<string>();
            // This call is required by the Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Condition required to activate this feature.
        /// </summary>
        [Browsable(true), Category("Installation"), Description("Target path for the desktop shortcut.")]
        public string ConditionArgument { get; set; }

        /// <summary>
        /// FileNames to process, relative to the installing assembly path.
        /// </summary>
        [Category("Installation"), Description("List of filenames to process (relative to the installation target directory).")]
        public Collection<string> FileNames { get; set; }

        /// <summary>
        /// Variables to replace.
        /// </summary>
        [Category("Installation"), Description(@"List of replacement key=values (e.g. C:\Program Files=%programfiles%). Additionally, any installer parameters can be referenced like environment variables, e.g. %assemblypath%. Also %assemblydir% and %assemblyname% are added for convenience.")]
        public Collection<string> Variables { get; set; }

        /// <summary>
        /// Indicates whether to delete the files at uninstall.
        /// </summary>
        [Browsable(true), Category("Installation"), Description("Deletes the processed files at uninstall.")]
        public bool DeleteAtUninstall { get; set; }

        /// <summary>
        /// Expands all environment variables in the files. Occurs at the last step so any replacements made by Variables will also be expanded if they are set to %variables%.
        /// </summary>
        [Browsable(true), Category("Installation"), Description("Expands all environment variables in the files. Occurs at the last step so any replacements made by Variables will also be expanded if they are set to %variables%.")]
        public bool ExpandVariables { get; set; }

        /// <summary>
        /// Checks all installers in the stack for the specified parameter.
        /// </summary>
        private Collection<string> GetAllParameters()
        {
            Installer installer = this;
            var parameterList = new Collection<string>();
            while (installer != null)
            {
                foreach (string parameter in installer.Context.Parameters.Keys)
                    if (!parameterList.Contains(parameter))
                        parameterList.Add(parameter);
                installer = installer.Parent;
            }
            return parameterList;
        }

        /// <summary>
        /// Checks all installers in the stack for the specified parameter.
        /// </summary>
        private string GetParameterValue(string parameter)
        {
            Installer installer = this;
            while (installer != null)
            {
                if (installer.Context.Parameters.ContainsKey(parameter))
                    return installer.Context.Parameters[parameter];
                installer = installer.Parent;
            }
            return null;
        }

        /// <summary>
        /// Checks all installers in the stack for the specified parameter and checks it's boolean value.
        /// </summary>
        private bool IsParameterTrue(string parameter)
        {
            Installer installer = this;
            while (installer != null)
            {
                if (installer.Context.Parameters.ContainsKey(parameter))
                {
                    string parameterValue = installer.Context.Parameters[parameter];
                    return (!String.IsNullOrEmpty(parameterValue) &&
                        (String.Compare(parameterValue.Trim(), "false", StringComparison.OrdinalIgnoreCase) != 0) &&
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
            if (String.IsNullOrEmpty(ConditionArgument))
                throw new ArgumentNullException(String.Format(CultureInfo.CurrentCulture,
                    Resources.ErrorMissingConditionProperty, "ConfigurationFileInstaller"));
            if (!IsParameterTrue(ConditionArgument))
                return;

            // Display status
            Context.LogMessage(String.Format(CultureInfo.CurrentCulture, Resources.StatusInstall,
                "ConfigurationFileInstaller", ConditionArgument));

            // Process each file
            string assemblyPath = Path.GetFullPath(GetParameterValue("assemblypath"));
            string assemblyDir = Path.GetDirectoryName(assemblyPath).TrimEnd(Path.DirectorySeparatorChar);
            var installedFilenames = new List<string>();
            foreach (string filename in FileNames)
            {
                // Load file
                string filePath = assemblyDir + Path.DirectorySeparatorChar + filename;
                Context.LogMessage(String.Format(CultureInfo.CurrentCulture, Resources.StatusLoadingFile, filePath));
                string fileContents;
                using (StreamReader reader = File.OpenText(filePath))
                    fileContents = reader.ReadToEnd();

                // Replace variables in file
                foreach (string variable in Variables)
                {
                    // Get variable key and value from (key=value)
                    string[] variableParts = variable.Split('=');
                    if (variableParts.Length != 2)
                        continue;
                    string variableKey = variableParts[0];
                    string variableValue = variableParts[1];

                    // Get property values (if specified)
                    if (variableValue.StartsWith("[", StringComparison.OrdinalIgnoreCase) &&
                        variableValue.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                    {
                        string propertyName = variableValue.Substring(1, variableValue.Length - 2);
                        string propertyValue = GetParameterValue(propertyName);
                        if ((propertyValue == null) || (propertyValue.Length == 0))
                            continue;
                    }

                    // Replace text in file
                    fileContents = fileContents.Replace(variableKey, variableValue);
                }

                // Expand variables
                foreach (string installerParameter in GetAllParameters())
                    fileContents = fileContents.Replace("%" + installerParameter + "%", GetParameterValue(installerParameter));
                if (ExpandVariables)
                    fileContents = Environment.ExpandEnvironmentVariables(fileContents);

                // Expand extra hard-coded variables
                fileContents = fileContents.Replace("%assemblydir%", assemblyDir);
                fileContents = fileContents.Replace("%assemblyname%", Path.GetFileNameWithoutExtension(assemblyPath));

                // Write modified file back to disk
                File.Delete(filePath);
                using (StreamWriter writer = File.CreateText(filePath))
                    writer.Write(fileContents);

                // Add to installed list
                installedFilenames.Add(filePath);
            }

            // Save state
            if (stateSaver.Contains(ConditionArgument))
                stateSaver.Remove(ConditionArgument);
            stateSaver.Add(ConditionArgument, installedFilenames.ToArray());
        }

        /// <summary>
        /// Uninstalls the component.
        /// </summary>
        public override void Uninstall(IDictionary savedState)
        {
            // Call base class implementation
            base.Uninstall(savedState);

            // Do nothing when conditional argument not satisfied
            if (savedState == null || !savedState.Contains(ConditionArgument))
                return;

            // Display status
            Context.LogMessage(String.Format(CultureInfo.CurrentCulture, Resources.StatusUninstall,
                "ConfigurationFileInstaller", ConditionArgument));

            // Get saved assembly list
            string[] installedFilenames = (string[])savedState[ConditionArgument];

            // Delete files (if flagged)
            if (DeleteAtUninstall)
            {
                foreach (string filename in installedFilenames)
                {
                    if (File.Exists(filename))
                    {
                        Context.LogMessage(String.Format(CultureInfo.CurrentCulture,
                            Resources.StatusDeletingFile, filename));
                        File.Delete(filename);
                    }
                }
            }

            // Clean up state (if conditional)
            savedState.Remove(ConditionArgument);
        }
    }
}
