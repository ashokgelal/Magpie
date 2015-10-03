using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.ViewModels;

namespace Magpie.Services
{
    public class MagpieService : IMagpieService
    {
        private readonly IDebuggingInfoLogger _logger;
        public event EventHandler<SingleEventArgs<RemoteAppcast>> RemoteAppcastAvailableEvent;

        public MagpieService(IDebuggingInfoLogger debuggingInfoLogger = null)
        {
            _logger = debuggingInfoLogger ?? new DebuggingWindowViewModel();
        }

        public async void RunInBackground(string appcastUrl, bool showDebuggingWindow = false)
        {
            _logger.Log(string.Format("Starting fetching remote appcast content from address: {0}", appcastUrl));
            try
            {
                var data = await FetchRemoteAppcastContent(appcastUrl).ConfigureAwait(false);
                _logger.Log(data);
                try
                {
                    var appcast = ParseAppcast(data);
                    OnRemoteAppcastAvailableEvent(new SingleEventArgs<RemoteAppcast>(appcast));
                }
                catch (Exception ex)
                {
                    _logger.Log(string.Format("Error parsing remote appcast: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
            }
            finally
            {
                _logger.Log("Finished fetching remote appcast content");
            }
        }

        protected virtual async Task<string> FetchRemoteAppcastContent(string appcastUrl)
        {
            var client = new WebClient();
            return await client.DownloadStringTaskAsync(new Uri(appcastUrl)).ConfigureAwait(false);
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