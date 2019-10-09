using System.Diagnostics.CodeAnalysis;

namespace CodeForDotNet.UI.Models
{
	/// <summary>
	/// Base class for all page UI models
	/// </summary>
	public abstract class PageUIModel<TApplicationUIModel> : UIModel
		where TApplicationUIModel : ApplicationUIModel
	{
		#region Protected Constructors

		/// <summary>
		/// Creates an instance.
		/// </summary>
		[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "")]
		protected PageUIModel(TApplicationUIModel application)
			: base(application.UITaskFactory)
		{
			// Initialize members
			Application = application;
		}

		#endregion Protected Constructors

		#region Public Properties

		/// <summary>
		/// Application model.
		/// </summary>
		public TApplicationUIModel Application { get; private set; }

		#endregion Public Properties
	}
}
