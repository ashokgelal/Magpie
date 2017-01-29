using MagpieUpdater.Interfaces;
using MagpieUpdater.Services;
using MagpieUpdater.ViewModels;

namespace Magpie.Tests.Mocks
{
    internal class MockMainWindowViewModel : MainWindowViewModel
    {
        public string Stylesheet { get; set; }

        public MockMainWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger,
            IRemoteContentDownloader contentDownloader, IAnalyticsLogger analyticsLogger)
            : base(appInfo, logger, contentDownloader, analyticsLogger)
        {
            DownloadNowCommand = new DelegateCommand(message => analyticsLogger.LogDownloadNow());
            SkipThisVersionCommand = new DelegateCommand(message => analyticsLogger.LogSkipThisVersion());
            RemindMeLaterCommand = new DelegateCommand(message => analyticsLogger.LogRemindMeLater());
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