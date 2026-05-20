using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Page;

namespace PixelCrypt2026.View.Page
{
    /// <summary>
    /// Логика взаимодействия для CryptographyPage.xaml
    /// </summary>
    public partial class CryptographyPage : System.Windows.Controls.Page
    {
        internal CryptographyPage(NavigationService navigation)
        {
            InitializeComponent();
            DataContext = new CryptographyPageViewModel(navigation);
        }
    }
}
