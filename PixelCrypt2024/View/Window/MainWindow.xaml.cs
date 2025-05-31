using PixelCrypt.ProgramData;
using PixelCrypt.ViewModel.Window;
using System.Windows;

namespace PixelCrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Context.MainWindow = this;
            DataContext = new MainWindowViewModel();
        }
    }
}