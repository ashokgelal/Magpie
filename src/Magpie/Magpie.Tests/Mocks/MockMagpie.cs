﻿using System.Threading.Tasks;
using Magpie.Tests.Models;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.Services;
using NSubstitute;

namespace Magpie.Tests.Mocks
{
    internal class MockMagpie : MagpieUpdater.Services.Magpie
    {
        private readonly string VALID_JSON =
            @"{'foo': 'bar', 'channels': [{ 'id': 2, 'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'}, { 'id': 3, 'version': '6.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'}, { 'id': 4, 'version': '7.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017', 'requires_enrollment': 'true', 'enrollment_eula_url': 'enrollment_url'}]}"
                .MakeJson();


        internal RemoteAppcast RemoteAppcast { get; private set; }
        internal bool _showUpdateWindowFlag;
        internal bool _showNoUpdatesWindowFlag;
        internal IRemoteContentDownloader _remoteContentDownloader;
        internal bool _showEnrollmentWindow;
        internal Enrollment _enrollmentToReturn;

        public MockMagpie(string validUrl, IDebuggingInfoLogger infoLogger = null, IAnalyticsLogger analyticsLogger = null)
            : base(new AppInfo(validUrl), infoLogger, analyticsLogger)
        {
            _remoteContentDownloader = Substitute.For<IRemoteContentDownloader>();
            _remoteContentDownloader.DownloadStringContent(validUrl, Arg.Any<IDebuggingInfoLogger>()).Returns(Task.FromResult(VALID_JSON));
            RemoteContentDownloader = _remoteContentDownloader;
        }

        protected override void OnRemoteAppcastAvailableEvent(SingleEventArgs<RemoteAppcast> args)
        {
            RemoteAppcast = args.Payload;
            base.OnRemoteAppcastAvailableEvent(args);
        }

        protected override async Task ShowUpdateWindow(Channel channel)
        {
            // can't do in tests
            _showUpdateWindowFlag = true;
        }

        protected override void ShowNoUpdatesWindow()
        {
            // can't do in tests
            _showNoUpdatesWindowFlag = true;
        }

        protected override void ShowErrorWindow()
        {
        }

        protected override void ShowEnrollmentWindow(Enrollment enrollment)
        {
            _showEnrollmentWindow = true;
            enrollment.IsEnrolled = _enrollmentToReturn.IsEnrolled;
            enrollment.Email = _enrollmentToReturn.Email;
        }
    }
}