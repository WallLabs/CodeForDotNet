using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeForDotNet.Windows.Imaging;
using Xunit;

namespace CodeForDotNet.Tests.Facts
{
    /// <summary>
    /// WIA unit tests.
    /// </summary>
    /// <remarks>
    /// These tests only work with a scanner attached.
    /// As we are really testing a thin wrapper over the relatively large WIA API
    /// it is not feasible at this time to use mocks.
    /// </remarks>
    public static class WiaFacts
    {
        /// <summary>
        /// Tests the <see cref="WiaDevice"/> class.
        /// </summary>
        [Fact(DisplayName = "WIA Device")]
        public static void WiaDeviceTest()
        {
            using (var manager = new WiaManager())
            {
                var devicesInfo = manager.GetDevices();
                Assert.True(devicesInfo.Count > 0);
                foreach (var deviceInfo in devicesInfo)
                {
                    var device = deviceInfo.Connect();
                    var commands = device.Commands;
                    Assert.True(commands.Count > 0);
                    
                    // Scan
                    // TODO: Make this more intuitive...
                    // var imageFile = device.Items[0].Transfer();
                }
            }
        }
    }
}
