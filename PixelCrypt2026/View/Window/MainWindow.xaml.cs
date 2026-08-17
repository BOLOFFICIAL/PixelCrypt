using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.View.Page;
using PixelCrypt2026.ViewModel.Page;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static MainWindow Instance { get; private set; }

        private const int WM_GETMINMAXINFO = 0x0024;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            SourceInitialized += OnSourceInitialized;
            StateChanged += (s, e) => UpdateMaximizeButton();

            ProgramHelper.CleanupTempFiles();

            var navigation = new NavigationService(MainFrame);

            navigation.Register<MainPageViewModel>(new MainPage(navigation));
            navigation.Register<CryptographyPageViewModel>(new CryptographyPage(navigation));
            navigation.Register<SteganographyPageViewModel>(new SteganographyPage(navigation));

            navigation.NavigateTo<MainPageViewModel>();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd)?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
                if (GetMonitorInfo(monitor, ref monitorInfo))
                {
                    var work = monitorInfo.rcWork;
                    var area = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.x = work.left - area.left;
                    mmi.ptMaxPosition.y = work.top - area.top;
                    mmi.ptMaxSize.x = work.right - work.left;
                    mmi.ptMaxSize.y = work.bottom - work.top;
                }
            }

            var dpi = VisualTreeHelper.GetDpi(this);
            mmi.ptMinTrackSize.x = (int)(MinWidth * dpi.PixelsPerInchX / 96.0);
            mmi.ptMinTrackSize.y = (int)(MinHeight * dpi.PixelsPerInchY / 96.0);

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateMaximizeButton()
        {
            var maximized = WindowState == WindowState.Maximized;
            MaximizeButton.Visibility = maximized ? Visibility.Collapsed : Visibility.Visible;
            RestoreButton.Visibility = maximized ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var res = Notification.Show("Do you really want to close the program?", "Exit",
                button: Program.Enum.NotificationButtonType.YesNo,
                icon: Program.Enum.NotificationIconType.Question);

            e.Cancel = res.Result != Program.Enum.NotificationResultType.Yes;

            if (res.Result == Program.Enum.NotificationResultType.Yes)
                ProgramHelper.CleanupTempFiles();

            base.OnClosing(e);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
    }
}
