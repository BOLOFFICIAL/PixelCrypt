using PixelCrypt.ViewModel;
using System.Windows.Controls;

namespace PixelCrypt.View
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = new MainPageViewModel();
        }
    }
}
