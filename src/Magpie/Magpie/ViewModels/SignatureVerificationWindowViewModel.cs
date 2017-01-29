using System.Windows.Input;
using Magpie.Services;

namespace Magpie.ViewModels
{
    internal class SignatureVerificationWindowViewModel : BindableBase
    {
        private string _appIconPath;
        private string _title;
        public ICommand ContinueCommand { get; set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string AppIconPath
        {
            get { return _appIconPath; }
            set { SetProperty(ref _appIconPath, value); }
        }

        public SignatureVerificationWindowViewModel(AppInfo appInfo)
        {
            AppIconPath = appInfo.AppIconPath;
            Title = string.Format(Properties.Resources.SignatureErrorTitle, MainAssembly.ProductName);
        }
    }
}