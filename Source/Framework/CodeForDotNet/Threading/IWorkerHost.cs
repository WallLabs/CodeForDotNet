using Microsoft.Extensions.Logging;

namespace CodeForDotNet.Threading;

/// <summary>
/// Worker host providing logging and cancellation.
/// </summary>
/// <remarks>
/// These classes were created before <see cref="System.Threading.Tasks.Task"/> and should be refactored or deprecated in the future.
/// </remarks>
public interface IWorkerHost
{
    /// <summary>
    /// Logging interface.
    /// </summary>
    ILogger Log { get; }

    /// <summary>
    /// Gets a value indicating whether the worker should cancel its work.
    /// </summary>
    bool Canceled { get; }
}
