using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using CodeForDotNet.Windows.Native;
using CodeForDotNet.Windows.Properties;

namespace CodeForDotNet.Install
{
   /// <summary>
   /// Generates native images on assemblies and their assemblyList during setup to decrease start-up
   /// times and increase performance.
   /// </summary>
   [RunInstaller(false)]
   [ToolboxItem(true)]
   [ToolboxBitmap(typeof(NativeCodeGeneratorInstaller), "NativeCodeGeneratorInstallerToolboxIcon.bmp")]
   [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Settable properties required by toolbox extension specification.")]
   public class NativeCodeGeneratorInstaller : Installer
   {
      #region Private Fields

      /// <summary>
      /// Required designer variable.
      /// </summary>
      private Container _components;

      #endregion Private Fields

      #region Public Constructors

      /// <summary>
      /// Creates the component.
      /// </summary>
      public NativeCodeGeneratorInstaller()
      {
         // This call is required by the Designer.
         InitializeComponent();

         // Initialize members
         AssemblyList = new Collection<string>();
      }

      #endregion Public Constructors

      #region Public Properties

      /// <summary>
      /// List of assemblies referenced by the application, so must should also have native images
      /// cached to decrease start-up times and increase performance. Make a list of filenames
      /// without path (e.g. MyAssembly.dll).
      /// </summary>
      [Category("Installation"), Description("List of assemblies referenced by the application, so must should also have native images cached to decrease start-up times and increase performance. Make a list of filenames without path (e.g. MyAssembly.dll).")]
      public Collection<string> AssemblyList { get; set; }

      /// <summary>
      /// Condition required to activate the feature.
      /// </summary>
      [Browsable(true), Category("Installation"), Description("Condition required to activate the feature.")]
      public string ConditionArgument { get; set; }

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Installs the component.
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1031", Justification = "External program error is logged before being re-thrown.")]
      public override void Install(IDictionary stateSaver)
      {
         // Call base class implementation
         base.Install(stateSaver);

         // Do nothing when conditional argument not satisfied
         if (string.IsNullOrEmpty(ConditionArgument))
            throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture,
                Resources.ErrorMissingConditionProperty, "NativeCodeGeneratorInstaller"));
         if (!IsParameterTrue(ConditionArgument))
            return;

         // Display status
         Context.LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.StatusInstall,
             "NativeCodeGeneratorInstaller", ConditionArgument));

         // Run NGEN to install assemblies into native image cache
         var targetDir = Path.GetDirectoryName(Context.Parameters["assemblypath"].Trim('"')).TrimEnd(Path.DirectorySeparatorChar);
         var installedAssemblyList = new List<string>();
         foreach (var assemblyFilename in AssemblyList)
         {
            try
            {
               // Run NGEN
               var assemblyPath = targetDir + Path.DirectorySeparatorChar + assemblyFilename;
               Context.LogMessage("\t" + assemblyPath);
               if (NativeCodeGenerator.Install(assemblyPath, out var consoleOutput, out var assemblyFullName))
                  installedAssemblyList.Add(assemblyFullName);

               // Log output.
               Context.LogMessage(consoleOutput);
            }
            catch (Exception error)
            {
               // Log error then re-throw.
               Context.LogMessage(error.GetFullMessage());
               throw;
            }
         }

         // Save state
         if (stateSaver.Contains(ConditionArgument))
            stateSaver.Remove(ConditionArgument);
         stateSaver.Add(ConditionArgument, installedAssemblyList.ToArray());
      }

      /// <summary>
      /// Uninstalls the component.
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1031", Justification = "External program error is logged before being re-thrown.")]
      public override void Uninstall(IDictionary savedState)
      {
         // Call base class implementation
         base.Uninstall(savedState);

         // Do nothing when conditional argument not satisfied
         if (savedState == null || !savedState.Contains(ConditionArgument))
            return;

         // Display status
         Context.LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.StatusUninstall,
             "NativeCodeGeneratorInstaller", ConditionArgument));

         // Get saved assembly list
         var installedAssemblies = (string[])savedState[ConditionArgument];

         // Run NGEN to delete assemblies from the native image cache
         foreach (var assemblyFullName in installedAssemblies)
         {
            try
            {
               // Run NGEN
               Context.LogMessage("\t" + assemblyFullName);
               NativeCodeGenerator.Remove(assemblyFullName, out var consoleOutput);

               // Log output.
               Context.LogMessage(consoleOutput);
            }
            catch (Exception error)
            {
               // Log error then re-throw.
               Context.LogMessage(error.GetFullMessage());
               throw;
            }
         }

         // Clean up state (if conditional)
         savedState.Remove(ConditionArgument);
      }

      #endregion Public Methods

      #region Protected Methods

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
      {
         try
         {
            // Dispose owned objects (when actually disposing, not finalizing)
            if (disposing)
            {
               if (_components != null)
                  _components.Dispose();
            }

            // Release references to aid garbage collection
            _components = null;
         }
         finally
         {
            // Dispose base class
            base.Dispose(disposing);
         }
      }

      #endregion Protected Methods

      #region Private Methods

      /// <summary>
      /// Required method for Designer support - do not modify the contents of this method with the
      /// code editor.
      /// </summary>
      private void InitializeComponent()
      {
         _components = new System.ComponentModel.Container();
      }

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
               var parameterValue = installer.Context.Parameters[parameter];
               return (!string.IsNullOrEmpty(parameterValue) &&
                   (string.Compare(parameterValue.Trim(), "false", StringComparison.OrdinalIgnoreCase) != 0) &&
                   (parameterValue.Trim() != "0"));
            }
            installer = installer.Parent;
         }
         return false;
      }

      #endregion Private Methods
   }
}
