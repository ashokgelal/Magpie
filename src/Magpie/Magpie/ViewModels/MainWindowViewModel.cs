using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Properties;
using Magpie.Services;

namespace Magpie.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IDebuggingInfoLogger _logger;
        private readonly IRemoteContentDownloader _contentDownloader;
        private string _releaseNotes;
        private string _title;
        private string _oldVersion;
        private string _newVersion;
        private string _appIconPath;
        private string _remoteVersion;

        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set
            {
                SetProperty(ref _releaseNotes, value);
            }
        }

        public ICommand ContinueUpdateCommand { get; set; }
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

        public string AppIconPath
        {
            get { return _appIconPath; }
            set { SetProperty(ref _appIconPath, value); }
        }

        public MainWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger, IRemoteContentDownloader contentDownloader)
        {
            AppIconPath = appInfo.AppIconPath;
            _logger = logger;
            _contentDownloader = contentDownloader;
        }

        internal async Task StartAsync(RemoteAppcast appcast)
        {
            InitializeCommands(appcast);
            Title = string.Format("A NEW VERSION OF {0} IS AVAILABLE", appcast.Title).ToUpperInvariant();
            OldVersion = GetOldVersion();
            NewVersion = appcast.Version.ToString();
            ReleaseNotes = await FetchReleaseNotesAsync(appcast.ReleaseNotesUrl).ConfigureAwait(false);
        }

        protected virtual string GetOldVersion()
        {
            return new AssemblyAccessor().Version;
        }

        private void InitializeCommands(RemoteAppcast appcast)
        {
            _remoteVersion = appcast.Version.ToString();
            SkipThisVersionCommand = new DelegateCommand(SkipThisVersionCommandHandler);
            RemindMeLaterCommand = new DelegateCommand(RemindMeLaterCommandHandler);
        }

        private void RemindMeLaterCommandHandler(object obj)
        {
            _logger.Log("Remind me later command invoked");
            var registryIO = new RegistryIO();
            registryIO.WriteToRegistry(MagicStrings.LAST_CHECK_DATE, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        private void SkipThisVersionCommandHandler(object obj)
        {
            _logger.Log("Skip this version command invoked");
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
            return string.IsNullOrWhiteSpace(stylesheet) ? string.Empty : string.Format("<style>{0}</style>", stylesheet);
        }

        protected virtual string GetStylesheet()
        {
            var stylesheetStream = Resources.ResourceManager.GetStream("style");
            if (stylesheetStream == null) return String.Empty;
            var stylesheetReader = new StreamReader(stylesheetStream);
            return stylesheetReader.ReadToEnd();
        }
    }
}