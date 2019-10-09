using System;
using System.Collections.ObjectModel;

namespace CodeForDotNet.ComponentModel
{
	/// <summary>
	/// View object with intelligent property, data and event caching.
	/// </summary>
	public interface IViewObject : IPropertyStore
	{
		#region Public Events

		/// <summary>
		/// Fired when the <see cref="Parent"/> is changed.
		/// </summary>
		event EventHandler<ViewObjectParentChangedEventArgs>? ParentChanged;

		#endregion Public Events

		#region Public Properties

		/// <summary>
		/// Gets a collection of child objects.
		/// </summary>
		ObservableCollection<IViewObject> Children { get; }

		/// <summary>
		/// Gets or sets the parent object.
		/// </summary>
		IViewObject Parent { get; set; }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Invalidates the layout of this object, optionally cascading to child objects.
		/// </summary>
		/// <remarks>Property cache is invalidated immediately, including children when specifeid, also when events are suspended.</remarks>
		void InvalidateLayout(bool includeChildren);

		/// <summary>
		/// Invalidates the view, causing it to be re-rendered.
		/// </summary>
		/// <remarks>If events are suspended the request is cached, then rendering will be delayed until events are resumed.</remarks>
		/// <returns>True if the operation was executed immediately, or false when cached.</returns>
		bool InvalidateView(bool includeChildren);

		#endregion Public Methods
	}
}
