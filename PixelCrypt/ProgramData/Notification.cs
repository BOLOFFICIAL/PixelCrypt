using PixelCrypt.View.Window;

namespace PixelCrypt.ProgramData
{
    public class Notification
    {
        public static NotificationResult MakeMessage(string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok) 
        {
            var notification = new NotificationWindow(content, title, messageBoxButton);

            Context.NotificationWindow = notification;

            notification.ShowDialog();

            return Context.NotificationResult;
        }
    }

    public enum NotificationResult
    {
        Ok,
        Yes,
        No,
        Close
    }

    public enum NotificationButton
    {
        Ok,
        YesNo
    }
}
