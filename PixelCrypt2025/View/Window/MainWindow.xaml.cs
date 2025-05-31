using PixelCrypt.ProgramData;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.View.Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Context.MainWindow = this;
            DataContext = new MainWindowViewModel();
        }
    }
}
