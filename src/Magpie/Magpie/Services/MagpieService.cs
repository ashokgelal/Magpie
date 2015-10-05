using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
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
        internal UpdateDecider UpdateDecider { get; set; }
        internal IRemoteContentDownloader RemoteContentDownloader { get; set; }
        public event EventHandler<SingleEventArgs<RemoteAppcast>> RemoteAppcastAvailableEvent;

        public MagpieService(AppInfo appInfo, IDebuggingInfoLogger debuggingInfoLogger = null)
        {
            _appInfo = appInfo;
            _logger = debuggingInfoLogger ?? new DebuggingWindowViewModel();
            RemoteContentDownloader = new DefaultRemoteContentDownloader();
            UpdateDecider = new UpdateDecider(_logger);
        }

        public async void RunInBackground(string appcastUrl, bool showDebuggingWindow = false)
        {
            _logger.Log(string.Format("Starting fetching remote appcast content from address: {0}", appcastUrl));
            try
            {
                var data = await RemoteContentDownloader.DownloadStringContent(appcastUrl).ConfigureAwait(true);
                var appcast = ParseAppcast(data);
                OnRemoteAppcastAvailableEvent(new SingleEventArgs<RemoteAppcast>(appcast));
                if (UpdateDecider.ShouldUpdate(appcast))
                {
                    ShowUpdateWindow(appcast);
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
            var viewModel = new MainWindowViewModel(appcast, _appInfo, _logger);
            await viewModel.InitializeAsync().ConfigureAwait(true);
            var window = new MainWindow { DataContext = viewModel };
            SetOwner(window);
            window.ShowDialog();
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
    }
}