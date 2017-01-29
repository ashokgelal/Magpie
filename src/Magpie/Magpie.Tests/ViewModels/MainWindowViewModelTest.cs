using System.Threading.Tasks;
using Magpie.Tests.Mocks;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.ViewModels
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private MockMainWindowViewModel _mainWindowViewModel;
        private MockChannel _channel;
        private IAnalyticsLogger _analyticsLogger;

        [TestInitialize]
        public void Initialize()
        {
            var debuggingInfoLogger = Substitute.For<IDebuggingInfoLogger>();
            _analyticsLogger = Substitute.For<IAnalyticsLogger>();
            var remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            var appInfo = new AppInfo("valid_url");
            _channel = new MockChannel(1, "1.0");
            AssemblyInjector.Inject();
            _mainWindowViewModel = new MockMainWindowViewModel(appInfo, debuggingInfoLogger, remoteContentDownloader,
                _analyticsLogger);
        }

        [TestMethod]
        public async Task TestReleaseNotesGetsDefaultCssFile()
        {
            _mainWindowViewModel.Stylesheet = "body{color:red;}";
            await _mainWindowViewModel.StartAsync(_channel);

            Assert.IsTrue(_mainWindowViewModel.ReleaseNotes.Contains("<style>body{color:red;}</style>"));
        }

        [TestMethod]
        public async Task TestReleaseNotesDoNotGetStyledIfCannotFindDefaultCssFile()
        {
            _mainWindowViewModel.Stylesheet = string.Empty;
            await _mainWindowViewModel.StartAsync(_channel);

            Assert.IsFalse(_mainWindowViewModel.ReleaseNotes.Contains("<style>"));
        }

        [TestMethod]
        public void TestDownloadNowCommandLogsAnalytics()
        {
            _mainWindowViewModel.DownloadNowCommand.Execute(null);
            _analyticsLogger.Received().LogDownloadNow();
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
        public async Task TestTitleIsProperlySet()
        {
            await _mainWindowViewModel.StartAsync(_channel);
            Assert.AreEqual("A NEW VERSION OF MAGPIE.TESTS IS AVAILABLE", _mainWindowViewModel.Title);
        }

        [TestMethod]
        public void TestLogUpdateCancelled()
        {
            _mainWindowViewModel.CancelUpdate();
            _analyticsLogger.Received().LogUpdateCancelled();
        }
    }
}