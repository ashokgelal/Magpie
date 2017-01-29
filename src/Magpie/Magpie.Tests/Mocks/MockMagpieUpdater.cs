using System.Threading.Tasks;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Services;
using Magpie.Tests.Models;
using NSubstitute;

namespace Magpie.Tests.Mocks
{
    internal class MockMagpieUpdater : Magpie.Services.MagpieUpdater
    {
        private readonly string VALID_JSON =
            @"{'foo': 'bar', 'channels': [{ 'id': 2, 'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'}]}"
                .MakeJson();

        internal RemoteAppcast RemoteAppcast { get; private set; }
        internal bool _showUpdateWindowFlag;
        internal bool _showNoUpdatesWindowFlag;
        internal IRemoteContentDownloader _remoteContentDownloader;

        public MockMagpieUpdater(string validUrl, IDebuggingInfoLogger infoLogger = null)
            : base(new AppInfo(validUrl), infoLogger)
        {
            _remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            _remoteContentDownloader.DownloadStringContent(validUrl).Returns(Task.FromResult(VALID_JSON));
            base.RemoteContentDownloader = _remoteContentDownloader;
        }

        protected override void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            RemoteAppcast = args.Payload;
            base.OnRemoteAppcastAvailableEvent(args);
        }

        protected override void ShowUpdateWindow(Channel channel)
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