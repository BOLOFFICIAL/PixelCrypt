using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.ProgramData
{
    public class Notification
    {
        public static NotificationResult MakeMessage(string content)
        {
            return MakeMessage(content, "PyxelCrypt", NotificationButton.Ok);
        }

        public static NotificationResult MakeMessage(string content, string title)
        {
            return MakeMessage(content, title, NotificationButton.Ok);
        }

        public static NotificationResult MakeMessage(string content, NotificationButton notificationButton)
        {
            return MakeMessage(content, "PyxelCrypt", notificationButton);
        }

        public static NotificationResult MakeMessage(string content, string title, NotificationButton messageBoxButton)
        {
            var notificationWindow = new NotificationWindow(content, title, messageBoxButton);

            var dataContext = notificationWindow.DataContext as NotificationViewModel;

            return dataContext?.NotificationResult ?? NotificationResult.Close;
        }
    }

    public enum NotificationResult
    {
        Close,
        Ok,
        Yes,
        No,
    }

    public enum NotificationButton
    {
        Ok,
        YesNo
    }
}
