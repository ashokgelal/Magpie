namespace Magpie.Example
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            new MagpieService().RunInBackground("https://dl.dropboxusercontent.com/u/83257/Updaters/Magpie/appcast.json");
        }
    }
}
