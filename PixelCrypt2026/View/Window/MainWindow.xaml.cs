using PixelCrypt2026.Program;
using PixelCrypt2026.View.Page;
using PixelCrypt2026.ViewModel.Page;
using PixelCrypt2026.ViewModel.Window;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private readonly NavigationService _navigation;

        public MainWindow()
        {
            InitializeComponent();

            _navigation = new NavigationService(MainFrame);
            _navigation.Register<MainPageViewModel>(new MainPage(_navigation));
            _navigation.Register<CryptographyPageViewModel>(new CryptographyPage(_navigation));
            _navigation.Register<SteganographyPageViewModel>(new SteganographyPage(_navigation));

            DataContext = new MainWindowViewModel(_navigation);
        }
    }
}
