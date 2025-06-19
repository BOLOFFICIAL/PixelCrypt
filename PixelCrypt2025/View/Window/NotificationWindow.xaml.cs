using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Window;

namespace PixelCrypt2025.View.Window
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : System.Windows.Window
    {
        public NotificationWindow()
        {
            InitializeComponent();
            Context.NotificationWindow = this;
            DataContext = new NotificationWindowViewModel();
        }
    }
}
