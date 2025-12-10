using System;

namespace CodeForDotNet.ComponentModel;

/// <summary>
/// Carries arguments for the <see cref="ViewObject.ParentChanged"/> event.
/// </summary>
public class ViewObjectParentChangedEventArgs : EventArgs
{
    #region Public Constructors

    /// <summary>
    /// Creates an empty instance.
    /// </summary>
    public ViewObjectParentChangedEventArgs()
    {
        OldParent = null!;
        NewParent = null!;
    }

    /// <summary>
    /// Creates an instance with the specified values.
    /// </summary>
    /// <param name="oldParent">Old parent.</param>
    /// <param name="newParent">New parent.</param>
    public ViewObjectParentChangedEventArgs(IViewObject oldParent, IViewObject newParent)
    {
        OldParent = oldParent;
        NewParent = newParent;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// New parent.
    /// </summary>
    public IViewObject NewParent { get; set; }

    /// <summary>
    /// Old parent.
    /// </summary>
    public IViewObject OldParent { get; set; }

    #endregion Public Properties
}
