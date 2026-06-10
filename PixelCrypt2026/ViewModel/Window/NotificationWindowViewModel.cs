using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.ViewModel.Base;

namespace PixelCrypt2026.ViewModel.Window
{
    class NotificationWindowViewModel : BaseViewModel
    {
        private Action _action;
        private string _content;
        private string _title;
        private NotificationType _type;
        private NotificationStatusType _status;

        public NotificationWindowViewModel(Action action, string content, string title, NotificationType type, NotificationStatusType status)
        {
            _action = action;
            _content = content;
            _title = title;
            _type = type;
            _status = status;
        }

        public NotificationResult NotificationResult { get; internal set; }
    }
}
