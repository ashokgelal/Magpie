using System.Threading.Tasks;

namespace Magpie.Interfaces
{
    public interface IRemoteContentDownloader
    {
        Task<string> DownloadStringContent(string url);
    }
}