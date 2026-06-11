using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.View.Window;
using PixelCrypt2026.ViewModel.Window;

namespace PixelCrypt2026.Program.Notification
{
    internal class Notification
    {
        public static NotificationResult Show(string content, string title = "PixelCrypt", NotificationButtonType button = 0, NotificationIconType icon = 0)
        {
            var notificationWindow = new NotificationWindow(content, title, button, icon);

            var dataContext = notificationWindow.DataContext as NotificationWindowViewModel;

            return dataContext?.NotificationResult;
        }

        public static NotificationResult Show(string content, List<(string, Action)> actions, string title = "PixelCrypt", NotificationIconType icon = 0)
        {
            var notificationWindow = new NotificationWindow(content, title, actions, icon);

            var dataContext = notificationWindow.DataContext as NotificationWindowViewModel;

            return dataContext?.NotificationResult;
        }
    }
}
