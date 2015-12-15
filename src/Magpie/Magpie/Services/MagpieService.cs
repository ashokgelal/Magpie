using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.ViewModels;
using Magpie.Views;

namespace Magpie.Services
{
    public class MagpieService : IMagpieService
    {
        private readonly AppInfo _appInfo;
        private readonly IDebuggingInfoLogger _logger;
        private IAnalyticsLogger _analyticsLogger;
        internal UpdateDecider UpdateDecider { get; set; }
        internal IRemoteContentDownloader RemoteContentDownloader { get; set; }
        public event EventHandler<SingleEventArgs<RemoteAppcast>> RemoteAppcastAvailableEvent;
        public event EventHandler<SingleEventArgs<string>> ArtifactDownloadedEvent;

        public MagpieService(AppInfo appInfo, IDebuggingInfoLogger debuggingInfoLogger = null, IAnalyticsLogger analyticsLogger = null)
        {
            _appInfo = appInfo;
            _logger = debuggingInfoLogger ?? new DebuggingWindowViewModel();
            _analyticsLogger = analyticsLogger ?? new AnalyticsLogger();
            RemoteContentDownloader = new DefaultRemoteContentDownloader();
            UpdateDecider = new UpdateDecider(_logger);
        }

        public async void CheckInBackground(string appcastUrl, bool showDebuggingWindow = false)
        {
            await Check(appcastUrl, showDebuggingWindow).ConfigureAwait(false);
        }

        public async void ForceCheckInBackground(string appcastUrl, bool showDebuggingWindow = false)
        {
            await Check(appcastUrl, showDebuggingWindow, true).ConfigureAwait(false);
        }

        private async Task Check(string appcastUrl, bool showDebuggingWindow = false, bool forceCheck = false)
        {
            _logger.Log(string.Format("Starting fetching remote appcast content from address: {0}", appcastUrl));
            try
            {
                var data = await RemoteContentDownloader.DownloadStringContent(appcastUrl).ConfigureAwait(true);
                var appcast = ParseAppcast(data);
                OnRemoteAppcastAvailableEvent(new SingleEventArgs<RemoteAppcast>(appcast));
                if (UpdateDecider.ShouldUpdate(appcast, forceCheck))
                {
                    ShowUpdateWindow(appcast);
                }
                else if (forceCheck)
                {
                    ShowNoUpdatesWindow();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(string.Format("Error parsing remote appcast: {0}", ex.Message));
            }
            finally
            {
                _logger.Log("Finished fetching remote appcast content");
            }
        }

        protected virtual async void ShowUpdateWindow(RemoteAppcast appcast)
        {
            var viewModel = new MainWindowViewModel(_appInfo, _logger, RemoteContentDownloader, _analyticsLogger);
            await viewModel.StartAsync(appcast).ConfigureAwait(true);
            var window = new MainWindow { ViewModel = viewModel };
            viewModel.DownloadNowCommand = new DelegateCommand(e =>
            {
                _analyticsLogger.LogDownloadNow();
                _logger.Log("Continuing with downloading the artifact");
                window.Close();
                ShowDownloadWindow(appcast);
            });
            SetOwner(window);
            window.ShowDialog();
        }

        protected virtual void ShowNoUpdatesWindow()
        {
            var window = new NoUpdatesWindow();
            SetOwner(window);
            window.ShowDialog();
        }

        private static string CreateTempPath(string url)
        {
            var uri = new Uri(url);
            var path = Path.GetTempPath();
            var fileName = string.Format(Guid.NewGuid() + Path.GetFileName(uri.LocalPath));
            return Path.Combine(path, fileName);
        }

        protected virtual void ShowDownloadWindow(RemoteAppcast appcast)
        {
            var viewModel = new DownloadWindowViewModel(_appInfo, _logger, RemoteContentDownloader);
            var destinationPath = CreateTempPath(appcast.ArtifactUrl);
            var window = new DownloadWindow { DataContext = viewModel };
            viewModel.ContinueWithInstallationCommand = new DelegateCommand(e =>
            {
                _logger.Log("Continue after downloading artifact");
                _analyticsLogger.LogContinueWithInstallation();
                OnArtifactDownloadedEvent(new SingleEventArgs<string>(destinationPath));
                window.Close();
                OpenArtifact(destinationPath);
            });
            SetOwner(window);
            viewModel.StartAsync(appcast, destinationPath);
            window.ShowDialog();
        }

        protected virtual void OpenArtifact(string artifactPath)
        {
            Process.Start(artifactPath);
        }

        protected virtual void SetOwner(Window window)
        {
            if (Application.Current != null && !Application.Current.MainWindow.Equals(window))
            {
                window.Owner = Application.Current.MainWindow;
            }
        }

        private RemoteAppcast ParseAppcast(string content)
        {
            _logger.Log("Started deserializing remote appcast content");
            var serializer = new DataContractJsonSerializer(typeof(RemoteAppcast));
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(content));
            var appcast = (RemoteAppcast)serializer.ReadObject(ms);
            ms.Close();
            _logger.Log("Finished deserializing remote appcast content");
            return appcast;
        }

        protected virtual void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            var handler = RemoteAppcastAvailableEvent;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnArtifactDownloadedEvent(SingleEventArgs<string> args)
        {
            var handler = ArtifactDownloadedEvent;
            if (handler != null) handler(this, args);
        }
    }
}