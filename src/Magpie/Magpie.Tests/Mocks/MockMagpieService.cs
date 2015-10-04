using System.Threading.Tasks;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Services;
using NSubstitute;

namespace Magpie.Tests.Mocks
{
    internal class MockMagpieService : MagpieService
    {
        private const string VALID_JSON = @"{ 'title': 'Magpie', 'version': '0.0.1', 'build_date': '10/03/2015', 'release_notes_url': '', 'artifact_url': 'https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.zip' }";
        internal RemoteAppcast RemoteAppcast { get; private set; }

        public MockMagpieService(string validUrl, IDebuggingInfoLogger infoLogger = null) : base(infoLogger)
        {
            var validJson = VALID_JSON.Replace("'", "\"");
            var downloader = Substitute.For<IRemoteContentDownloader>();
            downloader.DownloadStringContent(validUrl).Returns(Task.FromResult(validJson));
            base.RemoteContentDownloader = downloader;
        }

        protected override void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            RemoteAppcast = args.Payload;
            base.OnRemoteAppcastAvailableEvent(args);
        }
    }
}