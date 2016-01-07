using System;
using System.Reflection;
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
        private MockMagpieService _mockMagpieService;

        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
            _mockMagpieService = new MockMagpieService("validContentUrl");
        }

        [TestMethod]
        public void TestValidJson()
        {
            _mockMagpieService.CheckInBackground();
            var appcast = _mockMagpieService.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("Magpie", appcast.Title);
            Assert.AreEqual(new Version(0, 0, 1), appcast.Version);
            Assert.AreEqual(5, appcast.RawDictionary.Count);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            _mockMagpieService.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            _mockMagpieService.CheckInBackground();
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void TestNoUpdatesWindowShownOnForceCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(false);
            _mockMagpieService.UpdateDecider = updateDecider;
            _mockMagpieService.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieService._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestNoUpdatesWindowNotShownOnNormalCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>()).Returns(false);
            _mockMagpieService.UpdateDecider = updateDecider;
            _mockMagpieService.CheckInBackground();
            Assert.IsFalse(_mockMagpieService._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestUpdateWindowShown()
        {
            _mockMagpieService.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieService.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieService.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieService._showUpdateWindowFlag);
        }

        [TestMethod]
        public void TestForceCheckOverridingAppCastUrl()
        {
            _mockMagpieService.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieService.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieService.ForceCheckInBackground("alternateUrl");
            _mockMagpieService._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
        }

        [TestMethod]
        public void TestBackgroundCheckOverridingAppCastUrl()
        {
            _mockMagpieService.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieService.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieService.CheckInBackground("alternateUrl");
            _mockMagpieService._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
        }
    }

    internal static class AssemblyInjector
    {
        /// <summary>
        /// Allows setting the Entry Assembly when needed. 
        /// Use AssemblyUtilities.SetEntryAssembly() as first line in XNA ad hoc tests
        /// </summary>
        /// <param name="assembly">Assembly to set as entry assembly</param>
        public static void Inject(Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            var manager = new AppDomainManager();
            var entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            if (entryAssemblyfield != null) entryAssemblyfield.SetValue(manager, assembly);

            var domain = AppDomain.CurrentDomain;
            var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            if (domainManagerField != null) domainManagerField.SetValue(domain, manager);
        }
    }
}
