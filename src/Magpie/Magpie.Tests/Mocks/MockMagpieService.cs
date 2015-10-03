using System.Threading.Tasks;
using Magpie.Interfaces;

namespace Magpie.Tests.Mocks
{
    internal class MockMagpieService : MagpieService
    {
        private readonly string _appcastContent;
        internal RemoteAppcast RemoteAppcast { get; private set; }

        public MockMagpieService(string appcastContent, IDebuggingInfoLogger infoLogger = null) : base(infoLogger)
        {
            _appcastContent = appcastContent;
        }

        protected override Task<string> FetchRemoteAppcastContent(string appcastUrl)
        {
            return Task.FromResult(_appcastContent);
        }

        protected override void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            RemoteAppcast = args.Payload;
            base.OnRemoteAppcastAvailableEvent(args);
        }
    }
}