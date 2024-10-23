using FontAwesome5;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public ICommand ClosePageCommand { get; }
        public ICommand ShowPaswordCommand { get; }
        public ICommand ChoseImageCommand { get; set; }
        public ICommand DoActionCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        private ICommand RemoveImageCommand { get; }
        private ICommand ShowImageCommand { get; }
        private StackPanel _filePathImageStackPanel = new StackPanel();
        private List<string> _filePathImages = new List<string>();
        private List<BitmapImage> _resultImages = new List<BitmapImage>();
        private GridLength _resultImageWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imagesWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imageResultHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imageResultWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _closePasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _openPasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _choseImageWidth = new GridLength(0, GridUnitType.Pixel);
        private int _selectedElementIndex = -1;

        public PicturePageViewModel()
        {
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted, CanChoseImageCommandExecute);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            OnShowPaswordCommandExecuted(null);
        }

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
                foreach (var file in openFileDialog.FileNames)
                {
                    if (!_filePathImages.Contains(file))
                    {
                        _filePathImages.Add(file);
                    }
                }

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
                _resultImages = new List<BitmapImage>();

                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        public async void OnDoActionCommandExecuted(object p = null)
        {
            try
            {
                if (p is not string action) return;

                IsButtonFree = false;
                _isSuccessAction = false;
                _resultImages.Clear();
                FilePathImageStackPanel = LoadFilePathImages();
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

                FilePathImageStackPanel = LoadFilePathImages();

                var source = _resultImages[_selectedElementIndex];

                if (source.Width > source.Height)
                {
                    ImageResultHeight = new GridLength(1, GridUnitType.Star);
                    ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                    Context.ResultImageHeight.Source = source;
                }
                else
                {
                    ImageResultWidth = new GridLength(1, GridUnitType.Star);
                    ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                    Context.ResultImageWidth.Source = source;
                }

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

                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            try
            {
                CommonOpenFileDialog folderPicker = new CommonOpenFileDialog();

                folderPicker.IsFolderPicker = true;
                folderPicker.Title = "Выбор папки для хранения данных";
                var now = DateTime.Now;

                var folder = Path.Combine(Path.GetDirectoryName(_filePathImages[0]), $"PixelCrypt_{now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "")}");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                folderPicker.InitialDirectory = folder;

                CommonFileDialogResult dialogResult = folderPicker.ShowDialog();

                if (dialogResult == CommonFileDialogResult.Ok)
                {
                    var image = new System.Windows.Controls.Image();
                    for (int i = 0; i < _resultImages.Count; i++)
                    {
                        var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(_filePathImages[i]) + $"_PixelCrypt_{now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "")}");
                        var format = ImageFormat.Png;

                        format = ImageFormat.Png;
                        name += ".png";
                        image.Source = _resultImages[i];
                        Converter.ConvertImageToBitmap(image).Save(name, format);
                    }

                    Notification.MakeMessage("Данные сохранены", "Сохранение данных");
                }
            }
            catch (Exception)
            {
                Notification.MakeMessage("Возникла ошибка при сохранении", "Сохранение");
            }
        }

        private async Task Decrypt()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            try
            {
                foreach (var file in _filePathImages)
                {
                    var decryptPhoto = await Cryptography.DecryptPhoto(file, hashPassword);

                    _resultImages.Add(decryptPhoto);

                    FilePathImageStackPanel = LoadFilePathImages();
                }

                Notification.MakeMessage("Все картинки расшифрованы");

                _isSuccessAction = true;
            }
            catch
            {
                Notification.MakeMessage("Не удалось расшифровать картинки");
                _isSuccessAction = false;
            }
        }

        private async Task Encrypt()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            try
            {
                foreach (var file in _filePathImages)
                {
                    var encryptPhoto = await Cryptography.EncryptPhoto(file, hashPassword);

                    _resultImages.Add(encryptPhoto);

                    FilePathImageStackPanel = LoadFilePathImages();
                }

                FilePathImageStackPanel = LoadFilePathImages();

                Notification.MakeMessage("Все картинки успешно зашифрованы");

                _isSuccessAction = true;
            }
            catch (Exception ex)
            {
                Notification.MakeMessage("Не удалось зашифровать картинку");
                _isSuccessAction = false;
                _resultImages.Clear();
                FilePathImageStackPanel = LoadFilePathImages();
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
                    var source = _resultImages[_selectedElementIndex];

                    if (source.Width > source.Height)
                    {
                        ImageResultHeight = new GridLength(1, GridUnitType.Star);
                        ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                        Context.ResultImageHeight.Source = source;
                    }
                    else
                    {
                        ImageResultWidth = new GridLength(1, GridUnitType.Star);
                        ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                        Context.ResultImageWidth.Source = source;
                    }
                }
            }

            FilePathImageStackPanel = LoadFilePathImages();
        }

        public void OnShowImageCommandExecuted(object p = null)
        {
            int index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1 || _selectedElementIndex == index) return;

            _selectedElementIndex = index;

            ImageData = _filePathImages[_selectedElementIndex];

            if (_isSuccessAction)
            {
                var source = _resultImages[_selectedElementIndex];

                if (source.Width > source.Height)
                {
                    ImageResultHeight = new GridLength(1, GridUnitType.Star);
                    ImageResultWidth = new GridLength(0, GridUnitType.Pixel);
                    Context.ResultImageHeight.Source = source;
                }
                else
                {
                    ImageResultWidth = new GridLength(1, GridUnitType.Star);
                    ImageResultHeight = new GridLength(0, GridUnitType.Pixel);
                    Context.ResultImageWidth.Source = source;
                }
            }

            FilePathImageStackPanel = LoadFilePathImages();
        }

        private StackPanel LoadFilePathImages()
        {
            var stackPanel = new StackPanel();
            int index = 0;

            foreach (var image in _filePathImages)
            {
                var grid = new Grid()
                {
                    Margin = new Thickness(0, 10, 0, 0)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                var icon = new ImageAwesome
                {
                    Icon = EFontAwesomeIcon.Regular_CheckCircle,
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(0, 0, 10, 0),
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color5)
                };

                Grid.SetColumn(icon, 0);

                var button = new Button()
                {
                    Content = Path.GetFileName(image),
                    Command = ShowImageCommand,
                    CommandParameter = index,
                    FontSize = 15,
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
                    Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color4),
                    BorderBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
                    BorderThickness = new Thickness(2)
                };

                if (index == _selectedElementIndex)
                {
                    button.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3);
                    button.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color4);
                    button.BorderThickness = new Thickness(0);
                }

                Grid.SetColumn(button, 1);

                var deleteButton = new Button
                {
                    Margin = new Thickness(5, 0, 0, 0),
                    Width = 35,
                    Height = 35,
                    Padding = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Content = new ImageAwesome
                    {
                        Icon = EFontAwesomeIcon.Regular_TimesCircle,
                        Width = 25,
                        Height = 25,
                        Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3)
                    },
                    Command = RemoveImageCommand,
                    IsEnabled = IsButtonFree,
                    CommandParameter = index,
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.Transparent,
                };

                Grid.SetColumn(deleteButton, 2);

                if (index < _resultImages.Count)
                {
                    grid.Children.Add(icon);
                }

                grid.Children.Add(button);
                grid.Children.Add(deleteButton);
                stackPanel.Children.Add(grid);

                index++;
            }

            return stackPanel;
        }
    }
}
