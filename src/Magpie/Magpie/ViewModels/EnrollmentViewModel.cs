using System;
using System.Net.Mail;
using MagpieUpdater.Models;
using MagpieUpdater.Services;

namespace MagpieUpdater.ViewModels
{
    internal class EnrollmentViewModel : BindableBase 
    {
        private string _emailAddress;
        private readonly Enrollment _enrollment;
        private string _channelName;
        private string _appIconPath;
        private string _enrollmentEulaUrl;

        public string AppIconPath
        {
            get { return _appIconPath; }
            set { SetProperty(ref _appIconPath, value); }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                SetProperty(ref _emailAddress, value);
                EnrollCommand.RaiseCanExecuteChanged();
            }
        }

        public string ChannelName
        {
            get { return _channelName; }
            set { SetProperty(ref _channelName, value); }
        }

        public string EnrollmentEulaUrl
        {
            get { return _enrollmentEulaUrl; }
            set { SetProperty(ref _enrollmentEulaUrl, value); }
        }

        public DelegateCommand EnrollCommand { get; private set; }

        public EnrollmentViewModel(Enrollment enrollment, AppInfo appInfo)
        {
            _enrollment = enrollment;
            AppIconPath = appInfo.AppIconPath;
            ChannelName = _enrollment.Channel.Build;
            EnrollmentEulaUrl = _enrollment.Channel.EnrollmentEulaUrl;
            EnrollCommand = new DelegateCommand(EnrollCommandHandler, CanEnroll);
        }

        private bool CanEnroll(object obj)
        {
            return ValidateEmailFormat();
        }

        private void EnrollCommandHandler(object obj)
        {
            _enrollment.IsEnrolled = true;
            _enrollment.Email = EmailAddress;
        }

        private bool ValidateEmailFormat()
        {
            if (string.IsNullOrWhiteSpace(EmailAddress))
            {
                return false;
            }
            try
            {
#pragma warning disable 168
                var m = new MailAddress(EmailAddress);
#pragma warning restore 168
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}
