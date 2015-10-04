using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.ViewModels;
using Magpie.Views;

namespace Magpie.Services
{
    public class MagpieService : IMagpieService
    {
        private readonly IDebuggingInfoLogger _logger;
        public event EventHandler<SingleEventArgs<RemoteAppcast>> RemoteAppcastAvailableEvent;
        protected IRemoteContentDownloader RemoteContentDownloader { get; set; }

        public MagpieService(IDebuggingInfoLogger debuggingInfoLogger = null)
        {
            _logger = debuggingInfoLogger ?? new DebuggingWindowViewModel();
            RemoteContentDownloader = new DefaultRemoteContentDownloader();
        }

        public async void RunInBackground(string appcastUrl, bool showDebuggingWindow = false)
        {
            _logger.Log(string.Format("Starting fetching remote appcast content from address: {0}", appcastUrl));
            try
            {
                var data = await RemoteContentDownloader.DownloadStringContent(appcastUrl).ConfigureAwait(true);
                var appcast = ParseAppcast(data);
                OnRemoteAppcastAvailableEvent(new SingleEventArgs<RemoteAppcast>(appcast));
                if (CanUpdate(appcast))
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

        protected virtual void ShowUpdateWindow(RemoteAppcast appcast)
        {
            var viewModel = new MainWindowViewModel(appcast, _logger);
            var window = new MainWindow { DataContext = viewModel };
            // todo: set owner
            window.ShowDialog();
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

        protected virtual bool CanUpdate(RemoteAppcast appcast)
        {
            // todo: check registry
            var curVer = Assembly.GetExecutingAssembly().GetName().Version;
            var remVer = appcast.Version;

            var isHigherVersionAvailable = remVer.Revision > curVer.Revision
                   || remVer.Build > curVer.Build
                   || remVer.Minor > curVer.Minor
                   || remVer.Major > curVer.Major;
            _logger.Log(string.Format("Higher version of app is {0}available", isHigherVersionAvailable ? "" : "not "));
            return isHigherVersionAvailable;
        }

        protected virtual void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            var handler = RemoteAppcastAvailableEvent;
            if (handler != null) handler(this, args);
        }
    }
}