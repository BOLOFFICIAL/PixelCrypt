using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    internal class PasswordBoxViewModel : BaseViewModel
    {
        private const int _passwordLength = 8;
        private const string _passwordAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";

        private string _password;
        private GridLength _hideWidth;
        private GridLength _visibleWidth;
        private GridLength _autoPasswordWidth;
        private bool _isVisible = true;
        private string _toggleVisibilityIcon;

        public ICommand ToggleVisibilityCommand { get; }
        public ICommand AutoPasswordCommand { get; }

        public PasswordBoxViewModel()
        {
            ToggleVisibilityCommand = new LambdaCommand(OnToggleVisibilityCommand);
            AutoPasswordCommand = new LambdaCommand(OnAutoPasswordCommand);
            OnToggleVisibilityCommand(null);
        }

        public string Password
        {
            get => _password;
            set
            {
                Set(ref _password, value);
                AutoPasswordWidth = string.IsNullOrEmpty(_password)
                    ? new GridLength(1, GridUnitType.Auto)
                    : new GridLength(0, GridUnitType.Pixel);
            }
        }

        public GridLength HideWidth
        {
            get => _hideWidth;
            set => Set(ref _hideWidth, value);
        }

        public GridLength VisibleWidth
        {
            get => _visibleWidth;
            set => Set(ref _visibleWidth, value);
        }

        public GridLength AutoPasswordWidth
        {
            get => _autoPasswordWidth;
            set => Set(ref _autoPasswordWidth, value);
        }

        public string ToggleVisibilityIcon
        {
            get => _toggleVisibilityIcon;
            set => Set(ref _toggleVisibilityIcon, value);
        }

        private void OnToggleVisibilityCommand(object obj)
        {
            _isVisible = !_isVisible;

            if (_isVisible)
            {
                HideWidth = new GridLength(0, GridUnitType.Star);
                VisibleWidth = new GridLength(1, GridUnitType.Star);
                ToggleVisibilityIcon = "Regular_Eye";
            }
            else
            {
                HideWidth = new GridLength(1, GridUnitType.Star);
                VisibleWidth = new GridLength(0, GridUnitType.Star);
                ToggleVisibilityIcon = "Regular_EyeSlash";
            }
        }

        private void OnAutoPasswordCommand(object obj)
        {
            var random = new Random();

            Password = new string(Enumerable.Repeat(_passwordAlphabet, _passwordLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
