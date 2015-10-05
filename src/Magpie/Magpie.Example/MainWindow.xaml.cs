using Magpie.Services;

namespace Magpie.Example
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var appInfo = new AppInfo();
            appInfo.SetAppIcon("Magpie.Example", "lp.png");
            new MagpieService(appInfo).RunInBackground("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json");
        }
    }
}
