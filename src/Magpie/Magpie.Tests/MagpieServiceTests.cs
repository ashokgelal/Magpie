using System;
using System.Linq;
using Magpie.Models;
using Magpie.Services;
using Magpie.Tests.Mocks;
using Magpie.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests
{
    [TestClass]
    public class MagpieServiceTests
    {
        private MockMagpieUpdater _mockMagpieUpdater;

        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
            _mockMagpieUpdater = new MockMagpieUpdater("validContentUrl");
        }

        [TestMethod]
        public void TestValidJson()
        {
            _mockMagpieUpdater.CheckInBackground();
            var appcast = _mockMagpieUpdater.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("bar", appcast.RawDictionary["foo"]);
            Assert.AreEqual(new Version(5, 8, 8), appcast.Channels.First().Version);
            Assert.AreEqual(2, appcast.RawDictionary.Count);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            _mockMagpieUpdater.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            _mockMagpieUpdater.CheckInBackground();
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void TestNoUpdatesWindowShownOnForceCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpieUpdater.UpdateDecider = updateDecider;
            _mockMagpieUpdater.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieUpdater._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestNoUpdatesWindowNotShownOnNormalCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>()).Returns(false);
            _mockMagpieUpdater.UpdateDecider = updateDecider;
            _mockMagpieUpdater.CheckInBackground();
            Assert.IsFalse(_mockMagpieUpdater._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestUpdateWindowShown()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpieUpdater.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieUpdater._showUpdateWindowFlag);
        }

        [TestMethod]
        public void TestForceCheckOverridingAppCastUrl()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpieUpdater.ForceCheckInBackground("alternateUrl");
            _mockMagpieUpdater._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
        }

        [TestMethod]
        public void TestBackgroundCheckOverridingAppCastUrl()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpieUpdater.CheckInBackground("alternateUrl");
            _mockMagpieUpdater._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
        }
    }
}