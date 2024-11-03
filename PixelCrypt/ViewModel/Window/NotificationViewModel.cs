using PixelCrypt.Commands.Base;
using PixelCrypt.Model;
using PixelCrypt.ProgramData;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Window
{
    internal class NotificationViewModel : Base.ViewModel
    {
        private string _notificationResultText = "";

        public ICommand CloseNotificationCommand { get; }
        public string Title { get; set; }
        public string Content { get; set; }

        private Action _closeWindow;

        public string FirstButtonContent { get; set; }
        public string SecondButtonContent { get; set; }
        public GridLength FirstButtonWidth { get; set; } = new GridLength(0, GridUnitType.Pixel);
        public GridLength SecondButtonWidth { get; set; } = new GridLength(0, GridUnitType.Pixel);
        public GridLength EnterButtonWidth { get; set; } = new GridLength(0, GridUnitType.Pixel);
        public NotificationResult NotificationResult { get; private set; } = new NotificationResult();

        private Dictionary<string, NotificationResultType> _buttonContents = new()
        {
            {"Ок",NotificationResultType.Ok },
            {"Да",NotificationResultType.Yes },
            {"Нет",NotificationResultType.No },
            {"Close",NotificationResultType.Close },
        };

        public NotificationViewModel(Action closeWindow, string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok)
        {
            CloseNotificationCommand = new LambdaCommand(OnCloseNotificationCommandExecuted);

            Title = title;
            Content = content;
            _closeWindow = closeWindow;

            UpdateParams(messageBoxButton);
        }

        public string NotificationResultText
        {
            get => _notificationResultText;
            set => Set(ref _notificationResultText, value);
        }

        private void OnCloseNotificationCommandExecuted(object p = null)
        {
            if (p is not string parametr) return;

            NotificationResult = new NotificationResult()
            {
                NotificationResultType = _buttonContents[parametr],
                NotificationResultText = NotificationResultText,
            };

            _closeWindow();
        }

        private void UpdateParams(NotificationButton messageBoxButton)
        {
            switch (messageBoxButton)
            {
                case NotificationButton.Ok:
                    {
                        FirstButtonWidth = new GridLength(1, GridUnitType.Auto);
                        FirstButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResultType.Ok).Key;
                        break;
                    }
                case NotificationButton.YesNo:
                    {
                        FirstButtonWidth = new GridLength(1, GridUnitType.Auto);
                        SecondButtonWidth = new GridLength(1, GridUnitType.Auto);
                        FirstButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResultType.Yes).Key;
                        SecondButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResultType.No).Key;
                        break;
                    }
                case NotificationButton.Enter:
                    {
                        EnterButtonWidth = new GridLength(1, GridUnitType.Auto);
                        FirstButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResultType.Ok).Key;
                        break;
                    }
            }
        }
    }
}