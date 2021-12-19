using System.ComponentModel;
using System.Configuration;

namespace CodeForDotNet.Windows.Configuration
{
    /// <summary>
    /// Interface representation of the system <see cref="ApplicationSettingsBase"/>.
    /// </summary>
    /// <remarks>
    /// Acts as a base class for deriving concrete wrapper classes to implement the
    /// application settings feature in Window Forms applications.
    /// </remarks>
    public interface IApplicationSettingsBase : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the application settings context associated with the settings group.
        /// </summary>
        /// <returns>
        /// A System.Configuration.SettingsContext associated with the settings group.
        /// </returns>
        [Browsable(false)]
        SettingsContext Context { get; }

        /// <summary>
        /// Gets the collection of settings properties in the wrapper.
        /// </summary>
        /// <returns>
        /// A System.Configuration.SettingsPropertyCollection containing all the System.Configuration.SettingsProperty
        /// objects used in the current wrapper.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The associated settings provider could not be found or its instantiation failed.
        /// </exception>
        [Browsable(false)]
        SettingsPropertyCollection Properties { get; }

        /// <summary>
        /// Gets a collection of property values.
        /// </summary>
        /// <returns>
        /// A System.Configuration.SettingsPropertyValueCollection of property values.
        /// </returns>
        [Browsable(false)]
        SettingsPropertyValueCollection PropertyValues { get; }

        /// <summary>
        /// Gets the collection of application settings providers used by the wrapper.
        /// </summary>
        /// <returns>
        /// A System.Configuration.SettingsProviderCollection containing all the System.Configuration.SettingsProvider
        /// objects used by the settings properties of the current settings wrapper.
        /// </returns>
        [Browsable(false)]
        SettingsProviderCollection Providers { get; }

        /// <summary>
        /// Gets or sets the settings key for the application settings group.
        /// </summary>
        /// <returns>
        /// A System.String containing the settings key for the current settings group.
        /// </returns>
        [Browsable(false)]
        string SettingsKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the specified application settings property.
        /// </summary>
        /// <param name="propertyName">A System.String containing the name of the property to access.</param>
        /// <returns>If found, the value of the named settings property; otherwise, null.</returns>
        /// <exception cref="SettingsPropertyNotFoundException">
        /// There are no properties associated with the current wrapper or the specified
        /// property could not be found.
        /// </exception>
        /// <exception cref="SettingsPropertyIsReadOnlyException">
        /// An attempt was made to set a read-only property.
        /// </exception>
        /// <exception cref="SettingsPropertyWrongTypeException">
        /// The value supplied is of a type incompatible with the settings property,
        /// during a set operation.
        /// </exception>
        object this[string propertyName] { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before the value of an application settings property is changed.
        /// </summary>
        event SettingChangingEventHandler SettingChanging;

        /// <summary>
        /// Occurs after the application settings are retrieved from storage.
        /// </summary>
        event SettingsLoadedEventHandler SettingsLoaded;

        /// <summary>
        /// Occurs before values are saved to the data store.
        /// </summary>
        event SettingsSavingEventHandler SettingsSaving;

        #endregion

        #region Methods

        /// <summary>
        /// Returns the value of the named settings property for the previous version
        /// of the same application.
        /// </summary>
        /// <param name="propertyName">
        /// A System.String containing the name of the settings property whose value
        /// is to be returned.
        /// </param>
        /// <returns>
        /// An System.Object containing the value of the specified System.Configuration.SettingsProperty
        /// if found; otherwise, null.
        /// </returns>
        /// <exception cref="SettingsPropertyNotFoundException">
        /// The property does not exist. The property count is zero or the property cannot
        /// be found in the data store.
        /// </exception>
        object GetPreviousVersion(string propertyName);

        /// <summary>
        /// Refreshes the application settings property values from persistent storage.
        /// </summary>
        void Reload();

        /// <summary>
        /// Restores the persisted application settings values to their corresponding
        /// default properties.
        /// </summary>
        void Reset();

        /// <summary>
        /// Stores the current values of the application settings properties.
        /// </summary>
        void Save();

        /// <summary>
        /// Updates application settings to reflect a more recent installation of the application.
        /// </summary>
        void Upgrade();

        #endregion
    }
}
