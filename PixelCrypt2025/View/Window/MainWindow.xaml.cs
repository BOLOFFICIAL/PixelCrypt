using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Window;
using System.ComponentModel;

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

        protected override void OnClosing(CancelEventArgs e)
        {
            var res = Notification.Show(
                content: "Вы действительно хотите закрыть программу?",
                type: Enum.NotificationType.YesNo,
                status: Enum.NotificationStatus.Question);

            e.Cancel = res.Result != Enum.NotificationResultType.Yes;
            base.OnClosing(e);
        }
    }
}
