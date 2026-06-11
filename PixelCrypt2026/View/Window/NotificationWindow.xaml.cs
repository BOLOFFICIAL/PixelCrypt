using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Window;
using System.Windows.Input;

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
            Initialize();
        }

        public NotificationWindow(string content, string title, List<(string, Action)> actions, NotificationIconType icon)
        {
            InitializeComponent();      
            DataContext = new NotificationWindowViewModel(new Action(Close), content, title, actions, icon);
            Initialize();
        }

        private void Initialize() 
        {
            if (MainWindow.Instance.Focusable)
                Owner = MainWindow.Instance;

            this.KeyDown += OnKeyDown;

            ShowDialog();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
                Close();
        }
    }
}
