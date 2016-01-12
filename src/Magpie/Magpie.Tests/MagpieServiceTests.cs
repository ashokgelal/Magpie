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
            Assert.AreEqual("Magpie", appcast.Title);
            Assert.AreEqual(new Version(0, 0, 1), appcast.Version);
            Assert.AreEqual(5, appcast.RawDictionary.Count);
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
            updateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(false);
            _mockMagpieUpdater.UpdateDecider = updateDecider;
            _mockMagpieUpdater.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieUpdater._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestNoUpdatesWindowNotShownOnNormalCheck()
        {
            var updateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            updateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>()).Returns(false);
            _mockMagpieUpdater.UpdateDecider = updateDecider;
            _mockMagpieUpdater.CheckInBackground();
            Assert.IsFalse(_mockMagpieUpdater._showNoUpdatesWindowFlag);
        }

        [TestMethod]
        public void TestUpdateWindowShown()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieUpdater.ForceCheckInBackground();
            Assert.IsTrue(_mockMagpieUpdater._showUpdateWindowFlag);
        }

        [TestMethod]
        public void TestForceCheckOverridingAppCastUrl()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieUpdater.ForceCheckInBackground("alternateUrl");
            _mockMagpieUpdater._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
        }

        [TestMethod]
        public void TestBackgroundCheckOverridingAppCastUrl()
        {
            _mockMagpieUpdater.UpdateDecider = Substitute.For<UpdateDecider>(new DebuggingWindowViewModel());
            _mockMagpieUpdater.UpdateDecider.ShouldUpdate(Arg.Any<RemoteAppcast>(), true).Returns(true);
            _mockMagpieUpdater.CheckInBackground("alternateUrl");
            _mockMagpieUpdater._remoteContentDownloader.Received(1).DownloadStringContent("alternateUrl");
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
