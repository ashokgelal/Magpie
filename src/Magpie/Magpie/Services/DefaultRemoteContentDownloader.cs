using System;
using System.Net;
using System.Threading.Tasks;
using MagpieUpdater.Interfaces;

namespace MagpieUpdater.Services
{
    internal class DefaultRemoteContentDownloader : IRemoteContentDownloader
    {
        public async Task<string> DownloadStringContent(string url, IDebuggingInfoLogger logger = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    return await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Log(e.Message);
                }
                return string.Empty;
            }
        }

        public async Task<string> DownloadFile(string sourceUrl, string destinationPath, Action<int> onProgressChanged, IDebuggingInfoLogger logger = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) => onProgressChanged(e.ProgressPercentage);
                    var uri = new Uri(sourceUrl);
                    await client.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
                    return destinationPath;
                }
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Log(e.Message);
                }
                return string.Empty;
            }
        }
    }
}