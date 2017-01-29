using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using Magpie.ViewModels;

namespace Magpie.Views
{
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;

        internal MainWindowViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                DataContext = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            SetValue(NoIconBehavior.ShowIconProperty, false);
        }

        private void PoweredBy_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ViewModel.CancelUpdate();
        }

        private void RemindMeLater_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.RemindMeLaterCommand.Execute(null);
            }
        }

        private void SkipThisVersion_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.SkipThisVersionCommand.Execute(null);
            }
        }
    }

    internal class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowserBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string) d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
                webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
        }
    }

    internal class NoIconBehavior
    {
        private const int GwlExstyle = -20;
        private const int SwpFramechanged = 0x0020;
        private const int SwpNomove = 0x0002;
        private const int SwpNosize = 0x0001;
        private const int SwpNozorder = 0x0004;
        private const int WsExDlgmodalframe = 0x0001;

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.RegisterAttached(
                "ShowIcon",
                typeof(bool),
                typeof(NoIconBehavior),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => RemoveIcon((Window) d))));


        public static Boolean GetShowIcon(UIElement element)
        {
            return (Boolean) element.GetValue(ShowIconProperty);
        }

        public static void RemoveIcon(Window window)
        {
            window.SourceInitialized += delegate
            {
                // Get this window's handle
                var hwnd = new WindowInteropHelper(window).Handle;

                // Change the extended window style to not show a window icon
                int extendedStyle = GetWindowLong(hwnd, GwlExstyle);
                SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExDlgmodalframe);

                // Update the window's non-client area to reflect the changes
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove |
                                                            SwpNosize | SwpNozorder | SwpFramechanged);
            };
        }

        public static void SetShowIcon(UIElement element, Boolean value)
        {
            element.SetValue(ShowIconProperty, value);
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
            IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
            int x, int y, int width, int height, uint flags);
    }
}