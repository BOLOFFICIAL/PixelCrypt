using PixelCrypt.ViewModel.Page;

namespace PixelCrypt.View.Page
{
    /// <summary>
    /// Логика взаимодействия для CryptographyPage.xaml
    /// </summary>
    public partial class CryptographyPage : System.Windows.Controls.Page
    {
        public CryptographyPage()
        {
            InitializeComponent();
            DataContext = new CryptographyPageViewModel();
        }
    }
}
