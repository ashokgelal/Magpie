using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Navigation;
using MagpieUpdater.Services;
using MagpieUpdater.ViewModels;
using Application = System.Windows.Application;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace MagpieUpdater.Views
{
    public partial class MainWindow
    {
        private MainWindowViewModel _viewModel;
        private bool _firstPageLoaded;
        public static string CustomStylesFilename = "Resources/MagpieStyles.xaml";

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
            var styles = string.Format("/{0};component/{1}", new AssemblyAccessor().AssemblyName, CustomStylesFilename);
            this.AddCustomStyles(styles);
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

        private void ReleaseNotesBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!_firstPageLoaded)
            {
                _firstPageLoaded = true;
                return;
            }
            e.Cancel = true;
            Process.Start(e.Uri.ToString());
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

    public static class WindowExtension
    {
        public static bool AddCustomStyles(this Window window, string styleResourceFile)
        {
            try
            {
                var res = (ResourceDictionary)Application.LoadComponent(new Uri(styleResourceFile, UriKind.Relative));
                if (res != null)
                {
                    window.Resources.MergedDictionaries.Add(res);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AddCustomResource(this Window window, string key, string value)
        {
            try
            {
                window.Resources[key] = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void SetOwnerToTopMostWindow(this Window window)
        {
            if (Application.Current == null || Application.Current.MainWindow.Equals(window)) return;
            window.Owner = Application.Current.MainWindow;
        }

        public static WindowInteropHelper SetOwnerToTopMostWinForm(this Window window, Form form = null)
        {
            if (form == null)
            {
                form = WindowWinFormInteropExtension.GetPossibleTopMostForm();
            }
            if (form != null)
            {
                form = WindowWinFormInteropExtension.GetTopMostWindow(form.Handle);
            }
            return form == null ? null : new WindowInteropHelper(window) { Owner = form.Handle };
        }
    }

    // From: http://stackoverflow.com/a/1005815/33203
    public static class WindowWinFormInteropExtension
    {
        public const int GW_HWNDNEXT = 2; // The next window is below the specified window
        public const int GW_HWNDPREV = 3; // The previous window is above

        [DllImport("user32.dll")]
        static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindow", SetLastError = true)]
        public static extern IntPtr GetNextWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);


        public static Form GetPossibleTopMostForm()
        {
            return System.Windows.Forms.Application.OpenForms.Cast<Form>().FirstOrDefault(x => x.Focused);
        }

        /// <summary>
        /// Searches for the topmost visible form of your app in all the forms opened in the current Windows session.
        /// </summary>
        /// <param name="hWndMainFrm">Handle of the main form</param>
        /// <returns>The Form that is currently TopMost, or null</returns>
        public static Form GetTopMostWindow(IntPtr hWndMainFrm)
        {

            var hwnd = GetTopWindow((IntPtr)null);
            if (hwnd == IntPtr.Zero) return null;

            Form frm = null;
            while ((!IsWindowVisible(hwnd) || frm == null) && hwnd != hWndMainFrm)
            {
                // Get next window under the current handler
                hwnd = GetNextWindow(hwnd, GW_HWNDNEXT);

                try
                {
                    frm = (Form)Form.FromHandle(hwnd);
                }
                catch
                {
                    // Weird behaviour: In some cases, trying to cast to a Form a handle of an object 
                    // that isn't a form will just return null. In other cases, will throw an exception.
                }
            }
            return frm;
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


        public static bool GetShowIcon(UIElement element)
        {
            return (bool) element.GetValue(ShowIconProperty);
        }

        public static void RemoveIcon(Window window)
        {
            window.SourceInitialized += delegate
            {
                // Get this window's handle
                var hwnd = new WindowInteropHelper(window).Handle;

                // Change the extended window style to not show a window icon
                var extendedStyle = GetWindowLong(hwnd, GwlExstyle);
                SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExDlgmodalframe);

                // Update the window's non-client area to reflect the changes
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove |
                                                            SwpNosize | SwpNozorder | SwpFramechanged);
            };
        }

        public static void SetShowIcon(UIElement element, bool value)
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