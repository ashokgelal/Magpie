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

        [TestInitialize]
        public void Initialize()
        {
            var logger = Substitute.For<IDebuggingInfoLogger>();
            var remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            var appInfo = new AppInfo();
            _appCast = new MockRemoteAppcast(new Version(1, 0));
            _mainWindowViewModel = new MockMainWindowViewModel(appInfo, logger, remoteContentDownloader);
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
    }
}
