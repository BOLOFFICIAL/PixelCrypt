using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    internal class PasswordBoxViewModel : BaseViewModel
    {
        private string _password;
        private GridLength _hideWidth;
        private GridLength _visibleWidth;
        private bool isVisible = true;

        public ICommand ToggleVisibilityCommand { get; }

        public PasswordBoxViewModel()
        {
            ToggleVisibilityCommand = new LambdaCommand(OnToggleVisibilityCommand);
            OnToggleVisibilityCommand(null);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
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

        private void OnToggleVisibilityCommand(object obj)
        {
            isVisible = !isVisible;

            if (isVisible)
            {
                HideWidth = new GridLength(0, GridUnitType.Star);
                VisibleWidth = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                HideWidth = new GridLength(1, GridUnitType.Star);
                VisibleWidth = new GridLength(0, GridUnitType.Star);
            }
        }
    }
}
