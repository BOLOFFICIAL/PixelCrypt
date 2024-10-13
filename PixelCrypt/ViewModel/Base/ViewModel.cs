using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        private string _password = "";
        private string _showPasword = "";
        private string _imageData = "";

        private bool _isClosePassword = false;

        private GridLength _closePasswordWidth;
        private GridLength _openPasswordWidth;
        private GridLength _choseImageWidth;

        private Visibility _choseImageVisibility;

        public string _filePathImage = "";

        public bool _isSuccessAction = false;

        public ICommand ClosePageCommand { get; }
        public ICommand ShowPaswordCommand { get; }
        public ICommand ChoseImageCommand { get; set; }
        public ICommand DoActionCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModel() 
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);

            OnShowPaswordCommandExecuted(null);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        public string Color1 => "#1E1E1E";
        public string Color2 => "#8B0000";
        public string Color3 => "#FFFFFF";
        public string Color4 => "#303336";

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string ShowPasword
        {
            get => _showPasword;
            set => Set(ref _showPasword, value);
        }

        public string ImageData
        {
            get => _imageData;
            set => Set(ref _imageData, value);
        }

        public string FilePathImage
        {
            get => Path.GetFileName(_filePathImage);
            set => Set(ref _filePathImage, value);
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

        public GridLength ChoseImageWidth
        {
            get => _choseImageWidth;
            set => Set(ref _choseImageWidth, value);
        }

        public Visibility ChoseImageVisibility
        {
            get => _choseImageVisibility;
            set => Set(ref _choseImageVisibility, value);
        }

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnShowPaswordCommandExecuted(object p = null)
        {
            if (_isClosePassword)
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

            _isClosePassword = !_isClosePassword;
        }
    }
}
