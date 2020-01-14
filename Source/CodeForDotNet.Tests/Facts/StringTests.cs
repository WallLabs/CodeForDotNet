using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeForDotNet.Tests.Facts
{
	/// <summary>
	/// Tests the <see cref="StringExtensions"/> class.
	/// </summary>
	[TestClass]
    [SuppressMessage("Microsoft.Globalization", "CA1303: Do not pass literals as localized parameters", Justification = "Test data.")]
    public class StringTests
    {
        #region Public Methods

        /// <summary>
        /// Test the <see cref="StringExtensions.SplitEscaped"/> method.
        /// </summary>
        [TestMethod]
        public void StringTestSplitEscaped()
		{
			var test = @"abc, de\,f, gh\=i".SplitEscaped(',', '\\', false, false);
			Assert.IsTrue(test[0].Equals(@"abc", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(test[1].Equals(@" de\,f", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(test[2].Equals(@" gh\=i", StringComparison.OrdinalIgnoreCase));

			test = @"abc, de\,f,, gh\=i".SplitEscaped(',', '\\', true, true);
			Assert.IsTrue(test[0].Equals(@"abc", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(test[1].Equals(@" de,f", StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(test[2].Equals(@" gh=i", StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Tests the <see cref="StringExtensions.Truncate(string, int, string)"/> method and overload.
		/// </summary>
		[TestMethod]
		public void StringTestTruncate()
		{
			const string original = "This is a very long string that we want to truncate";

			// Test values which should not be truncated
			Assert.AreEqual(original, original.Truncate(original.Length));
			Assert.AreEqual(original, original.Truncate(original.Length + 10));

			// Test truncation with default ending
			Assert.AreEqual("This is a very lo...", original.Truncate(20));

			// Test truncation with different and shorter ending
			Assert.AreEqual("This is a very long>", original.Truncate(20, ">"));
		}

		#endregion Public Methods
	}
}
