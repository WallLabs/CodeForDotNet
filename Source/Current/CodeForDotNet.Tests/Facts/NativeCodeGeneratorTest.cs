using CodeForDotNet.Windows.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace CodeForDotNet.Tests.Facts
{
    /// <summary>
    /// Tests the NativeCodeGenerator class.
    /// </summary>
    [TestClass]
    public class NativeCodeGeneratorTest
    {
        /// <summary>
        /// Tests the <see cref="NativeCodeGenerator"/>.
        /// </summary>
        [TestMethod]
        public void InstallTestNativeCodeGenerator()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            Assert.IsTrue(NativeCodeGenerator.Install(assemblyPath, out string consoleOutput, out string assemblyName), consoleOutput);
            Assert.IsTrue(NativeCodeGenerator.Install(assemblyPath, out consoleOutput, out assemblyName), consoleOutput);
            Assert.IsTrue(NativeCodeGenerator.Remove(assemblyPath, out consoleOutput), consoleOutput);
            Assert.IsFalse(NativeCodeGenerator.Remove(assemblyPath, out consoleOutput), consoleOutput);
        }
    }
}
