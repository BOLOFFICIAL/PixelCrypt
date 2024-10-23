using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Window
{
    internal class NotificationViewModel : Base.ViewModel
    {
        public ICommand CloseNotificationCommand { get; }
        public string Title { get; set; }
        public string Content { get; set; }

        private Action _closeWindow;

        public string FirstButtonContent { get; set; }
        public string SecondButtonContent { get; set; }
        public GridLength SecondButtonWidth { get; set; }
        public NotificationResult NotificationResult { get; private set; }

        private Dictionary<string, NotificationResult> _buttonContents = new()
        {
            {"Ок",NotificationResult.Ok },
            {"Да",NotificationResult.Yes },
            {"Нет",NotificationResult.No },
            {"Close",NotificationResult.Close },
        };

        public NotificationViewModel(Action closeWindow, string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok)
        {
            CloseNotificationCommand = new LambdaCommand(OnCloseNotificationCommandExecuted);

            Title = title;
            Content = content;
            _closeWindow = closeWindow;

            UpdateParams(messageBoxButton);
        }

        private void OnCloseNotificationCommandExecuted(object p = null)
        {
            if (p is not string parametr) return;
            NotificationResult = _buttonContents[parametr];
            _closeWindow();
        }

        private void UpdateParams(NotificationButton messageBoxButton)
        {
            switch (messageBoxButton)
            {
                case NotificationButton.Ok:
                    {
                        SecondButtonWidth = new GridLength(0, GridUnitType.Pixel);
                        FirstButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResult.Ok).Key;
                        break;
                    }
                case NotificationButton.YesNo:
                    {
                        SecondButtonWidth = new GridLength(1, GridUnitType.Auto);
                        FirstButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResult.Yes).Key;
                        SecondButtonContent = _buttonContents.FirstOrDefault(x => x.Value == NotificationResult.No).Key;
                        break;
                    }
            }
        }
    }
}