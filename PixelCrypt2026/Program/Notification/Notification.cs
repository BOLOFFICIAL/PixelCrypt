using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.View.Window;
using PixelCrypt2026.ViewModel.Window;

namespace PixelCrypt2026.Program.Notification
{
    internal class Notification
    {
        public static NotificationResult Show(string content, string title = "PixelCrypt", NotificationType type = 0, NotificationStatusType status = 0)
        {
            var notificationWindow = new NotificationWindow(content, title, type, status);

            var dataContext = notificationWindow.DataContext as NotificationWindowViewModel;

            return dataContext?.NotificationResult;
        }
    }
}
