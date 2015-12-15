using Magpie.Interfaces;
using Magpie.Services;
using Magpie.ViewModels;

namespace Magpie.Tests.Mocks
{
    internal class MockMainWindowViewModel :MainWindowViewModel
    {
        public string Stylesheet { get; set; }

        public MockMainWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger, IRemoteContentDownloader contentDownloader, IAnalyticsLogger analyticsLogger) : base(appInfo, logger, contentDownloader, analyticsLogger)
        {
            ContinueUpdateCommand = new DelegateCommand(message=> analyticsLogger.LogContinueUpdate());
        }
        
        protected override string GetOldVersion()
        {
            return "1.0";
        }

        protected override string GetStylesheet()
        {
            return Stylesheet;
        }
    }
}
