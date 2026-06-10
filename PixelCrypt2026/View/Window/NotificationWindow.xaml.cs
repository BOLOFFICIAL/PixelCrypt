using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Window;

namespace PixelCrypt2026.View.Window
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : System.Windows.Window
    {
        private List<Action> actions;
        private NotificationIconType status;

        public NotificationWindow(string content, string title, NotificationButtonType button, NotificationIconType icon)
        {
            InitializeComponent();
            DataContext = new NotificationWindowViewModel(new Action(Close), content, title, button, icon);

            //if (Context.MainWindow.Focusable) 
            //    Owner = Context.MainWindow;

            ShowDialog();
        }

        public NotificationWindow(string content, string title, List<Action> actions, NotificationIconType icon)
        {
            InitializeComponent();
            DataContext = new NotificationWindowViewModel(new Action(Close), content, title, actions, icon);

            //if (Context.MainWindow.Focusable) 
            //    Owner = Context.MainWindow;

            ShowDialog();
        }
    }
}
