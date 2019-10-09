using Windows.ApplicationModel.Resources;

namespace CodeForDotNet.WindowsUniversal
{
	/// <summary>
	/// Provides access to resources defined in this assembly.
	/// </summary>
	internal static class AssemblyResources
	{
		#region Public Fields

		/// <summary>
		/// Resource loader.
		/// </summary>
		public static readonly ResourceLoader Loader = ResourceLoader.GetForCurrentView();

		#endregion Public Fields

		#region Public Properties

		/// <summary>
		/// <see cref="ApplicationBaseOnLaunchedErrorCreatePage"/> string.
		/// </summary>
		public static string ApplicationBaseOnLaunchedErrorCreatePage => Loader.GetString(nameof(ApplicationBaseOnLaunchedErrorCreatePage));

		/// <summary>
		/// <see cref="DynamicTextBoxStyleMissingCursor"/> string.
		/// </summary>
		public static string DynamicTextBoxStyleMissingCursor => Loader.GetString(nameof(DynamicTextBoxStyleMissingCursor));

		/// <summary>
		/// <see cref="SuspensionManagerRegisterFrameErrorSessionExists"/> string.
		/// </summary>
		public static string SuspensionManagerRegisterFrameErrorSessionExists => Loader.GetString(nameof(SuspensionManagerRegisterFrameErrorSessionExists));

		/// <summary>
		/// <see cref="SuspensionManagerRegisterFrameErrorState"/> string.
		/// </summary>
		public static string SuspensionManagerRegisterFrameErrorState => Loader.GetString(nameof(SuspensionManagerRegisterFrameErrorState));

		/// <summary>
		/// <see cref="SuspensionManagerRestoreAsyncErrorState"/> string.
		/// </summary>
		public static string SuspensionManagerRestoreAsyncErrorState => Loader.GetString(nameof(SuspensionManagerRestoreAsyncErrorState));

		/// <summary>
		/// <see cref="SuspensionManagerSaveAsyncErrorState"/> string.
		/// </summary>
		public static string SuspensionManagerSaveAsyncErrorState => Loader.GetString(nameof(SuspensionManagerSaveAsyncErrorState));

		#endregion Public Properties
	}
}
