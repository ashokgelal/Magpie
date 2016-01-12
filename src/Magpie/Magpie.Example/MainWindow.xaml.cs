using System.Windows;
using Magpie.Services;

namespace Magpie.Example
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            new MagpieUpdater(MakeAppInfo()).CheckInBackground();
        }

        private void ForceCheck_OnClick(object sender, RoutedEventArgs e)
        {
            new MagpieUpdater(MakeAppInfo()).ForceCheckInBackground();
        }

        private static AppInfo MakeAppInfo()
        {
            var appInfo = new AppInfo("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json");
            appInfo.SetAppIcon("Magpie.Example", "logo64x64.tiff");
            return appInfo;
        }
    }
}
