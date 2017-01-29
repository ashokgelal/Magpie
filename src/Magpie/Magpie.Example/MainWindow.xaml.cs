using System.Reflection;
using System.Windows;
using MagpieUpdater.Services;

namespace MagpieExample
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

        private Magpie _magpie;
        private int _selectedChannel;

        public MainWindow()
        {
            InitializeComponent();
            CurrentVersion.Content = "Current version: " + Assembly.GetEntryAssembly().GetName().Version;
            SelectedChannel = 1;
            _magpie = new Magpie(MakeAppInfo(SelectedChannel));
            _magpie.CheckInBackground();
        }

        private void ForceCheck_OnClick(object sender, RoutedEventArgs e)
        {
            _magpie.ForceCheckInBackground();
        }

        private static AppInfo MakeAppInfo(int id)
        {
            var appInfo = new AppInfo("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json", id);
            appInfo.SetAppIcon("Magpie.Example", "logo64x64.tiff");
            return appInfo;
        }

        private void StableChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 1;
            _magpie.SwitchSubscribedChannel(SelectedChannel);
        }

        private void BetaChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 2;
            _magpie.SwitchSubscribedChannel(SelectedChannel);
        }

        private void AlphaChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 3;
            _magpie.SwitchSubscribedChannel(SelectedChannel);
        }

        private void DailyBuildChannel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedChannel = 4;
            _magpie.SwitchSubscribedChannel(SelectedChannel);
        }
    }
}