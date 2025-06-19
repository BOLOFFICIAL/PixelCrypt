using PixelCrypt2025.ViewModel.Page.MainWindow;

namespace PixelCrypt2025.View.Page.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }
    }
}
