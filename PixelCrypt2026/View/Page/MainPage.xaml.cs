using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Page;

namespace PixelCrypt2026.View.Page
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page
    {
        internal MainPage(NavigationService navigation)
        {
            InitializeComponent();
            DataContext = new MainPageViewModel(navigation);

#if DEBUG
            Version.Text = "DEBUG";
#endif

        }
    }
}
