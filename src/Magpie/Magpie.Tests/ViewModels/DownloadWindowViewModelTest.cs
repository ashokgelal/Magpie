using System;
using System.Threading.Tasks;
using Magpie.Tests.Mocks;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Services;
using MagpieUpdater.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.ViewModels
{
    [TestClass]
    public class DownloadWindowViewModelTest
    {
        private DownloadWindowViewModel _sut;
        private IRemoteContentDownloader _remoteContentDownloader;

        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
            var appInfo = new AppInfo("test_url") {AppIconPath = "icon_path"};
            _remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            _sut = new DownloadWindowViewModel(appInfo, Substitute.For<IDebuggingInfoLogger>(), _remoteContentDownloader);
        }

        [TestMethod]
        public async Task TitleIsSetProperly()
        {
            var mockChannel = new MockChannel(1, "1.0");
            mockChannel.SetArtifactUrl("artifact_url");
            await _sut.StartAsync(mockChannel, "dest_path");
            Assert.AreEqual("Downloading Magpie.Tests Updates...", _sut.Title);
        }

        [TestMethod]
        public async Task StartAsyncStartsDownloadingFile()
        {
            var mockChannel = new MockChannel(1, "1.0");
            mockChannel.SetArtifactUrl("artifact_url");
            _remoteContentDownloader.DownloadFile("artifact_url", "dest_path", Arg.Any<Action<int>>())
                .Returns("saved_dest_path");

            var savedAt = await _sut.StartAsync(mockChannel, "dest_path");
            await _remoteContentDownloader.Received().DownloadFile("artifact_url", "dest_path", Arg.Any<Action<int>>());
            Assert.AreEqual("saved_dest_path", savedAt);
        }

        [TestMethod]
        public async Task UpdatesProgressWhenDownloading()
        {
            var mockChannel = new MockChannel(1, "1.0");
            mockChannel.SetArtifactUrl("artifact_url");
            _remoteContentDownloader.DownloadFile("artifact_url", "dest_path", Arg.Do<Action<int>>(a =>
                {
                    a(50);
                    a(75);
                }))
                .Returns("saved_dest_path");

            await _sut.StartAsync(mockChannel, "dest_path");
            Assert.AreEqual(75, _sut.ProgressPercent);
        }
    }
}