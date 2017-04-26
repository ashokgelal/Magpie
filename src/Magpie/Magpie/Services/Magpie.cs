using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.ViewModels;
using MagpieUpdater.Views;

namespace MagpieUpdater.Services
{
    public class Magpie : ISoftwareUpdater
    {
        public AppInfo AppInfo { get; private set; }
        private readonly IDebuggingInfoLogger _logger;
        private readonly IAnalyticsLogger _analyticsLogger;
        internal UpdateDecider UpdateDecider { get; set; }
        internal BestChannelFinder BestChannelFinder { get; set; }
        internal IRemoteContentDownloader RemoteContentDownloader { get; set; }
        public event EventHandler<SingleEventArgs<RemoteAppcast>> RemoteAppcastAvailableEvent;
        public event EventHandler<SingleEventArgs<string>> ArtifactDownloadedEvent;
        public event EventHandler<SingleEventArgs<Enrollment>> EnrollmentAvailableEvent;

        public Magpie(AppInfo appInfo, IDebuggingInfoLogger debuggingInfoLogger = null,
            IAnalyticsLogger analyticsLogger = null)
        {
            AppInfo = appInfo;
            _logger = debuggingInfoLogger ?? new DebuggingWindowViewModel();
            _analyticsLogger = analyticsLogger ?? new AnalyticsLogger();
            RemoteContentDownloader = new DefaultRemoteContentDownloader();
            UpdateDecider = new UpdateDecider(_logger);
            BestChannelFinder = new BestChannelFinder(_logger);
        }

        public async void CheckInBackground(string appcastUrl = null, bool showDebuggingWindow = false)
        {
            await Check(appcastUrl ?? AppInfo.AppCastUrl, CheckState.InBackground, AppInfo.SubscribedChannel,
                showDebuggingWindow).ConfigureAwait(false);
        }

        public async void ForceCheckInBackground(string appcastUrl = null, bool showDebuggingWindow = false)
        {
            await Check(appcastUrl ?? AppInfo.AppCastUrl, CheckState.Force, AppInfo.SubscribedChannel,
                showDebuggingWindow).ConfigureAwait(false);
        }

        public async void SwitchSubscribedChannel(int channelId, bool showDebuggingWindow = false)
        {
            AppInfo.SubscribedChannel = channelId;
            await Check(AppInfo.AppCastUrl, CheckState.ChannelSwitch, channelId, showDebuggingWindow)
                .ConfigureAwait(false);
        }

