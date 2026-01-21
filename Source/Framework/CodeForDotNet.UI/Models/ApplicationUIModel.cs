using System.Threading.Tasks;

namespace CodeForDotNet.UI.Models;

/// <summary>
/// Application UI model.
/// </summary>
public class ApplicationUIModel : UIModel
{
    #region Lifetime

    /// <summary>
    /// Creates an instance.
    /// </summary>
    public ApplicationUIModel(TaskFactory uiTaskFactory)
        : base(uiTaskFactory)
    {
    }

    #endregion Lifetime
}