using System;
using System.Threading.Tasks;

namespace MagpieUpdater.Interfaces
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
        /// Downloads the file pointed to by <see cref="sourceUrl"/> and stores it at the <see cref="destinationPath"/>.
        /// The <see cref="destinationPath"/> points to a Temp directory by default.
        /// </summary>
        /// <param name="sourceUrl">URL for file to download</param>
        /// <param name="destinationPath">Destination for downloaded file.</param>
        /// <param name="onProgressChanged">A callback for updating progress when the file is being downloaded.</param>
        Task<string> DownloadFile(string sourceUrl, string destinationPath, Action<int> onProgressChanged);
    }
}