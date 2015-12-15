using System;
using System.Threading.Tasks;
using Magpie.Interfaces;
using Magpie.Services;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private MockMainWindowViewModel _mainWindowViewModel;
        private MockRemoteAppcast _appCast;
        private IAnalyticsLogger _analyticsLogger;

        [TestInitialize]
        public void Initialize()
        {
            var debuggingInfoLogger = Substitute.For<IDebuggingInfoLogger>();
            _analyticsLogger = Substitute.For<IAnalyticsLogger>();
            var remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            var appInfo = new AppInfo();
            _appCast = new MockRemoteAppcast(new Version(1, 0));
            _mainWindowViewModel = new MockMainWindowViewModel(appInfo, debuggingInfoLogger, remoteContentDownloader, _analyticsLogger);
        }

        [TestMethod]
        public async Task TestReleaseNotesGetsDefaultCssFile()
        {
            _mainWindowViewModel.Stylesheet = "body{color:red;}";
            await _mainWindowViewModel.StartAsync(_appCast);

            Assert.IsTrue(_mainWindowViewModel.ReleaseNotes.Contains("<style>body{color:red;}</style>"));
        }

        [TestMethod]
        public async Task TestReleaseNotesDoNotGetStyledIfCannotFindDefaultCssFile()
        {
            _mainWindowViewModel.Stylesheet = string.Empty;
            await _mainWindowViewModel.StartAsync(_appCast);

            Assert.IsFalse(_mainWindowViewModel.ReleaseNotes.Contains("<style>"));
        }

        [TestMethod]
        public void TestContinueUpdateCommandLogsAnalytics()
        {
            _mainWindowViewModel.ContinueUpdateCommand.Execute(null);
            _analyticsLogger.Received().LogContinueUpdate();
        }

        [TestMethod]
        public void TestSkipThisVersionCommandLogsAnalytics()
        {
            _mainWindowViewModel.SkipThisVersionCommand.Execute(null);
            _analyticsLogger.Received().LogSkipThisVersion();
        }

        [TestMethod]
        public void TestRemindMeLaterCommandLogsAnalytics()
        {
            _mainWindowViewModel.RemindMeLaterCommand.Execute(null);
            _analyticsLogger.Received().LogRemindMeLater();
        }

        [TestMethod]
        public void TestLogOldVersion()
        {
            _mainWindowViewModel.OldVersion = "1.0.0";
            _analyticsLogger.Received().LogOldVersion("1.0.0");
        }

        [TestMethod]
        public void TestLogNewVersion()
        {
            _mainWindowViewModel.NewVersion = "1.0.1";
            _analyticsLogger.Received().LogNewVersion("1.0.1");
        }

        [TestMethod]
        public void TestLogAppTitle()
        {
            _mainWindowViewModel.Title = "My Super Awesome App";
            _analyticsLogger.Received().LogAppTitle("My Super Awesome App");
        }

        [TestMethod]
        public void TestLogUpdateWindowClosed()
        {
            _mainWindowViewModel.CancelCommand.Execute(null);
            _analyticsLogger.Received().LogUpdateCancelled();
        }

    }
}
