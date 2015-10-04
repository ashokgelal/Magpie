using System;
using System.Net;
using System.Threading.Tasks;
using Magpie.Interfaces;

namespace Magpie.Services
{
    internal class DefaultRemoteContentDownloader : IRemoteContentDownloader
    {
        public async Task<string> DownloadStringContent(string url)
        {
            var client = new WebClient();
            return await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);
        }
    }
}