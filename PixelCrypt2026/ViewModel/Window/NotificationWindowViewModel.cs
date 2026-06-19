using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PixelCrypt2026.ViewModel.Window
{
    class NotificationWindowViewModel : BaseViewModel
    {
        private Action _closeAction;
        private NotificationButtonType _button;
        private NotificationIconType _icon;
        private List<(string title, Action action)> _actions;
        private GridLength _heightButtonOk = new GridLength(0, GridUnitType.Pixel);
        private GridLength _heightButtonYesNo = new GridLength(0, GridUnitType.Pixel);
        private GridLength _heightButtonText = new GridLength(0, GridUnitType.Pixel);
        private GridLength _heightButtonCustom = new GridLength(0, GridUnitType.Pixel);
        private string _inputText;
        private DispatcherTimer _autoCloseTimer;

        public ICommand CloseCommand { get; private set; }
        public ICommand OkCommand { get; private set; }
        public ICommand YesCommand { get; private set; }
        public ICommand NoCommand { get; private set; }
        public ICommand TextCommand { get; private set; }

        public GridLength HeightButtonOk
        {
            get => _heightButtonOk;
            set => Set(ref _heightButtonOk, value);
        }
        public GridLength HeightButtonYesNo
        {
            get => _heightButtonYesNo;
            set => Set(ref _heightButtonYesNo, value);
        }
        public GridLength HeightButtonText
        {
            get => _heightButtonText;
            set => Set(ref _heightButtonText, value);
        }
        public GridLength HeightButtonCustom
        {
            get => _heightButtonCustom;
            set => Set(ref _heightButtonCustom, value);
        }

        public ObservableCollection<NotificationButton> Buttons { get; private set; } = new ObservableCollection<NotificationButton>();

        public NotificationWindowViewModel(Action closeAction, string content, string title, NotificationButtonType button, NotificationIconType icon)
        {
            _closeAction = closeAction;
            Content = content;
            Title = title;
            _button = button;
            _icon = icon;

            Initialize();
        }

        public NotificationWindowViewModel(Action closeAction, string content, string title, List<(string, Action)> actions, NotificationIconType icon)
        {
            _button = NotificationButtonType.Custom;
            _closeAction = closeAction;
            Content = content;
            Title = title;
            _actions = actions;
            _icon = icon;

            Initialize();
        }

        private void Initialize()
        {
            CloseCommand = new LambdaCommand(OnCloseCommand);
            SetIcon();
            SetColor();
            SetButton();
        }

        private void SetIcon()
        {
            var icon = "";

            switch (_icon)
            {
                case NotificationIconType.Success: icon = "Solid_CheckCircle"; break;
                case NotificationIconType.Error: icon = "Solid_TimesCircle"; break;
                case NotificationIconType.Question: icon = "Solid_ExclamationTriangle"; break;
                default: icon = "Solid_Bell"; break;
            }

            Icon = icon;
        }

        private void SetButton()
        {
            var height = new GridLength(1, GridUnitType.Auto);

            switch (_button)
            {
                case NotificationButtonType.None:
                    _autoCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                    _autoCloseTimer.Tick += (s, e) =>
                    {
                        _autoCloseTimer.Stop();
                        OnCloseCommand(null);
                    };
                    _autoCloseTimer.Start();
                    break;
                case NotificationButtonType.Ok:
                    HeightButtonOk = height;
                    OkCommand = new LambdaCommand(OnOkCommand);
                    break;
                case NotificationButtonType.YesNo:
                    HeightButtonYesNo = height;
                    YesCommand = new LambdaCommand(OnYesCommand);
                    NoCommand = new LambdaCommand(OnNoCommand);
                    break;
                case NotificationButtonType.Text:
                    HeightButtonText = height;
                    TextCommand = new LambdaCommand(OnTextCommand);
                    break;
                case NotificationButtonType.Custom:
                    HeightButtonCustom = height;
                    foreach (var action in _actions)
                    {
                        Buttons.Add(new NotificationButton()
                        {
                            Text = action.title,
                            Command = new LambdaCommand((object obj) =>
                            {
                                action.action?.Invoke();
                                NotificationResult.Result = NotificationResultType.Custom;
                                OnCloseCommand(null);
                            })
                        });
                    }
                    break;
            }
        }

        public string InputText
        {
            get => _inputText;
            set => Set(ref _inputText, value);
        }

        private void OnTextCommand(object obj)
        {
            NotificationResult.Result = NotificationResultType.Text;
            NotificationResult.Content = InputText;
            OnCloseCommand(null);
        }

        private void OnNoCommand(object obj)
        {
            NotificationResult.Result = NotificationResultType.No;
            OnCloseCommand(null);
        }

        private void OnYesCommand(object obj)
        {
            NotificationResult.Result = NotificationResultType.Yes;
            OnCloseCommand(null);
        }

        private void OnOkCommand(object obj)
        {
            NotificationResult.Result = NotificationResultType.Ok;
            OnCloseCommand(null);
        }

        private void SetColor()
        {
            var color = "NotificationBaseColor";

            switch (_icon)
            {
                case NotificationIconType.Success: color = "NotificationSuccessColor"; break;
                case NotificationIconType.Error: color = "NotificationErrorColor"; break;
                case NotificationIconType.Question: color = "NotificationQuestionColor"; break;
            }

            StatusColor = Application.Current.TryFindResource(color).ToString();
        }

        private void OnCloseCommand(object obj)
        {
            _closeAction?.Invoke();
        }

        public string Title { get; private set; }
        public string Icon { get; private set; }
        public string Content { get; private set; }
        public string StatusColor { get; private set; }

        public NotificationResult NotificationResult { get; internal set; } = new NotificationResult();
    }
}
