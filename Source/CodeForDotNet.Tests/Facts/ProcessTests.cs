using CodeForDotNet.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#nullable enable

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
			var output = ProcessExtensions.Run("%COMSPEC%", "/C DIR /A", "%TEMP%", 2);
			Assert.IsTrue(output.Output?.Length > 0);
			Assert.IsFalse(output.TimedOut);
			Assert.IsTrue(output.ReturnCode.HasValue);
			Assert.IsTrue(output.ReturnCode == 0);

			// Run command with timeout that will never exit (test timeout)
			output = ProcessExtensions.Run("NOTEPAD.EXE", null, null, 2);
			Assert.IsTrue(output.Output?.Length == 0);
			Assert.IsTrue(output.TimedOut);
			Assert.IsFalse(output.ReturnCode.HasValue);
		}

		#endregion Public Methods
	}
}
