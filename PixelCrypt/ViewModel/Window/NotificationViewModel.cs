using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Window
{
    internal class NotificationViewModel : Base.ViewModel
    {
        private string _title;
        private string _content;
        private string _firstButtonContent;
        private string _secondButtonContent;

        private GridLength _secondButtonWidth;

        public ICommand CloseNotificationCommand { get; }

        public NotificationViewModel(string content, string title = "PixelCrypt", NotificationButton messageBoxButton = NotificationButton.Ok)
        {
            CloseNotificationCommand = new LambdaCommand(OnCloseNotificationCommandExecuted);
            _title = title;
            _content = content;

            UpdateParams(messageBoxButton);
        }

        private void OnCloseNotificationCommandExecuted(object p = null)
        {
            if (p is not string parametr) return;
            Context.NotificationWindow.Close();

            switch (parametr)
            {
                case "Ок": Context.NotificationResult = NotificationResult.Ok; break;
                case "Да": Context.NotificationResult = NotificationResult.Yes; break;
                case "Нет": Context.NotificationResult = NotificationResult.No; break;
                case "Close": Context.NotificationResult = NotificationResult.Close; break;
            }
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

        public string FirstButtonContent
        {
            get => _firstButtonContent;
            set => Set(ref _firstButtonContent, value);
        }

        public string SecondButtonContent
        {
            get => _secondButtonContent;
            set => Set(ref _secondButtonContent, value);
        }

        public GridLength SecondButtonWidth
        {
            get => _secondButtonWidth;
            set => Set(ref _secondButtonWidth, value);
        }

        private void UpdateParams(NotificationButton messageBoxButton)
        {
            switch (messageBoxButton)
            {
                case NotificationButton.Ok:
                    {
                        SecondButtonWidth = new GridLength(0, GridUnitType.Pixel);
                        FirstButtonContent = "Ок";
                        break;
                    }
                case NotificationButton.YesNo:
                    {
                        SecondButtonWidth = new GridLength(1, GridUnitType.Auto);
                        FirstButtonContent = "Да";
                        SecondButtonContent = "Нет";
                        break;
                    }
            }
        }
    }
}
