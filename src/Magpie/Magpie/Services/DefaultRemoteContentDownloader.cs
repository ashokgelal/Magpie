using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Magpie.Interfaces;

namespace Magpie.Services
{
    internal class DefaultRemoteContentDownloader : IRemoteContentDownloader
    {
        public async Task<string> DownloadStringContent(string url)
        {
            using (var client = new WebClient())
            {
                return await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);
            }
        }

        public async Task<string> DownloadFile(string sourceUrl, string destinationPath, WebClient client)
        {
            var uri = new Uri(sourceUrl);
            await client.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
            return destinationPath;
        }


    }
}