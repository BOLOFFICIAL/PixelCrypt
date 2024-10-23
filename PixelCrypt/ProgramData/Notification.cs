using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.ProgramData
{
    public class Notification
    {
        public static NotificationResult MakeMessage(string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok)
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
