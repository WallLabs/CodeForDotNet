using System.Reflection;
using CodeForDotNet.Windows.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeForDotNet.Tests.Facts
{
   /// <summary>
   /// Tests the NativeCodeGenerator class.
   /// </summary>
   [TestClass]
   public class NativeCodeGeneratorTest
   {
      #region Public Methods

      /// <summary>
      /// Tests the <see cref="NativeCodeGenerator"/>.
      /// </summary>
      [TestMethod]
      public void InstallTestNativeCodeGenerator()
      {
         var assemblyPath = Assembly.GetExecutingAssembly().Location;
         Assert.IsTrue(NativeCodeGenerator.Install(assemblyPath, out var consoleOutput, out _), consoleOutput);
         Assert.IsTrue(NativeCodeGenerator.Install(assemblyPath, out consoleOutput, out _), consoleOutput);
         Assert.IsTrue(NativeCodeGenerator.Remove(assemblyPath, out consoleOutput), consoleOutput);
         Assert.IsFalse(NativeCodeGenerator.Remove(assemblyPath, out consoleOutput), consoleOutput);
      }

      #endregion Public Methods
   }
}
