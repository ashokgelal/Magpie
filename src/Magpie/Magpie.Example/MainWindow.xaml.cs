using System.Windows;
using Magpie.Services;

namespace Magpie.Example
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var appInfo = new AppInfo();
            appInfo.SetAppIcon("Magpie.Example", "logo64x64.tiff");
            new MagpieService(appInfo).CheckInBackground("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json");
        }

        private void ForceCheck_OnClick(object sender, RoutedEventArgs e)
        {
            var appInfo = new AppInfo();
            appInfo.SetAppIcon("Magpie.Example", "logo64x64.tiff");
            new MagpieService(appInfo).ForceCheckInBackground("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json");
        }
    }
}
