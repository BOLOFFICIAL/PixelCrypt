using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Enum;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2025.ViewModel.Window
{
    internal class NotificationWindowViewModel : Base.BaseViewModel
    {
        private string _content;
        private string _title;
        private string _inputContent = "";

        private Action _closeWindow;

        private GridLength _okGridLength = Constants.GridLengthZero;
        private GridLength _yesNoGridLength = Constants.GridLengthZero;
        private GridLength _textGridLength = Constants.GridLengthZero;
        private string _statusIcon;
        private string _statusColor;

        public ICommand ReturnResultCommand { get; }

        public NotificationWindowViewModel(Action closeWindow, string content, string title, NotificationType type, NotificationStatus status)
        {
            Content = content;
            Title = title;

            _closeWindow = closeWindow;

            ReturnResultCommand = new LambdaCommand(OnReturnResultCommandExecuted);

            UpdateActions(type);
            UpdateStatus(status);
        }

        public NotificationResult NotificationResult { get; private set; } = new NotificationResult()
        {
            Result = NotificationResultType.Cancel,
        };

        public NotificationResultType ResultTypeOk => NotificationResultType.Ok;
        public NotificationResultType ResultTypeYes => NotificationResultType.Yes;
        public NotificationResultType ResultTypeNo => NotificationResultType.No;
        public NotificationResultType ResultTypeText => NotificationResultType.Text;

        public string StatusColor
        {
            get => _statusColor;
            set => Set(ref _statusColor, value);
        }

        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public string StatusIcon
        {
            get => _statusIcon;
            set => Set(ref _statusIcon, value);
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public string InputContent
        {
            get => _inputContent;
            set => Set(ref _inputContent, value);
        }

        public GridLength OkGridLength
        {
            get => _okGridLength;
            set => Set(ref _okGridLength, value);
        }

        public GridLength YesNoGridLength
        {
            get => _yesNoGridLength;
            set => Set(ref _yesNoGridLength, value);
        }

        public GridLength TextGridLength
        {
            get => _textGridLength;
            set => Set(ref _textGridLength, value);
        }

        private void OnReturnResultCommandExecuted(object obj)
        {
            if (obj is not NotificationResultType parametr) return;

            NotificationResult = new NotificationResult()
            {
                Result = parametr,
                Content = InputContent,
            };

            _closeWindow.Invoke();
        }

        private void UpdateActions(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Ok:
                    OkGridLength = Constants.GridLengthStar;
                    break;
                case NotificationType.YesNo:
                    YesNoGridLength = Constants.GridLengthStar;
                    break;
                case NotificationType.Text:
                    TextGridLength = Constants.GridLengthStar;
                    break;
            }
        }

        private void UpdateStatus(NotificationStatus status)
        {
            switch (status)
            {
                case NotificationStatus.Success:
                    {
                        StatusIcon = "Solid_CheckCircle";
                        StatusColor = Palette.Color5;
                    }
                    break;
                case NotificationStatus.Error:
                    {
                        StatusIcon = "Solid_TimesCircle";
                        StatusColor = Palette.Color4;
                    }
                    break;
                case NotificationStatus.Question:
                    {
                        StatusIcon = "Solid_ExclamationTriangle";
                        StatusColor = Palette.Color6;
                    }
                    break;
                default:
                    {
                        StatusIcon = "Solid_Ban";
                        StatusColor = Palette.Color3;
                    }
                    break;
            }
        }
    }
}
