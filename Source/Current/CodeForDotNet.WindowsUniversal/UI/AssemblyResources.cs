using Windows.ApplicationModel.Resources;

namespace CodeForDotNet.WindowsUniversal.UI
{
    /// <summary>
    /// Provides access to resources defined in this assembly.
    /// </summary>
    internal static class AssemblyResources
    {
        #region Properties

        /// <summary>
        /// Resource loader.
        /// </summary>
        public static readonly ResourceLoader Loader = ResourceLoader.GetForCurrentView();

        /// <summary>
        /// "DynamicTextBoxStyleMissingCursor" string.
        /// </summary>
        public static string DynamicTextBoxStyleMissingCursor
        {
            get { return Loader.GetString("DynamicTextBoxStyleMissingCursor"); }
        }

        #endregion
    }
}
