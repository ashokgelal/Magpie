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
    }
}