        private async Task Check(string appcastUrl, CheckState checkState, int channelId = 1, bool showDebuggingWindow = false)
        {
            _logger.Log(string.Format("Starting fetching remote channel content from address: {0}", appcastUrl));
            try
            {
                var data = await RemoteContentDownloader.DownloadStringContent(appcastUrl, _logger).ConfigureAwait(true);
                if (string.IsNullOrWhiteSpace(data))
                {
                    if (checkState == CheckState.Force || checkState == CheckState.ChannelSwitch)
                    {
                        ShowErrorWindow();
                    }
                    return;
                }

                var appcast = ParseAppcast(data);

                if (checkState == CheckState.ChannelSwitch && FailedToEnroll(appcast, channelId)) return;

                var channelToUpdateFrom = BestChannelFinder.Find(channelId, appcast.Channels);

                if (UpdateDecider.ShouldUpdate(channelToUpdateFrom, checkState == CheckState.Force || checkState == CheckState.ChannelSwitch))
                {
                    _analyticsLogger.LogUpdateAvailable(channelToUpdateFrom);
                    await ShowUpdateWindow(channelToUpdateFrom);
                }
                else if (checkState == CheckState.Force)
                {
                    ShowNoUpdatesWindow();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(string.Format("Error parsing remote channel: {0}", ex.Message));
            }
            finally
            {
                _logger.Log("Finished fetching remote channel content");
            }
        }

        private bool FailedToEnroll(RemoteAppcast appcast, int channelId)
        {
            var channel = appcast.Channels.FirstOrDefault(c => c.Id == channelId);
            var enrollment = new Enrollment(channel);
            if (channel != null && channel.RequiresEnrollment)
            {
                enrollment.IsRequired = true;
                ShowEnrollmentWindow(enrollment);
            }
            _analyticsLogger.LogEnrollment(enrollment);
            OnEnrollmentAvailableEvent(new SingleEventArgs<Enrollment>(enrollment));
            return enrollment.IsRequired && !enrollment.IsEnrolled;
        }

        protected virtual void ShowEnrollmentWindow(Enrollment enrollment)
        {
            var viewModel = new EnrollmentViewModel(enrollment, AppInfo);
            var window = new EnrollmentWindow { DataContext = viewModel };
            SetOwner(window);
            OnWindowWillBeDisplayed(window, enrollment.Channel);
            window.ShowDialog();
        }

        protected virtual async Task ShowUpdateWindow(Channel channel)
        {
            var viewModel = new MainWindowViewModel(AppInfo, _logger, RemoteContentDownloader, _analyticsLogger);
            await viewModel.StartAsync(channel).ConfigureAwait(true);
            var window = new MainWindow {ViewModel = viewModel};
            viewModel.DownloadNowCommand = new DelegateCommand(async e =>
            {
                _analyticsLogger.LogDownloadNow();
                _logger.Log("Continuing with downloading the artifact");
                window.Close();
                await ShowDownloadWindow(channel);
            });
            SetOwner(window);
            OnWindowWillBeDisplayed(window, channel);
            window.ShowDialog();
        }

        protected virtual void OnWindowWillBeDisplayed(Window window, Channel channel = null)
        {
        }

        protected virtual void ShowNoUpdatesWindow()
        {
            var window = new NoUpdatesWindow();
            SetOwner(window);
            OnWindowWillBeDisplayed(window);
            window.ShowDialog();
        }

        protected virtual void ShowErrorWindow()
        {
            var window = new ErrorWindow();
            SetOwner(window);
            OnWindowWillBeDisplayed(window);
            window.ShowDialog();
        }

        private static string CreateTempPath(string url)
        {
            var uri = new Uri(url);
            var path = Path.GetTempPath();
            var fileName = string.Format(Guid.NewGuid() + Path.GetFileName(uri.LocalPath));
            return Path.Combine(path, fileName);
        }

        protected virtual async Task ShowDownloadWindow(Channel channel)
        {
            var viewModel = new DownloadWindowViewModel(AppInfo, _logger, RemoteContentDownloader);
            var artifactPath = CreateTempPath(channel.ArtifactUrl);
            var window = new DownloadWindow {DataContext = viewModel};
            bool[] finishedDownloading = {false};
            viewModel.ContinueWithInstallationCommand = new DelegateCommand(e =>
            {
                _logger.Log("Continue after downloading artifact");
                _analyticsLogger.LogContinueWithInstallation();
                OnArtifactDownloadedEvent(new SingleEventArgs<string>(artifactPath));
                window.Close();
                if (ShouldOpenArtifact(channel, artifactPath))
                {
                    OpenArtifact(artifactPath);
                    _logger.Log("Opened artifact");
                }
            }, o => finishedDownloading[0]);

            SetOwner(window);
            OnWindowWillBeDisplayed(window, channel);
            window.Show();

            var savedAt = await viewModel.StartAsync(channel, artifactPath).ConfigureAwait(true);
            finishedDownloading[0] = true;
            ((DelegateCommand) viewModel.ContinueWithInstallationCommand).RaiseCanExecuteChanged();

            if (string.IsNullOrWhiteSpace(savedAt))
            {
                window.Close();
                ShowErrorWindow();
            }
        }

        private bool ShouldOpenArtifact(Channel channel, string artifactPath)
        {
            if (string.IsNullOrEmpty(channel.DSASignature))
            {
                _logger.Log("No DSASignature provided. Skipping signature verification");
                return true;
            }
            _logger.Log("DSASignature provided. Verifying artifact's signature");
            if (VerifyArtifact(channel, artifactPath))
            {
                _logger.Log("Successfully verified artifact's signature");
                return true;
            }
            _logger.Log("Couldn't verify artifact's signature. The artifact will now be deleted.");
            var signatureWindowViewModel = new SignatureVerificationWindowViewModel(AppInfo);
            var signatureWindow = new SignatureVerificationWindow {DataContext = signatureWindowViewModel};
            signatureWindowViewModel.ContinueCommand = new DelegateCommand(e => { signatureWindow.Close(); });
            SetOwner(signatureWindow);
            OnWindowWillBeDisplayed(signatureWindow, channel);
            signatureWindow.ShowDialog();
            return false;
        }

        protected virtual bool VerifyArtifact(Channel channel, string artifactPath)
        {
            var verifer = new SignatureVerifier(AppInfo.PublicSignatureFilename);
            return verifer.VerifyDSASignature(channel.DSASignature, artifactPath);
        }

        protected virtual void OpenArtifact(string artifactPath)
        {
            Process.Start(artifactPath);
        }

        protected virtual void SetOwner(Window window)
        {
            if (AppInfo.InteropWithWinForm)
            {
                window.SetOwnerToTopMostWinForm();
            }
            else
            {
                window.SetOwnerToTopMostWindow();
            }
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private RemoteAppcast ParseAppcast(string content)
        {
            _logger.Log("Started deserializing remote channel content");
            var appcast = RemoteAppcast.MakeFromJson(content);
            _logger.Log("Finished deserializing remote channel content");
            OnRemoteAppcastAvailableEvent(new SingleEventArgs<RemoteAppcast>(appcast));
            return appcast;
        }

        protected virtual void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            var handler = RemoteAppcastAvailableEvent;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnArtifactDownloadedEvent(SingleEventArgs<string> args)
        {
            var handler = ArtifactDownloadedEvent;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnEnrollmentAvailableEvent(SingleEventArgs<Enrollment> args)
        {
            var handler = EnrollmentAvailableEvent;
            if (handler != null) handler(this, args);
        }
    }
}