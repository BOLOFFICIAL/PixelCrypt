using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View.Page;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Page
{
    internal class CryptographyPageViewModel : Base.ViewModel
    {
        private List<Model.Image> ContextImage = new List<Model.Image>();

        private string _showPasword = "";
        private string _password = "";

        private GridLength _closePasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _openPasswordWidth = new GridLength(0, GridUnitType.Pixel);

        private bool _isOpenPassword = false;

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand PaswordViewCommand { get; }

        public CryptographyPageViewModel()
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);

            OnPaswordViewCommandExecuted();
        }

        public string ShowPasword
        {
            get => _showPasword;
            set => Set(ref _showPasword, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public GridLength ClosePasswordWidth
        {
            get => _closePasswordWidth;
            set => Set(ref _closePasswordWidth, value);
        }

        public GridLength OpenPasswordWidth
        {
            get => _openPasswordWidth;
            set => Set(ref _openPasswordWidth, value);
        }

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnAddImageCommandExecuted(object p = null)
        {

        }

        private void OnPaswordViewCommandExecuted(object p = null)
        {
            if (_isOpenPassword)
            {
                OpenPasswordWidth = new GridLength(1, GridUnitType.Star);
                ClosePasswordWidth = new GridLength(0, GridUnitType.Star);
                ShowPasword = "Regular_Eye";
            }
            else
            {
                OpenPasswordWidth = new GridLength(0, GridUnitType.Star);
                ClosePasswordWidth = new GridLength(1, GridUnitType.Star);
                ShowPasword = "Regular_EyeSlash";
            }

            _isOpenPassword = !_isOpenPassword;
        }
    }
}
