using PixelCrypt2026.Program;
using PixelCrypt2026.View.Page;
using PixelCrypt2026.ViewModel.Page;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var navigation = new NavigationService(MainFrame);

            navigation.Register<MainPageViewModel>(new MainPage(navigation));
            navigation.Register<CryptographyPageViewModel>(new CryptographyPage(navigation));
            navigation.Register<SteganographyPageViewModel>(new SteganographyPage(navigation));

            navigation.NavigateTo<MainPageViewModel>();
        }
    }
}
