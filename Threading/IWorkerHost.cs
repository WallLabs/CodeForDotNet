using Microsoft.Extensions.Logging;

namespace CodeChief.Threading;

/// <summary>
/// Worker host providing logging and cancellation.
/// </summary>
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
