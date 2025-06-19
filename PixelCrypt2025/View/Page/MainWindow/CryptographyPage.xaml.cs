using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Page.MainWindow;

namespace PixelCrypt2025.View.Page.MainWindow
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
            Context.CryptographyPage = Context.CryptographyPage ?? this;
        }
    }
}
