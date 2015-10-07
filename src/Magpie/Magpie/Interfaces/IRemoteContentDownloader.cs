using System.Net;
using System.Threading.Tasks;

namespace Magpie.Interfaces
{
    public interface IRemoteContentDownloader
    {
        Task<string> DownloadStringContent(string url);
        Task<string> DownloadFile(string sourceUrl, string destinationPath, WebClient client);
    }
}