using PixelCrypt2025.Enum;
using PixelCrypt2025.Model;
using PixelCrypt2025.View.Window;
using PixelCrypt2025.ViewModel.Base;
using PixelCrypt2025.ViewModel.Window;
using System.Windows;

namespace PixelCrypt2025.ProgramData
{
    internal class Notification
    {
        public static NotificationResult Show(string content, string title = "PixelCrypt", NotificationType type = 0, NotificationStatus status = 0) 
        {
            var notificationWindow = new NotificationWindow(content, title, type, status);

            var dataContext = notificationWindow.DataContext as NotificationWindowViewModel;

            return dataContext?.NotificationResult;
        }
    }
}
