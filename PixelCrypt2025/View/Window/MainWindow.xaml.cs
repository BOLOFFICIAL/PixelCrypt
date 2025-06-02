using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Window;

namespace PixelCrypt2025.View.Window
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
