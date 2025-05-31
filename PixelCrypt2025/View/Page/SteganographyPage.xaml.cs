using PixelCrypt.ViewModel.Page;

namespace PixelCrypt.View.Page
{
    /// <summary>
    /// Логика взаимодействия для SteganographyPage.xaml
    /// </summary>
    public partial class SteganographyPage : System.Windows.Controls.Page
    {
        public SteganographyPage()
        {
            InitializeComponent();
            DataContext = new SteganographyPageViewModel();
        }
    }
}
