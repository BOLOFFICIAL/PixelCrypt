using PixelCrypt.ProgramData;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.View.Window
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : System.Windows.Window
    {
        public NotificationWindow(string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok)
        {
            InitializeComponent();
            DataContext = new NotificationViewModel((new Action(Close)), content, title, messageBoxButton);
            ShowDialog();
        }
    }
}
