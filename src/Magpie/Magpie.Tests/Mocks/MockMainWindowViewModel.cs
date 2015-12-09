using Magpie.Interfaces;
using Magpie.Services;
using Magpie.ViewModels;

namespace Magpie.Tests.Mocks
{
    internal class MockMainWindowViewModel :MainWindowViewModel
    {
        public MockMainWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger, IRemoteContentDownloader contentDownloader) : base(appInfo, logger, contentDownloader)
        {
        }
        
        protected override string GetOldVersion()
        {
            return "1.0";
        }
    }
}
