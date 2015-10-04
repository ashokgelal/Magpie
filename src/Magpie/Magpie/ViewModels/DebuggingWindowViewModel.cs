using System;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using Magpie.Interfaces;
using Magpie.Models;

namespace Magpie.ViewModels
{
    internal class DebuggingWindowViewModel : IDebuggingInfoLogger
    {
        public void Log(string message)
        {
            Trace.TraceInformation("Magpie: {0}", message);
        }
    }

    internal class MainWindowViewModel : BindableBase
    {
        private readonly RemoteAppcast _remoteAppcast;
        private readonly IDebuggingInfoLogger _logger;
        private string _releaseNotes;

        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set
            {
                SetProperty(ref _releaseNotes, value);
            }
        }

        public MainWindowViewModel(RemoteAppcast appcast, IDebuggingInfoLogger logger)
        {
            _remoteAppcast = appcast;
            _logger = logger;
            PrepareReleaseNotes(appcast.ReleaseNotesUrl);
        }

        private async void PrepareReleaseNotes(string releaseNotesUrl)
        {
            _logger.Log("Fetching release notes");
            var client = new WebClient();
            var notes = await client.DownloadStringTaskAsync(new Uri(releaseNotesUrl)).ConfigureAwait(false);
            _logger.Log("Finished fetching release notes");
            _logger.Log("Converting release notes from markdown to html");
            ReleaseNotes = CommonMark.CommonMarkConverter.Convert(notes);
            _logger.Log("Finished converting release notes from markdown to html");
        }
    }
}