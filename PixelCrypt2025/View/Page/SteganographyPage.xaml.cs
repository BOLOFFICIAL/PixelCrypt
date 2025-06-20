using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Page;

namespace PixelCrypt2025.View.Page
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
            Context.SteganographyPage = Context.SteganographyPage ?? this;
        }
    }
}
