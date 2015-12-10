using System.Net;
using System.Threading.Tasks;

namespace Magpie.Interfaces
{
    public interface IRemoteContentDownloader
    {
        /// <summary>
        /// Retreives the Appcast string from the designated URL
        /// </summary>
        /// <param name="url">URL for Appcast file</param>
        /// <returns>The contents of the Appcast file</returns>
        Task<string> DownloadStringContent(string url);

        /// <summary>
        /// Downloads the file pointed to by <see cref="sourceUrl"/> and stores it at the <see cref="destinationPath"/>
        /// using the <see cref="client"/>. The <see cref="destinationPath"/> points to a Temp directory by default.
        /// </summary>
        /// <param name="sourceUrl">URL for file to download</param>
        /// <param name="destinationPath">Destination for downloaded file.</param>
        /// <param name="client">A WebClient for controlling the download</param>
        Task<string> DownloadFile(string sourceUrl, string destinationPath, WebClient client);
    }
}