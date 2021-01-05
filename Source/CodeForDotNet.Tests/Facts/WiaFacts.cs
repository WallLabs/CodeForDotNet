using CodeForDotNet.Windows.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Versioning;

namespace CodeForDotNet.Tests.Facts
{
    /// <summary>
    /// WIA unit tests.
    /// </summary>
    /// <remarks>
    /// These tests only work with a scanner attached. As we are really testing a thin wrapper over the relatively large WIA API it is not feasible at this time
    /// to use mocks.
    /// </remarks>
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class WiaFacts
    {
        #region Public Methods

        /// <summary>
        /// Tests the <see cref="WiaDevice"/> class.
        /// </summary>
        [TestMethod, Ignore("Needs to be mocked so runs without hardware.")]
        public void WiaDeviceTest()
        {
            using var manager = new WiaManager();
            var devicesInfo = manager.GetDevices();
            Assert.IsTrue(devicesInfo.Count > 0);
            foreach (var deviceInfo in devicesInfo)
            {
                var device = deviceInfo.Connect();
                var commands = device.Commands;
                Assert.IsTrue(commands.Count > 0);

                // Scan
                // TODO: Make this more intuitive... var imageFile = device.Items[0].Transfer();
            }
        }

        #endregion Public Methods
    }
}
