using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

#nullable enable

namespace CodeForDotNet.Windows.Native
{
	/// <summary>
	/// Helper class that executes NGEN on target assemblies.
	/// </summary>
	public static class NativeCodeGenerator
	{
		#region Public Methods

		/// <summary>
		/// Installs the specified assembly into the native image cache, including all dependencies.
		/// </summary>
		/// <param name="assemblyPath">Fully qualified local file system path to the target assembly.</param>
		/// <param name="consoleOutput">Any console output. Use to obtain error text.</param>
		/// <param name="assemblyFullName">Assembly FullName which was queued for install into the cache.</param>
		public static bool Install(string assemblyPath, out string consoleOutput, out string? assemblyFullName)
		{
			// Execute NGEN to install the assembly
			var parameters = "install \"" + assemblyPath + "\" /nologo /silent";
			var returnCode = ExecuteProgram(GetNgenPath(), parameters, Path.GetDirectoryName(assemblyPath), 5, out consoleOutput);
			if (returnCode == 0)
			{
				// Get assembly full name if successfully installed into cache
				var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
				assemblyFullName = assembly.FullName;
				return true;
			}

			// Failed
			assemblyFullName = null;
			return false;
		}

		/// <summary>
		/// Removes an assembly from the cache.
		/// </summary>
		/// <param name="assemblyName">Assembly name, partial or fully qualified.</param>
		/// <param name="consoleOutput">Console output (if any) during execution.</param>
		public static bool Remove(string assemblyName, out string consoleOutput)
		{
			// Validate
			if (string.IsNullOrEmpty(assemblyName))
				throw new ArgumentNullException(nameof(assemblyName));

			// Execute NGEN to remove the assembly
			var parameters = "uninstall \"" + assemblyName.Trim('\"') + "\" /nologo /silent";
			var program = GetNgenPath();
			var returnCode = ExecuteProgram(program, parameters, Path.GetDirectoryName(program), 5, out consoleOutput);

			// Return result
			return (returnCode == 0);
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Executes a program with the specified parameters and working directory, waits for exit and
		/// </summary>
		/// <param name="programPath">Path of the program to execute.</param>
		/// <param name="parameters">Parameters to pass to the program.</param>
		/// <param name="workingDirectory">Working directory in which to execute.</param>
		/// <param name="timeout">Maximum time to allow the program to execute, before terminating with -1 return code.</param>
		/// <param name="consoleOutput">Console output (if any) during execution.</param>
		/// <returns>Return code, 0 = success, -1 = timeout or other error, other values are program specific.</returns>
		static private int ExecuteProgram(string programPath, string parameters, string workingDirectory,
			int timeout, out string consoleOutput)
		{
			// Prepare to run NGEN
			var startInfo = new ProcessStartInfo(programPath)
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				Arguments = parameters,
				WorkingDirectory = workingDirectory.Trim('"').TrimEnd(Path.DirectorySeparatorChar)
			};

			// Start program with parameters
			var process = Process.Start(startInfo);

			// Wait for program to exit or timeout
			var timeoutCounter = 60 * timeout;
			while (!process.HasExited)
			{
				System.Threading.Thread.Sleep(1000);
				if (--timeoutCounter == 0)
				{
					// Timeout
					process.Kill();
					break;
				}
			}

			// Get console output text (if any)
			consoleOutput = string.Empty;
			if (process.StandardOutput.Peek() != -1)
				consoleOutput = process.StandardOutput.ReadToEnd();

			// Return result code
			return process.ExitCode;
		}

		/// <summary>
		/// Gets the path to the .Net Framework.
		/// </summary>
		static private string GetNgenPath()
		{
			// Gets the path to the Framework directory
			var path = new StringBuilder(1024);
			var size = 0;
			var returnCode = SafeNativeMethods.GetCORSystemDirectory(path, path.Capacity, ref size);
			if (returnCode != 0)
				throw new InvalidOperationException();
			return path.ToString().TrimEnd('\\') + "\\ngen.exe";
		}

		#endregion Private Methods
	}
}
