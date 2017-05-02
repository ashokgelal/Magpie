using System;
using System.Linq;
using System.Threading.Tasks;
using Magpie.Tests.Mocks;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.Services;
using MagpieUpdater.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.Services
{
    [TestClass]
    public class MagpieTest
    {
        private MockMagpie _mockMagpie;
        private IAnalyticsLogger _analyticsLogger;

        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
            _analyticsLogger = Substitute.For<IAnalyticsLogger>();
            _mockMagpie = new MockMagpie("validContentUrl", analyticsLogger: _analyticsLogger);
        }

        [TestMethod]
        public void TestValidJson()
        {
            _mockMagpie.CheckInBackground();
            var appcast = _mockMagpie.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("bar", appcast.RawDictionary["foo"]);
            Assert.AreEqual(new Version(5, 8, 8), appcast.Channels.First().Version);
            Assert.AreEqual(2, appcast.RawDictionary.Count);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            _mockMagpie.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            _mockMagpie.CheckInBackground();
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void TestNoUpdatesWindowShownOnForceCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpie._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestNoUpdatesWindowNotShownOnNormalCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>()).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.CheckInBackground();
            Assert.IsFalse(_mockMagpie._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestUpdateWindowShown()
        {
            _mockMagpie.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpie.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpie._showUpdateWindowFlag);
        }

        [TestMethod]
        public void TestForceCheckOverridingAppCastUrl()
        {
            _mockMagpie.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpie.ForceCheckInBackground("alternateUrl");
            _mockMagpie._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl", Arg.Any<IDebuggingInfoLogger>());
        }

        [TestMethod]
        public void TestBackgroundCheckOverridingAppCastUrl()
        {
            _mockMagpie.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(true);
            _mockMagpie.CheckInBackground("alternateUrl");
            _mockMagpie._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl", Arg.Any<IDebuggingInfoLogger>());
        }

        [TestMethod]
        public void TestSwitchSubscribedChannelForceChecksForUpdates()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.SwitchSubscribedChannel(2);
            updateDecider.Received(1).ShouldUpdate(Arg.Is<Channel>(e => e.Id == 2), true);
        }

        [TestMethod]
        public void SwitchingSubscribedThenCallingForceUpdateChecksNewChannel()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.SwitchSubscribedChannel(3);
            _mockMagpie.ForceCheckInBackground();
            updateDecider.Received(2).ShouldUpdate(Arg.Is<Channel>(e => e.Id == 3), true);
        }

        [TestMethod]
        public void SwitchingToChannelThatDoesNotRequireEnrollment_ChecksForUpdates()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.SwitchSubscribedChannel(3);
            
            Assert.IsFalse(_mockMagpie._showEnrollmentWindow);
            updateDecider.Received(1).ShouldUpdate(Arg.Is<Channel>(e => e.Id == 3), true);
        }

        [TestMethod]
        public void SwitchingToChannelThatRequiresEnrollment_ShowsEnrollmentWindow()
        {
            _mockMagpie.SwitchSubscribedChannel(4);
            Assert.IsTrue(_mockMagpie._showEnrollmentWindow);
        }

        [TestMethod]
        public void SwitchingChannel_FailingToEnroll_DoesNotCheckForUpdates()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()){IsEnrolled = false};
            _mockMagpie.SwitchSubscribedChannel(4);

            Assert.IsTrue(_mockMagpie._showEnrollmentWindow);
            updateDecider.DidNotReceive().ShouldUpdate(Arg.Any<Channel>(), Arg.Any<bool>());
        }

        [TestMethod]
        public void SwitchingChannel_SuccessfullyEnrolled_CheckForUpdates()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider = updateDecider;
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()) { IsEnrolled = true };
            _mockMagpie.SwitchSubscribedChannel(4);

            Assert.IsTrue(_mockMagpie._showEnrollmentWindow);
            updateDecider.Received(1).ShouldUpdate(Arg.Is<Channel>(e => e.Id == 4), true);
        }

        [TestMethod]
        public void SwitchingChannel_LogsEnrollment()
        {
            _mockMagpie.SwitchSubscribedChannel(3);
            _analyticsLogger.Received(1).LogEnrollment(Arg.Any<Enrollment>());
        }

        [TestMethod]
        public void SwitchingChannel_EnrollmentAvailableEventGetsFired()
        {
            var raised = false;
            _mockMagpie.EnrollmentAvailableEvent += (s, a) => { raised = true; };
            _mockMagpie.SwitchSubscribedChannel(3);
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void SwitchingChannel_SuccessfullyEnrolled_UpdatesSubscribedChannel()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider = updateDecider;
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()) { IsEnrolled = true };
            _mockMagpie.SwitchSubscribedChannel(4);

            Assert.AreEqual(4, _mockMagpie.AppInfo.SubscribedChannel);
        }

        [TestMethod]
        public void SwitchingChannel_FailingToEnroll_DoesNotUpdateSubscribedChannel()
        {
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()) { IsEnrolled = false };
            _mockMagpie.SwitchSubscribedChannel(4);

            Assert.AreNotEqual(4, _mockMagpie.AppInfo.SubscribedChannel);
        }

        [TestMethod] 
        public void SwitchingChannel_ChannelDoesNotRequireEnrollment_UpdatesSubscribedChannel()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            _mockMagpie.SwitchSubscribedChannel(3);
            
            Assert.AreEqual(3, _mockMagpie.AppInfo.SubscribedChannel);
        }

        [TestMethod]
        public async Task SwitchingChannelAsync_SuccessfullyEnrolled_ReturnsTrue()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpie.UpdateDecider = updateDecider;
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()) { IsEnrolled = true };
            var success = await _mockMagpie.SwitchSubscribedChannelAsync(4);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task SwitchingAsync_ChannelDoesNotRequireEnrollment_ReturnsTrue()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<Channel>(), true).Returns(false);
            _mockMagpie.UpdateDecider = updateDecider;
            var success = await _mockMagpie.SwitchSubscribedChannelAsync(3);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task SwitchingChannelAsync_FailingToEnroll_ReturnsFalse()
        {
            _mockMagpie._enrollmentToReturn = new Enrollment(new Channel()) { IsEnrolled = false };
            var success = await _mockMagpie.SwitchSubscribedChannelAsync(4);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public async Task SwitchingAsync_AppCastIsEmpty_ReturnsFalse()
        {
            _mockMagpie.RemoteContentDownloader.DownloadStringContent(Arg.Any<string>(), Arg.Any<IDebuggingInfoLogger>()).Returns(Task.FromResult(string.Empty));
            var success = await _mockMagpie.SwitchSubscribedChannelAsync(3);

            Assert.IsFalse(success);
        }
    }
}