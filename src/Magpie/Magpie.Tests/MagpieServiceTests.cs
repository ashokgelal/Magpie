using System;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests
{
    [TestClass]
    public class MagpieServiceTests
    {
        private const string VALID_JSON = @"{ 'title': 'Magpie', 'version': '0.0.1', 'build_date': '10/03/2015', 'release_notes_url': '', 'artifact_url': 'https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.zip' }";

        [TestMethod]
        public void TestValidJson()
        {
            var validJson = VALID_JSON.Replace("'", "\"");
            var mockMagpieService = new MockMagpieService(validJson);
            mockMagpieService.RunInBackground("testUrl");
            var appcast = mockMagpieService.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("Magpie", appcast.Title);
            Assert.AreEqual(new Version(0, 0, 1), appcast.Version);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            var validJson = VALID_JSON.Replace("'", "\"");
            var mockMagpieService = new MockMagpieService(validJson);
            mockMagpieService.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            mockMagpieService.RunInBackground("testUrl");
            Assert.IsTrue(raised);
        }
    }
}
