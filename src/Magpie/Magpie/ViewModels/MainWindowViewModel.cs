using System;
using System.Net;
using System.Windows.Input;
using Magpie.Interfaces;
using Magpie.Models;

namespace Magpie.ViewModels
{
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

        public ICommand ContinueUpdateCommand { get; set; }
        public ICommand SkipThisVersionCommand { get; set; }
        public ICommand RemindMeLaterCommand { get; set; }

        public MainWindowViewModel(RemoteAppcast appcast, IDebuggingInfoLogger logger)
        {
            _remoteAppcast = appcast;
            _logger = logger;
            InitializeCommands();
            PrepareReleaseNotes(appcast.ReleaseNotesUrl);
        }

        private void InitializeCommands()
        {
            ContinueUpdateCommand = new DelegateCommand(ContinueUpdateCommandHandler);
            SkipThisVersionCommand = new DelegateCommand(SkipThisVersionCommandHandler);
            RemindMeLaterCommand = new DelegateCommand(RemindMeLaterCommandHandler);
        }

        private void RemindMeLaterCommandHandler(object obj)
        {
            _logger.Log("Remind me later command invoked");
        }

        private void SkipThisVersionCommandHandler(object obj)
        {
            _logger.Log("Skip this version command invoked");
        }

        private void ContinueUpdateCommandHandler(object obj)
        {
            _logger.Log("Continue with update command invoked");
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