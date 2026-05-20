using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Page;

namespace PixelCrypt2026.View.Page
{
    /// <summary>
    /// Логика взаимодействия для SteganographyPage.xaml
    /// </summary>
    public partial class SteganographyPage : System.Windows.Controls.Page
    {
        internal SteganographyPage(NavigationService navigation)
        {
            InitializeComponent();
            DataContext = new SteganographyPageViewModel(navigation);
        }
    }
}
