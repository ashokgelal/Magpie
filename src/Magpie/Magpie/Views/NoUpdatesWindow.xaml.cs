using System.Diagnostics;
using System.Windows.Navigation;

namespace MagpieUpdater.Views
{
    public partial class NoUpdatesWindow
    {
        public NoUpdatesWindow()
        {
            InitializeComponent();
            SetValue(NoIconBehavior.ShowIconProperty, false);
        }

        private void PoweredBy_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}