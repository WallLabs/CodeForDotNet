using System.Diagnostics;

namespace CodeChief.Threading
{
    /// <summary>
    /// Worker host providing logging and cancellation.
    /// </summary>
    public interface IWorkerHost
    {
        /// <summary>
        /// Trace source used for logging.
        /// </summary>
        TraceSource Log { get; }

        /// <summary>
        /// Gets a value indicating whether the worker should cancel its work.
        /// </summary>
        bool Canceled { get; }
    }
}
