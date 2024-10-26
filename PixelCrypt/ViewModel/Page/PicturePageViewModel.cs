using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using PixelCrypt.View.Page;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Page
{
    internal class PicturePageViewModel : Base.ViewModel
    {
        private string _password = "";
        private string _showPasword = "";
        private string _imageData = "";
        private bool _isOpenPassword = false;
        public string _filePathImage = "";
        public bool _isSuccessAction = false;
        private bool _isButtonFree = true;
        private bool _canBack = true;
        public ICommand ClosePageCommand { get; }
        public ICommand ShowPaswordCommand { get; }
        public ICommand ChoseImageCommand { get; set; }
        public ICommand DoActionCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        private ICommand RemoveImageCommand { get; }
        private ICommand ShowImageCommand { get; }
        public ICommand ClearAllCommand { get; }
        private StackPanel _filePathImageStackPanel = new StackPanel();
        private List<string> _filePathImages = new List<string>();
        private List<Bitmap> _resultImages = new List<Bitmap>();
        private GridLength _resultImageWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imagesWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imageResultHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imageResultWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _closePasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _openPasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _choseImageWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _clearWidth = new GridLength(0, GridUnitType.Pixel);
        private int _selectedElementIndex = -1;
        public System.Windows.Controls.Image ResultHeightImage { get; set; }
        public System.Windows.Controls.Image ResultWidthImage { get; set; }

        public PicturePageViewModel()
        {
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted, CanChoseImageCommandExecute);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);
            ClearAllCommand = new LambdaCommand(OnClearAllCommandExecuted, CanClearAllCommandExecute);

            OnShowPaswordCommandExecuted(null);
        }

        public string PageTitle => "Cryptography";

        public GridLength ResultImageWidth
        {
            get => _resultImageWidth;
            set => Set(ref _resultImageWidth, value);
        }

        public GridLength ImagesWidth
        {
            get => _imagesWidth;
            set => Set(ref _imagesWidth, value);
        }

        public GridLength ImageResultHeight
        {
            get => _imageResultHeight;
            set => Set(ref _imageResultHeight, value);
        }

        public GridLength ImageResultWidth
        {
            get => _imageResultWidth;
            set => Set(ref _imageResultWidth, value);
        }

        public GridLength ActionWidth
        {
            get => _actionWidth;
            set => Set(ref _actionWidth, value);
        }

        public GridLength SaveButtonWidth
        {
            get => _saveButtonWidth;
            set => Set(ref _saveButtonWidth, value);
        }

        public StackPanel FilePathImageStackPanel
        {
            get => _filePathImageStackPanel;
            set => Set(ref _filePathImageStackPanel, value);
        }

        public bool IsButtonFree
        {
            get => _isButtonFree;
            set => Set(ref _isButtonFree, value);
        }

        public bool CanBack
        {
            get => _canBack;
            set => Set(ref _canBack, value);
        }

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

        public GridLength ClearWidth
        {
            get => _clearWidth;
            set => Set(ref _clearWidth, value);
        }

        private bool CanChoseImageCommandExecute(object arg)
        {
            var res = true;

            if (_filePathImages.Count > 0)
            {
                ActionWidth = new GridLength(1, GridUnitType.Star);
                ImagesWidth = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                ActionWidth = new GridLength(0, GridUnitType.Pixel);
                ImagesWidth = new GridLength(0, GridUnitType.Pixel);
            }

            return res;
        }

        public void OnChoseImageCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
                Multiselect = true,
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                var prefCount = _filePathImages.Count;

                foreach (var file in openFileDialog.FileNames)
                {
                    if (!_filePathImages.Contains(file))
                    {
                        _filePathImages.Add(file);
                    }
                }

                if (prefCount!= _filePathImages.Count) 
                {
                    _selectedElementIndex = _selectedElementIndex == -1 ? _filePathImages.Count - 1 : _selectedElementIndex;

                    ImageData = _filePathImages[_selectedElementIndex];

                    FilePathImage = ImageData;

                    ActionWidth = new GridLength(1, GridUnitType.Star);
                    ImagesWidth = new GridLength(1, GridUnitType.Star);
                    SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                    ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                    ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                    ResultImageWidth = SaveButtonWidth;

                    _isSuccessAction = false;
                    _resultImages = new List<Bitmap>();

                    FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
                }
            }
        }

        public async void OnDoActionCommandExecuted(object p = null)
        {
            try
            {
                if (p is not string action) return;

                IsButtonFree = false;
                CanBack = false;
                _isSuccessAction = false;
                _resultImages.Clear();
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                ResultImageWidth = SaveButtonWidth;

                switch (action)
                {
                    case "Encrypt": await Encrypt(); break;
                    case "Decrypt": await Decrypt(); break;
                }

                IsButtonFree = true;

                FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);

                UpdateResultImage();

                if (_isSuccessAction)
                {
                    SaveButtonWidth = new GridLength(1, GridUnitType.Auto);
                    ResultImageWidth = new GridLength(1, GridUnitType.Star);
                }
            }
            catch (Exception ex)
            {
                Notification.MakeMessage("Не удалось выполнить действие");

                IsButtonFree = true;

                FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
            }
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            var title = "Сохранение изображений";
            try
            {
                if (Program.SaveDataToFolder(_filePathImages, _resultImages))
                {
                    Notification.MakeMessage("Картинки сохранены", title);
                }
            }
            catch (Exception)
            {
                Notification.MakeMessage("Возникла ошибка при сохранении", title);
            }
        }

        private async Task Decrypt()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            CanBack = true;

            var message = "Все картинки расшифрованы";
            var title = "Расшифровывание данных";

            try
            {
                foreach (var file in _filePathImages)
                {
                    var decryptPhoto = await Cryptography.DecryptPhoto(file, hashPassword);
                    _resultImages.Add(decryptPhoto);
                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(PicturePage) && Context.MainWindow.IsActive)
                    {
                        FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);
                    }
                }

                _isSuccessAction = true;
            }
            catch
            {
                message = "Не удалось расшифровать картинки";
                _isSuccessAction = false;
                _resultImages.Clear();
            }
            finally
            {
                DoNotification(message, title, typeof(PicturePage), PageTitle);
            }
        }

        private async Task Encrypt()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            CanBack = true;

            var message = "Все данные зашифрованы";
            var title = "Шифрование данных";

            try
            {
                foreach (var file in _filePathImages)
                {
                    var encryptPhoto = await Cryptography.EncryptPhoto(file, hashPassword);
                    _resultImages.Add(encryptPhoto);
                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(PicturePage) && Context.MainWindow.IsActive)
                    {
                        FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);
                    }
                }

                _isSuccessAction = true;
            }
            catch (Exception ex)
            {
                message = "Не удалось зашифровать данные";
                _isSuccessAction = false;
                _resultImages.Clear();
            }
            finally
            {
                DoNotification(message, title, typeof(PicturePage), PageTitle);
            }
        }

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnShowPaswordCommandExecuted(object p = null)
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

        public void OnRemoveImageCommandExecuted(object p = null)
        {
            int index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1) return;

            _filePathImages.RemoveAt(index);

            if (index <= _selectedElementIndex)
            {
                _selectedElementIndex--;

                if (_filePathImages.Count > 0)
                {
                    if (_selectedElementIndex == -1)
                    {
                        _selectedElementIndex = 0;
                    }

                    ImageData = _filePathImages[_selectedElementIndex];
                }
            }

            if (_isSuccessAction)
            {
                _resultImages.RemoveAt(index);

                if (_filePathImages.Count > 0)
                {
                    UpdateResultImage();
                }
            }

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);
        }

        public void OnShowImageCommandExecuted(object p = null)
        {
            int index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1 || _selectedElementIndex == index || !IsButtonFree) return;

            _selectedElementIndex = index;

            ImageData = _filePathImages[_selectedElementIndex];

            if (_isSuccessAction)
            {
                UpdateResultImage();
            }

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);
        }

        public void OnClearAllCommandExecuted(object p = null)
        {
            ActionWidth = new GridLength(0, GridUnitType.Pixel);
            ImagesWidth = new GridLength(0, GridUnitType.Pixel);

            _filePathImages.Clear();
            _resultImages.Clear();

            _selectedElementIndex = -1;
            FilePathImageStackPanel = new StackPanel();
        }

        private bool CanClearAllCommandExecute(object arg)
        {
            if (_filePathImages.Count > 1)
            {
                ClearWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                ClearWidth = new GridLength(0, GridUnitType.Pixel);
            }

            return true;
        }

        public void InitializeImage()
        {
            if (_isSuccessAction == false || _selectedElementIndex == -1) return;

            UpdateResultImage();
        }

        private void UpdateResultImage()
        {
            var source = _resultImages[_selectedElementIndex];

            if (source.Width > source.Height)
            {
                ImageResultHeight = new GridLength(1, GridUnitType.Star);
                ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                ResultHeightImage.Source = Converter.ConvertBitmapToImageSource(source);
            }
            else
            {
                ImageResultWidth = new GridLength(1, GridUnitType.Star);
                ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                ResultWidthImage.Source = Converter.ConvertBitmapToImageSource(source);
            }
        }
    }
}
