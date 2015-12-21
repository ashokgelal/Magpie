using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Services;

namespace Magpie.ViewModels
{
    internal class DownloadWindowViewModel : BindableBase
    {
        private readonly IDebuggingInfoLogger _logger;
        private readonly IRemoteContentDownloader _contentDownloader;
        private int _progressPercent;
        private string _appIconPath;
        private string _title;
        public ICommand ContinueWithInstallationCommand { get; set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public int ProgressPercent
        {
            get { return _progressPercent; }
            set { SetProperty(ref _progressPercent, value); }
        }
        public string AppIconPath
        {
            get { return _appIconPath; }
            set { SetProperty(ref _appIconPath, value); }
        }

        internal DownloadWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger, IRemoteContentDownloader contentDownloader)
        {
            AppIconPath = appInfo.AppIconPath;
            _logger = logger;
            _contentDownloader = contentDownloader;
        }

        internal async void StartAsync(RemoteAppcast appcast, string destinationPath)
        {
            Title = string.Format(Properties.Resources.DownloadingInstaller, appcast.Title);
            await DownloadArtifact(appcast, destinationPath).ConfigureAwait(true);
        }

        private async Task DownloadArtifact(RemoteAppcast appcast, string destinationPath)
        {
            _logger.Log("Starting to download artifact");
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, args) => { ProgressPercent = args.ProgressPercentage; };
                await _contentDownloader.DownloadFile(appcast.ArtifactUrl, destinationPath, client).ConfigureAwait(false);
                _logger.Log(string.Format("Artifact downloaded to {0}", destinationPath));
            }
        }

    }
}