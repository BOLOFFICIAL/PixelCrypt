using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.ViewModel.Base;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Window
{
    class NotificationWindowViewModel : BaseViewModel
    {
        private Action _closeAction;
        private string _content;
        private string _title;
        private NotificationButtonType _button;
        private NotificationIconType _icon;
        private List<Action> _actions;

        public ICommand CloseCommand { get; private set; }

        public NotificationWindowViewModel(Action closeAction, string content, string title, NotificationButtonType button, NotificationIconType icon)
        {
            _closeAction = closeAction;
            _content = content;
            _title = title;
            _button = button;
            _icon = icon;

            Initialize();
        }

        public NotificationWindowViewModel(Action closeAction, string content, string title, List<Action> actions, NotificationIconType icon)
        {
            _button = NotificationButtonType.Custom;
            _closeAction = closeAction;
            _content = content;
            _title = title;
            _actions = actions;
            _icon = icon;

            Initialize();
        }

        private void Initialize() 
        {
            CloseCommand = new LambdaCommand(OnCloseCommand);
        }

        private void OnCloseCommand(object obj)
        {
            _closeAction?.Invoke();
        }

        public string Title 
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public NotificationResult NotificationResult { get; internal set; }
    }
}
