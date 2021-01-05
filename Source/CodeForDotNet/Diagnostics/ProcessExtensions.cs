using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeForDotNet.Diagnostics
{
	/// <summary>
	/// Extends the <see cref="Process"/> class.
	/// </summary>
	public static class ProcessExtensions
	{
        #region Public Methods

        /// <summary>
        /// Creates and executes a process with timeout, capturing the results.
        /// </summary>
        /// <param name="program">Program to execute. Must not be null</param>
        /// <param name="parameters">Parameters to pass. Can be null. Can contain environment variables</param>
        /// <param name="workingDirectory">Working directory to use. Can be relative. Null or empty means current directory.</param>
        /// <param name="timeout">Optional timeout in seconds.</param>
        /// <returns><see cref="ProcessResult"/> with set ReturnCode, Output, Errors, TimedOut</returns>
        public static ProcessResult Run(string program, string? parameters = null, string? workingDirectory = null, int? timeout = null)
		{
			// Validate
			if (string.IsNullOrEmpty(program)) throw new ArgumentNullException(nameof(program));
			if (timeout.HasValue && timeout <= 0) throw new ArgumentOutOfRangeException(nameof(timeout));

			// Run program and wait for exit
			var start = new ProcessStartInfo
			{
				FileName = Environment.ExpandEnvironmentVariables(program),
				UseShellExecute = false,            // Required when redirecting output
				RedirectStandardOutput = true,      // Capture output so we can check it
				RedirectStandardError = true,       // Capture error so we can check it
				CreateNoWindow = true               // We are redirecting the output = nothing to see!
			};
			if (parameters != null) start.Arguments = Environment.ExpandEnvironmentVariables(parameters);
			if (workingDirectory != null) start.WorkingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);

			// Create process
			using var process = new Process { StartInfo = start };

			// Receiving output as it is generated (otherwise it will hang when the buffer is full)
			var consoleOutput = new StringBuilder();
			process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs args)
			{
				if (args.Data != null)
					consoleOutput.AppendLine(args.Data);
			};
			var consoleErrors = new StringBuilder();
			process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs args)
			{
				if (args.Data != null)                  // Null output may be received, e.g. during kill of windowed program
					consoleErrors.Append(args.Data);
			};

			// Start process and wait for process to complete with optional timeout
			if (process.Start())
			{
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit(timeout.HasValue ? timeout.Value * 1000 : int.MaxValue);
			}

			// Kill process when timed out
			var output = new ProcessResult();
			if (!process.HasExited)
			{
				try
				{
					// Try to close windows programs normally first
					if (process.CloseMainWindow())
						process.WaitForExit(1000);

					// Kill process when still running
					if (!process.HasExited)
					{
						process.Kill();
						process.WaitForExit();
					}

					// Flag timeout
					output.TimedOut = true;
				}
				catch (InvalidOperationException) { /* Ignore error when process exits whilst we process timeout */}
			}

			// Return Output
			if (!output.TimedOut)
				output.ReturnCode = process.ExitCode;
			process.CancelOutputRead();
			output.Output = consoleOutput.ToString();
			process.CancelErrorRead();
			output.Errors = consoleErrors.ToString();
			return output;
		}

		#endregion Public Methods
	}
}
