using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Window;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : System.Windows.Window
    {
        public NotificationWindow(string content, string title, NotificationType type, NotificationStatusType status)
        {
            InitializeComponent();
            DataContext = new NotificationWindowViewModel(new Action(Close), content, title, type, status);

            //if (Context.MainWindow.Focusable) 
            //    Owner = Context.MainWindow;

            ShowDialog();
        }
    }
}
