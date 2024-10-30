using PixelCrypt.ProgramData;

namespace PixelCrypt.View
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = Context.MainPageViewModel;
        }
    }
}
