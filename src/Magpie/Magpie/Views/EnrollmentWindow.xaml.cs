using System.Diagnostics;
using System.Windows.Navigation;

namespace MagpieUpdater.Views
{
    public partial class EnrollmentWindow
    {
        public EnrollmentWindow()
        {
            InitializeComponent();
            SetValue(NoIconBehavior.ShowIconProperty, false);
        }

        private void CloseWindow(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void NavigateUri(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
