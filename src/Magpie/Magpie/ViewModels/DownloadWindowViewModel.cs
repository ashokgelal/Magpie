using System.Threading.Tasks;
using System.Windows.Input;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.Services;

namespace MagpieUpdater.ViewModels
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

        internal DownloadWindowViewModel(AppInfo appInfo, IDebuggingInfoLogger logger,
            IRemoteContentDownloader contentDownloader)
        {
            AppIconPath = appInfo.AppIconPath;
            _logger = logger;
            _contentDownloader = contentDownloader;
        }

        internal async Task<string> StartAsync(Channel channel, string destinationPath)
        {
            Title = string.Format(Properties.Resources.DownloadingInstaller, MainAssembly.ProductName);
            return await DownloadArtifact(channel, destinationPath).ConfigureAwait(true);
        }

        private async Task<string> DownloadArtifact(Channel channel, string destinationPath)
        {
            _logger.Log("Starting to download artifact");
            var savedAt =
                await _contentDownloader.DownloadFile(channel.ArtifactUrl, destinationPath, p => ProgressPercent = p, _logger)
                    .ConfigureAwait(false);
            _logger.Log(string.Format("Artifact downloaded to {0}", destinationPath));
            return savedAt;
        }
    }
}