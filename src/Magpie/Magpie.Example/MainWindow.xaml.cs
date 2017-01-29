using System.Reflection;
using System.Windows;
using MagpieUpdater.Services;

namespace Magpie.Example
{
    public partial class MainWindow
    {
        private int SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                _selectedChannel = value;
                SubscribedTo.Content = "Subscribed to Channel ID: " + value;
            }
        }

        private MagpieUpdater.Services.MagpieUpdater _magpieUpdater;
        private int _selectedChannel;

        public MainWindow()
        {
            InitializeComponent();
            CurrentVersion.Content = "Current version: " + Assembly.GetEntryAssembly().GetName().Version;
            SelectedChannel = 1;
            _magpieUpdater = new MagpieUpdater.Services.MagpieUpdater(MakeAppInfo(SelectedChannel));
            _magpieUpdater.CheckInBackground();
        }

        private void ForceCheck_OnClick(object sender, RoutedEventArgs e)
        {
            _magpieUpdater.ForceCheckInBackground();
        }

        private static AppInfo MakeAppInfo(int id)
        {
            var appInfo = new AppInfo("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json", id);
            appInfo.SetAppIcon("Magpie.Example", "logo64x64.tiff");
            return appInfo;
        }

        private void BetaChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 2;
            _magpieUpdater.SwitchSubscribedChannel(SelectedChannel);
        }

        private void AlphaChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 3;
            _magpieUpdater.SwitchSubscribedChannel(SelectedChannel);
        }

        private void DailyBuildChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 4;
            _magpieUpdater.SwitchSubscribedChannel(SelectedChannel);
        }
    }
}