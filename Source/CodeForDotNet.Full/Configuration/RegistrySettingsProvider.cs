// This code is based of the MSDN RegistrySettingsProvider sample:
// http://msdn2.microsoft.com/library/ms181001(en-us,vs.80).aspx
//
// 2006.09.21 - Tony Wall
//  * Added support for optional sub-paths specified by the RegistrySettingsProviderSubkeyAttribute,
//    to support separation of settings made in different modules.
//      Full path is now HKLM/HKCU then SOFTWARE\Company\Product\[Subkey\]Setting = Value.
//
// 2006.09.19 - Tony Wall
//  * Changed ApplicationName and GetSubKeypath to be built dynamically from AssemblyProduct and AssemblyCompany.
//
// 2006.07.27 - Tony Wall
//  * Converted comments to XML documentation standard
//  * Refactored names to make easier to read/maintain
//  * Fixed bug where values would be set with null, they are now deleted if they exist when set to null, e.g. default/unset.
//
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

namespace CodeForDotNet.Configuration
{
    /// <summary>
    /// Settings provider which stores settings in the HKLM or HKCU registry.
    /// Builds the registry path based on the SOFTWARE base key then subkeys of AssemblyCompany, AssemblyProduct
    /// and optional RegistrySettingsProviderSubkeyAttribute.
    /// e.g. HKLM/HKCU then SOFTWARE\Company\Product\[Subkey\]Setting = Value.
    /// </summary>
    public class RegistrySettingsProvider : SettingsProvider
    {
        /// <summary>
        /// Name of the application. Returns the AssemblyProduct name.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var productAttribute = (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    assembly, typeof(AssemblyProductAttribute));
                return productAttribute.Product;
            }
            set { }
        }

        /// <summary>
        /// Initializes the provider with our application name.
        /// </summary>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        /// <summary>
        /// SetPropertyValue is invoked when ApplicationSettingsBase.Save is called
        /// for only the values marked with this provider attribute.
        /// </summary>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            // Validate arguments
            if (collection == null)
                throw new ArgumentNullException("collection");

            // Iterate through the settings to be stored
            foreach (SettingsPropertyValue property in collection)
            {
                RegistryKey key = GetRegKey(property.Property);
                if (property.SerializedValue != null)
                {
                    // Set value when non-default
                    key.SetValue(property.Name, property.SerializedValue);
                }
                else
                {
                    // Delete value when default or null (if exists)
                    key.DeleteValue(property.Name, false);
                }
            }
        }

        /// <summary>
        /// Returns the collection of settings property values for the specified application
        /// instance and settings property group.
        /// </summary>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            // Validate parameters
            if (collection == null)
                throw new ArgumentNullException("collection");

            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    SerializedValue = GetRegKey(setting).GetValue(setting.Name)
                };
                values.Add(value);
            }
            return values;
        }

        /// <summary>
        /// Helper method: fetches correct registry subkey.
        /// HKLM is used for settings marked as application-scoped.
        /// HKLU is used for settings marked as user-scoped. 
        /// </summary>
        static RegistryKey GetRegKey(SettingsProperty property)
        {
            RegistryKey key = IsUserScoped(property) ? Registry.CurrentUser : Registry.LocalMachine;
            key = key.CreateSubKey(GetSubKeyPath(property), RegistryKeyPermissionCheck.ReadSubTree);
            return key;
        }

        /// <summary>
        /// Helper method: walks the "attribute bag" for a given property
        /// to determine if it is user-scoped or not.
        /// Note that this provider does not enforce other rules, such as 
        ///    - unknown attributes
        ///    - improper attribute combinations (e.g. both user and app - this implementation
        ///      would say true for user-scoped regardless of existence of app-scoped)
        /// </summary>
        static bool IsUserScoped(SettingsProperty property)
        {
            foreach (DictionaryEntry settingsAttribute in property.Attributes)
            {
                var attribute = (Attribute)settingsAttribute.Value;
                if (attribute.GetType() == typeof(UserScopedSettingAttribute))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Builds the registry path based on the SOFTWARE base key then subkeys of AssemblyCompany, AssemblyProduct
        /// and optional RegistrySettingsProviderSubkeyAttribute. Does not include the HKLM or HKCU hive.
        /// e.g. SOFTWARE\Company\Product\[Subkey\]Setting = Value.
        /// </summary>
        static string GetSubKeyPath(SettingsProperty property)
        {
            // Get the AssemblyCompany and AssemblyProduct names from the caller
            Assembly assembly = Assembly.GetCallingAssembly();
            AssemblyProductAttribute productAttribute = (AssemblyProductAttribute)
                Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            AssemblyCompanyAttribute companyAttribute = (AssemblyCompanyAttribute)
                Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute));
            string path = "SOFTWARE\\" + companyAttribute.Company + "\\" + productAttribute.Product;

            // Add the RegistrySettingsProviderSubkeyAttribute if specified
            Type attributeType = typeof(RegistrySettingsProviderSubkeyAttribute);
            if (property.Attributes.ContainsKey(attributeType))
            {
                RegistrySettingsProviderSubkeyAttribute attribute = (RegistrySettingsProviderSubkeyAttribute)
                    property.Attributes[attributeType];
                path += "\\" + attribute.Subkey.TrimEnd('\\');
            }
            return path;
        }
    }

    /// <summary>
    /// Attribute which is applied to settings properties in the settings designer class
    /// to allow the sub-key(s) to be specified when using the RegistrySettingsProvider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RegistrySettingsProviderSubkeyAttribute : Attribute
    {
        /// <summary>
        /// Specified the sub-key(s) in which to store this setting.
        /// </summary>
        /// <param name="subkey"></param>
        public RegistrySettingsProviderSubkeyAttribute(string subkey)
        {
            Subkey = subkey;
        }

        /// <summary>
        /// Sub-Key path in which to store the setting. e.g. "Preferences\Display".
        /// </summary>
        public string Subkey { get; private set; }
    }
}
