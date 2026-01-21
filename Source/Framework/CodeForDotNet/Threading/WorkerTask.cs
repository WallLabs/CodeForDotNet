using System.Threading;

namespace CodeForDotNet.Threading;

/// <summary>
/// Individual worker task in a <see cref="WorkerGroup"/>.
/// </summary>
/// <remarks>
/// These classes were created before <see cref="System.Threading.Tasks.Task"/> and should be refactored or deprecated in the future.
/// </remarks>
public class WorkerTask
{
    /// <summary>
    /// Friendly name used for logging.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Delegate to worker function that is be executed.
    /// </summary>
    public WorkerMethod? Method { get; set; }

    /// <summary>
    /// Execution timeout in seconds, after which the cancel flag will be set.
    /// Set to zero to disable.
    /// </summary>
    public int RunTimeout { get; set; }

    /// <summary>
    /// Additional time in seconds the thread is allowed to stop after receiving a cancellation request.
    /// The thread will be aborted if it exceeds this timeout.
    /// Set to zero to disable.
    /// </summary>
    public int StopTimeout { get; set; }

    /// <summary>
    /// Cancellation token used to abort the worker task.
    /// </summary>
    public CancellationTokenSource Cancel { get; set; }
}
