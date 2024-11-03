using PixelCrypt.Model;
using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.ProgramData
{
    public class Notification
    {
        public static NotificationResult MakeMessage(string content) => MakeMessage(content, "PixelCrypt", NotificationButton.Ok);

        public static NotificationResult MakeMessage(string content, string title) => MakeMessage(content, title, NotificationButton.Ok);

        public static NotificationResult MakeMessage(string content, NotificationButton notificationButton) => MakeMessage(content, "PixelCrypt", notificationButton);

        public static NotificationResult MakeMessage(string content, string title, NotificationButton messageBoxButton)
        {
            var notificationWindow = new NotificationWindow(content, title, messageBoxButton);

            var dataContext = notificationWindow.DataContext as NotificationViewModel;

            return dataContext?.NotificationResult;
        }
    }

    public enum NotificationResultType
    {
        Close,
        Ok,
        Yes,
        No,
        Text,
    }

    public enum NotificationButton
    {
        Ok,
        YesNo,
        Enter,
    }
}
