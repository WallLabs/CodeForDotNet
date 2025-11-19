using CodeForDotNet.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeForDotNet.Tests.Facts
{
	/// <summary>
	/// Tests the <see cref="ProcessExtensions"/> class.
	/// </summary>
	[TestClass]
	public class ProcessTests
	{
		#region Public Methods

		/// <summary>
		/// Tests the <see cref="ProcessExtensions.Run"/> method.
		/// </summary>
		[TestMethod]
		public void ProcessTestRun()
		{
            // Run simple command which should exit successfully
            var output = ProcessExtensions.Run("%COMSPEC%", "/C DIR /A", workingDirectory: "%TEMP%", timeout: 2);
			Assert.IsGreaterThan(0, output.Output.Length);
			Assert.IsFalse(output.TimedOut);
			Assert.IsTrue(output.ReturnCode.HasValue);
			Assert.AreEqual(0, output.ReturnCode);

			// Run command with timeout that will never exit (test timeout)
			output = ProcessExtensions.Run("MSPAINT.EXE", timeout: 2);
			Assert.AreEqual(0, output.Output.Length);
			Assert.IsTrue(output.TimedOut);
			Assert.IsFalse(output.ReturnCode.HasValue);
		}

		#endregion Public Methods
	}
}
