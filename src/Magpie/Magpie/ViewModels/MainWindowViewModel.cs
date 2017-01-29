using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.Properties;
using MagpieUpdater.Services;

namespace MagpieUpdater.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IDebuggingInfoLogger _logger;
        private readonly IAnalyticsLogger _analyticsLogger;
        private readonly IRemoteContentDownloader _contentDownloader;
        private string _releaseNotes;
        private string _title;
        private string _oldVersion;
        private string _newVersion;
        private string _build;
        private string _appIconPath;
        private string _remoteVersion;

        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set { SetProperty(ref _releaseNotes, value); }
        }

        public ICommand DownloadNowCommand { get; set; }
        public ICommand SkipThisVersionCommand { get; set; }
        public ICommand RemindMeLaterCommand { get; set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string OldVersion
        {
            get { return _oldVersion; }
            set { SetProperty(ref _oldVersion, value); }
        }

        public string NewVersion
        {
            get { return _newVersion; }
            set { SetProperty(ref _newVersion, value); }
        }

        public string Build
        {
            get { return _build; }
            set { SetProperty(ref _build, value); }
        }

        public string AppIconPath
        {
            get { return _appIconPath; }
            set { SetProperty(ref _appIconPath, value); }
        }

        public MainWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger,
            IRemoteContentDownloader contentDownloader, IAnalyticsLogger analyticsLogger)
        {
            AppIconPath = appInfo.AppIconPath;
            _logger = logger;
            _contentDownloader = contentDownloader;
            _analyticsLogger = analyticsLogger;
        }

        internal async Task StartAsync(Channel channel)
        {
            InitializeCommands(channel);
            Title = string.Format(Resources.NewVersionAvailable, MainAssembly.ProductName).ToUpperInvariant();
            Build = channel.Build;
            OldVersion = GetOldVersion();
            NewVersion = channel.Version.ToString();
            ReleaseNotes = await FetchReleaseNotesAsync(channel.ReleaseNotesUrl).ConfigureAwait(false);
        }

        protected virtual string GetOldVersion()
        {
            return new AssemblyAccessor().Version;
        }

        private void InitializeCommands(Channel channel)
        {
            _remoteVersion = channel.Version.ToString();
            SkipThisVersionCommand = new DelegateCommand(SkipThisVersionCommandHandler);
            RemindMeLaterCommand = new DelegateCommand(RemindMeLaterCommandHandler);
        }

        private void RemindMeLaterCommandHandler(object obj)
        {
            _logger.Log("Remind me later command invoked");
            _analyticsLogger.LogRemindMeLater();
            var registryIO = new RegistryIO();
            registryIO.WriteToRegistry(MagicStrings.LAST_CHECK_DATE, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        private void SkipThisVersionCommandHandler(object obj)
        {
            _logger.Log("Skip this version command invoked");
            _analyticsLogger.LogSkipThisVersion();
            var registryIO = new RegistryIO();
            registryIO.WriteToRegistry(MagicStrings.SKIP_VERSION_KEY, _remoteVersion);
        }

        private async Task<string> FetchReleaseNotesAsync(string releaseNotesUrl)
        {
            _logger.Log("Fetching release notes");
            var notes = await _contentDownloader.DownloadStringContent(releaseNotesUrl).ConfigureAwait(false);
            _logger.Log("Finished fetching release notes");
            _logger.Log("Converting release notes from markdown to html");
            var htmlNotes = CommonMark.CommonMarkConverter.Convert(notes);
            htmlNotes = CreateDefaultCssLink() + htmlNotes;
            _logger.Log("Finished converting release notes from markdown to html");
            return htmlNotes;
        }

        private string CreateDefaultCssLink()
        {
            var stylesheet = GetStylesheet();
            return string.IsNullOrWhiteSpace(stylesheet)
                ? string.Empty
                : string.Format("<style>{0}</style>", stylesheet);
        }

        protected virtual string GetStylesheet()
        {
            var stylesheetStream = Resources.ResourceManager.GetObject("style") as string;
            return stylesheetStream ?? string.Empty;
        }

        public void CancelUpdate()
        {
            _analyticsLogger.LogUpdateCancelled();
        }
    }
}