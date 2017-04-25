using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MagpieUpdater.Models;
using MagpieUpdater.Properties;
using MagpieUpdater.Services;

namespace MagpieUpdater.ViewModels
{
    internal class EnrollmentViewModel : BindableBase 
    {
        private string _emailAddress;
        private readonly Enrollment _enrollment;
        private string _channelName;
        private string _appIconPath;

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
                _emailAddress = value;
                SetProperty(ref _emailAddress, value);
                EnrollCommand.RaiseCanExecuteChanged();
            }
        }

        public string ChannelName
        {
            get { return _channelName; }
            set
            {
                _channelName = value;
                SetProperty(ref _channelName, value);
                
            }
        }

        public DelegateCommand EnrollCommand { get; set; }

        public EnrollmentViewModel(Enrollment enrollment, AppInfo appInfo)
        {
            _enrollment = enrollment;
            AppIconPath = appInfo.AppIconPath;
            ChannelName = _enrollment.Channel.Build;
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
