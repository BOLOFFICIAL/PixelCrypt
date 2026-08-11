using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.View.Page;
using PixelCrypt2026.ViewModel.Page;
using System.ComponentModel;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            ProgramHelper.CleanupTempFiles();

            var navigation = new NavigationService(MainFrame);

            navigation.Register<MainPageViewModel>(new MainPage(navigation));
            navigation.Register<CryptographyPageViewModel>(new CryptographyPage(navigation));
            navigation.Register<SteganographyPageViewModel>(new SteganographyPage(navigation));

            navigation.NavigateTo<MainPageViewModel>();
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
    }
}
