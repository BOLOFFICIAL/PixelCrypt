using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
            Menu menu = new Menu();
            Content = menu;
        }
    }
}
