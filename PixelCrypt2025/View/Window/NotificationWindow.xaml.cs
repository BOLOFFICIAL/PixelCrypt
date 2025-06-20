using PixelCrypt2025.Enum;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Window;
using System.Windows;

namespace PixelCrypt2025.View.Window
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : System.Windows.Window
    {
        public NotificationWindow(string content, string title, NotificationType type)
        {
            InitializeComponent();
            DataContext = new NotificationWindowViewModel(new Action(Close),content, title, type);
            ShowDialog();
        }
    }
}
