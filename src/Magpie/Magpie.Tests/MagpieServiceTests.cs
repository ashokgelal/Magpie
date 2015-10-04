using System;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests
{
    [TestClass]
    public class MagpieServiceTests
    {
        [TestMethod]
        public void TestValidJson()
        {
            var mockMagpieService = new MockMagpieService("validContentUrl");
            mockMagpieService.RunInBackground("validContentUrl");
            var appcast = mockMagpieService.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("Magpie", appcast.Title);
            Assert.AreEqual(new Version(0, 0, 1), appcast.Version);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            var mockMagpieService = new MockMagpieService("validContentUrl");
            mockMagpieService.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            mockMagpieService.RunInBackground("validContentUrl");
            Assert.IsTrue(raised);
        }
    }
}
