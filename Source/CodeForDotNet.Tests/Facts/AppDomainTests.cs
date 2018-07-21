using CodeForDotNet.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace CodeForDotNet.Tests.Facts
{
    /// <summary>
    /// Tests the <see cref="AppDomainExtensions"/> class.
    /// </summary>
    [TestClass]
    public class AppDomainTests
    {
        /// <summary>
        /// Tests the <see cref="AppDomainExtensions.Run"/> method.
        /// </summary>
        [TestMethod]
        public void AppDomainTestRun()
        {
            // Run a disposable instance, e.g. a process which runs a simple command
            var testMethod = typeof(Process).GetMethod("Start", new[] { typeof(String), typeof(String) });
            AppDomain.CurrentDomain.Run(testMethod, Environment.ExpandEnvironmentVariables("%COMSPEC%"), "/C DIR");
        }
    }
}
