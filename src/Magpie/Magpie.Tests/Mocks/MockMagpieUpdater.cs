using System.Threading.Tasks;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Services;
using NSubstitute;

namespace Magpie.Tests.Mocks
{
    internal class MockMagpieUpdater : MagpieUpdater
    {
        private const string VALID_JSON = @"{ 'title': 'Magpie', 'version': '0.0.1', 'build_date': '10/03/2015', 'release_notes_url': '', 'artifact_url': 'https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.zip' }";
        internal RemoteAppcast RemoteAppcast { get; private set; }
        internal bool _showUpdateWindowFlag;
        internal bool _showNoUpdatesWindowFlag;
        internal IRemoteContentDownloader _remoteContentDownloader;

        public MockMagpieUpdater(string validUrl, IDebuggingInfoLogger infoLogger = null) : base(new AppInfo(validUrl), infoLogger)
        {
            var validJson = VALID_JSON.Replace("'", "\"");
            _remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            _remoteContentDownloader.DownloadStringContent(validUrl).Returns(Task.FromResult(validJson));
            base.RemoteContentDownloader = _remoteContentDownloader;
        }

        protected override void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            RemoteAppcast = args.Payload;
            base.OnRemoteAppcastAvailableEvent(args);
        }

        protected override void ShowUpdateWindow(RemoteAppcast appcast)
        {
            // can't do in tests
            _showUpdateWindowFlag = true;
        }

        protected override void ShowNoUpdatesWindow()
        {
            // can't do in tests
            _showNoUpdatesWindowFlag = true;
        }
    }
}