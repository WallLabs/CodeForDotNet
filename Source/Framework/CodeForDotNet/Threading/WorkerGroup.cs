using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using CodeForDotNet.Properties;
using Microsoft.Extensions.Logging;

namespace CodeForDotNet.Threading;

/// <summary>
/// Executes a sequence of worker functions in the background.
/// </summary>
/// <remarks>
/// These classes were created before <see cref="System.Threading.Tasks.Task"/> and should be refactored or deprecated in the future.
/// </remarks>
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "TODO: Update to LoggerMessageAttribute.")]
[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "TODO: Update to LoggerMessageAttribute.")]
[SuppressMessage("Performance", "CA1863:Use 'CompositeFormat'", Justification = "TODO: Update to LoggerMessageAttribute.")]
[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "TODO: Update to LoggerMessageAttribute.")]
public class WorkerGroup : DisposableObject, IWorkerHost, IThreadSafe
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public WorkerGroup(string name, ILogger log, bool pollEnabled, int pollInterval = 0)
    {
        // Initialize members
        Name = name;
        Log = log;
        PollEnabled = pollEnabled;
        PollInterval = pollInterval;
        _workers = [];
        _runTimer = new System.Timers.Timer { AutoReset = false };
        _runTimer.Elapsed += OnRunTimeout;
        _stopTimer = new System.Timers.Timer { AutoReset = false };
        _stopTimer.Elapsed += OnStopTimeout;
        _currentWorkerId = -1;
        _thread = new Thread(RunWorker);
        SyncRoot = new object();
    }

    /// <summary>
    /// Frees all resources belonging to this instance.
    /// </summary>
    /// <param name="disposing">True when called via <see cref="IDisposable.Dispose()"/>, false when called via finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Dispose managed resources during dispose
            if (disposing)
            {
                _runTimer?.Dispose();
                _stopTimer?.Dispose();
            }
        }
        finally
        {
            // Dispose base class
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Worker thread.
    /// </summary>
    private Thread _thread;

    /// <summary>
    /// Timer used to cancel the worker.
    /// </summary>
    private readonly System.Timers.Timer _runTimer;

    /// <summary>
    /// Timer used to abort the worker.
    /// </summary>
    private readonly System.Timers.Timer _stopTimer;

    /// <summary>
    /// The id of the currently running worker.
    /// </summary>
    private int _currentWorkerId;

    /// <summary>
    /// The currently running worker.
    /// </summary>
    private WorkerTask? _currentWorker;

    /// <summary>
    /// Workers to execute (in sequence).
    /// </summary>
    private readonly List<WorkerTask> _workers;

    /// <summary>
    /// Synchronization object.
    /// </summary>
    public object SyncRoot { get; private set; }

    /// <summary>
    /// Visual name of the worker group.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets or sets the trace source used for logging.
    /// </summary>
    public ILogger Log { get; private set; }

    /// <summary>
    /// Enables iteration.
    /// </summary>
    public bool PollEnabled { get; set; }

    /// <summary>
    /// Gets the time in seconds to wait between iterations of all workers in the group.
    /// </summary>
    public int PollInterval
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the execution of the current worker should be canceled.
    /// </summary>
    public bool Canceled { get; private set; }

    /// <summary>
    /// Gets a value that indicates whether the worker group is stopping.
    /// </summary>
    public bool Stopping { get; private set; }

    /// <summary>
    /// Gets a value that indicates whether the worker is currently executing.
    /// </summary>
    public bool Running => _thread != null && _thread.IsAlive;

    /// <summary>
    /// Gets a value that indicates whether the worker has been aborted.
    /// </summary>
    public bool Aborted => _thread != null &&
                (_thread.ThreadState == System.Threading.ThreadState.AbortRequested ||
                _thread.ThreadState == System.Threading.ThreadState.Aborted);

    /// <summary>
    /// Adds a worker to the end of the execution group.
    /// </summary>
    /// <param name="name">Friendly name for logging.</param>
    /// <param name="method">Worker method.</param>
    /// <param name="runTimeout">Optional execution timeout in seconds. Set to zero for no timeout.</param>
    /// <param name="stopTimeout">Optional stop timeout in seconds, after which the thread is aborted. Set to zero for no timeout.</param>
    /// <exception cref="InvalidOperationException">Thrown when the group is running so cannot be changed.</exception>
    public void Add(string name, WorkerMethod method, int runTimeout = 0, int stopTimeout = 0)
    {
        lock (SyncRoot)
        {
            // Check if the thread is running already
            if (Running)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                                            Resources.WorkerGroupAlreadyRunning, Name);
                throw new InvalidOperationException(message);
            }

            // Add worker
            _workers.Add(new WorkerTask {
                Method = method,
                Name = name,
                RunTimeout = runTimeout,
                StopTimeout = stopTimeout
            });
        }
    }

    /// <summary>
    /// Starts executing the worker group.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the group is already running.</exception>
    public void Start()
    {
        lock (SyncRoot)
        {
            // Log event
            Log.LogInformation(0, Resources.WorkerGroupStarting, Name);

            // Check if the thread is running already
            if (Running)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                                            Resources.WorkerGroupAlreadyRunning, Name);
                throw new InvalidOperationException(message);
            }

            // Nothing to do if no workers inside.
            if (_workers.Count == 0)
                return;

            // Reset flags
            Canceled = false;
            Stopping = false;

            // Execute workers
            _thread = new Thread(RunWorker);
            _thread.Start();
        }
    }

    /// <summary>
    /// Requests the worker group stops, any running task should exit normally
    /// at the next opportunity and no new work will be started.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Abort"/> to kill any running task.
    /// </remarks>
    public void Stop()
    {
        lock (SyncRoot)
        {
            // Log event
            Log.LogInformation(0, Resources.WorkerGroupStopping, Name);

            // Signal stop
            Canceled = true;
            Stopping = true;
        }
    }

    /// <summary>
    /// Stops the worker group, aborting any running work.
    /// </summary>
    /// <remarks>
    /// Does nothing when not <see cref="Running"/> or already <see cref="Aborted"/>.
    /// </remarks>
    public void Abort()
    {
        lock (SyncRoot)
        {
            // Do nothing when not running or already aborted
            if (!Running || Aborted)
                return;

            // Log event
            Log.LogWarning(0, Resources.WorkerGroupAborting, Name);

            // Signal stop (if not already requested)
            Canceled = true;
            Stopping = true;

            // Prevent threads from re-starting
            if (_runTimer.Enabled)
                _runTimer.Stop();
            if (_stopTimer.Enabled)
                _stopTimer.Stop();

            // Abort any running thread
            if (Running && !_currentWorker.Cancel.IsCancellationRequested)
                _currentWorker.Cancel.Cancel();
        }
    }

    /// <summary>
    /// Waits for execution to stop.
    /// </summary>
    public void Wait()
    {
        // Wait for all workers to stop
        while (Running)
            Thread.Sleep(1000);
    }

    /// <summary>
    /// Waits for execution to stop with a timeout.
    /// </summary>
    /// <remarks>
    /// The thread is NOT aborted if the <paramref name="timeout"/> is exceeded.
    /// </remarks>
    /// <param name="timeout">Maximum time to wait in seconds. This does not affect the running thread.</param>
    /// <returns>True when returned because the thread stopped within the <paramref name="timeout"/>, otherwise false.</returns>
    public bool Wait(int timeout)
    {
        // Wait for all workers to stop
        var ticks = 0;
        while (Running)
        {
            // Wait 1 second...
            Thread.Sleep(1000);

            // Return false did not stop within interval
            if (ticks++ >= timeout)
                return false;
        }

        // Return true when stopped
        return true;
    }

    /// <summary>
    /// Pauses between execution of workers, checking for the <see cref="Stopping"/> signal.
    /// </summary>
    /// <param name="seconds">Time to wait in seconds.</param>
    private void Pause(int seconds)
    {
        while (!Stopping && seconds-- > 0)
            Thread.Sleep(1000);
    }

    /// <summary>
    /// Runs all workers in the group, with wait interval, until stopped.
    /// </summary>
    /// <remarks>
    /// Invoked on a background thread.
    /// </remarks>
    private void RunWorker()
    {
        // Execute until stopped...
        while (!Stopping)
        {
            // Get the next worker and wait when poll complete
            _currentWorkerId++;
            if (_currentWorkerId >= _workers.Count)
            {
                // Iterate back to the first worker (when enabled)
                if (!PollEnabled)
                    return;
                _currentWorkerId = 0;

                // Wait till we have to run again
                Pause(PollInterval);
            }
            _currentWorker = _workers[_currentWorkerId];

            // Do nothing when stopped during poll interval
            if (Stopping) return;

            // Prepare to start new work
            lock (SyncRoot)
            {
                // Set the timer if required
                if (_currentWorker.RunTimeout > 0)
                {
                    Log.LogDebug(0, Resources.WorkerGroupStartingRunTimer,
                                   _currentWorker.Name, Name, _currentWorker.RunTimeout);
                    _runTimer.Interval = _currentWorker.RunTimeout * 1000;
                    _runTimer.Start();
                }

                // Clear canceled flag (only affects last thread, does not cancel group)
                Canceled = false;
            }

            // Execute work...
            try
            {
                // Execute worker method in this (background) thread
                Log.LogDebug(0, Resources.WorkerGroupStartingWorker,
                               _currentWorker!.Name, Name);
                _currentWorker.Method!(this);
            }
            catch (ThreadAbortException)
            {
                // Aborted (warning - should just have come from our abort method or stop timeout)
                Log.LogWarning(0, Resources.WorkerGroupAbortedWorker,
                               _currentWorker!.Name, Name);
            }
            catch (Exception error)
            {
                // Failed (error)
                Log.LogError(0, Resources.WorkerGroupError,
                               _currentWorker!.Name, Name, error.GetFullMessage(true));
            }
            finally
            {
                // Clean-up
                lock (SyncRoot)
                {
                    // Stop timers
                    if (_runTimer.Enabled)
                        _runTimer.Stop();
                    if (_stopTimer.Enabled)
                        _stopTimer.Stop();
                }
            }
        }
    }

    /// <summary>
    /// Signals the thread to stop when the run timeout is exceeded.
    /// </summary>
    /// <param name="sender">Sender of this event.</param>
    /// <param name="timeout">Event arguments.</param>
    private void OnRunTimeout(object? sender, System.Timers.ElapsedEventArgs timeout)
    {
        lock (SyncRoot)
        {
            // Signal thread to stop
            Log.LogWarning(0, Resources.WorkerGroupStoppingWorker,
                           _currentWorker!.Name, Name);
            Canceled = true;

            // Set the stop timeout if specified, to abort the thread if necessary
            if (_currentWorker.StopTimeout > 0)
            {
                Log.LogDebug(0, Resources.WorkerGroupStartingStopTimer,
                               _currentWorker.Name, Name, _currentWorker.RunTimeout);
                _stopTimer.Interval = _currentWorker.StopTimeout * 1000;
                _stopTimer.Start();
            }
        }
    }

    /// <summary>
    /// Aborts the thread when it was signaled to stop but exceeded the stop timeout.
    /// Execution resumes with the next worker.
    /// </summary>
    /// <param name="sender">Sender of this event.</param>
    /// <param name="stop">Event arguments.</param>
    private void OnStopTimeout(object? sender, System.Timers.ElapsedEventArgs stop)
    {
        lock (SyncRoot)
        {
            // Abort the current thread
            Log.LogWarning(0, Resources.WorkerGroupAbortingWorker,
                           _currentWorker!.Name, Name);
            if (Running && !_currentWorker.Cancel.IsCancellationRequested)
                _currentWorker.Cancel.Cancel();

            // Start next worker if group not stopping
            if (!Stopping)
                _thread.Start();
        }
    }
}

/// <summary>
/// Method to execute when the worker runs.
/// </summary>
/// <param name="host">Host in which the worker is running, should be used for all logging and checked frequently for cancellation.</param>
public delegate void WorkerMethod(IWorkerHost host);
