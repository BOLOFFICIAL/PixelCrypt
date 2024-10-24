using FontAwesome5;
using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt.ViewModel.Page
{
    internal class TextInPicturePageViewModel : Base.ViewModel
    {
        private string _password = "";
        private string _showPasword = "";
        private string _imageData = "";
        private bool _isOpenPassword = false;
        private GridLength _closePasswordWidth;
        private GridLength _openPasswordWidth;
        private GridLength _choseImageWidth;
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imagesWidth = new GridLength(0, GridUnitType.Pixel);
        public string _filePathImage = "";
        public bool _isSuccessAction = false;
        private bool _isButtonFree = true;
        public ICommand ClosePageCommand { get; }
        public ICommand ShowPaswordCommand { get; }
        public ICommand ChoseImageCommand { get; set; }
        public ICommand DoActionCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        private ICommand ShowImageCommand { get; }
        private readonly int slash = 8;
        private string _actionButtonName = "";
        private string _filePathFile = "";
        private string _fileData = "";
        private string _importButtonBackgroundColor = "";
        private string _importButtonForegroundColor = "";
        private string _exportButtonBackgroundColor = "";
        private string _exportButtonForegroundColor = "";
        private string _actionButtonBackgroundColor = "";
        private string _actionButtonForegroundColor = "";
        private string _errorDoActionMessage = "";
        private string _errorSaveMessage = "";
        private bool _isImport = false;
        private bool _isFileDataReadonly = false;
        private bool _canDoAction = true;
        private GridLength _onePictureWidth;
        private GridLength _manyPictureWidth;
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);
        private List<string> _filePathImages = new List<string>();
        private List<Bitmap> _resultImages = new List<Bitmap>();
        private StackPanel _filePathImageStackPanel = new StackPanel();
        private int _selectedElementIndex = -1;
        public ICommand ActionCommand { get; }
        public ICommand SplitCommand { get; }
        public ICommand ClearPathFileCommand { get; }
        public ICommand ChoseFileCommand { get; }
        private ICommand RemoveImageCommand { get; }


        public TextInPicturePageViewModel()
        {
            ActionCommand = new LambdaCommand(OnActionCommandExecuted);
            SplitCommand = new LambdaCommand(OnSplitCommandExecuted);
            ClearPathFileCommand = new LambdaCommand(OnClearPathFileCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            ChoseFileCommand = new LambdaCommand(OnChoseFileCommandExecuted);
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted, CanChoseImageCommandExecute);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted, CanDoActionCommandExecute);
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            OnShowPaswordCommandExecuted(null);

            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);

            OnSplitCommandExecuted(null);
            OnActionCommandExecuted("Import");
        }

        public bool IsFileDataReadonly
        {
            get => _isFileDataReadonly;
            set => Set(ref _isFileDataReadonly, value);
        }

        private bool IsSuccessAction
        {
            get => _isSuccessAction;
            set => Set(ref _isSuccessAction, value);
        }

        public string ActionButtonName
        {
            get => _actionButtonName;
            set => Set(ref _actionButtonName, value);
        }

        public string FilePathFile
        {
            get => Path.GetFileName(_filePathFile);
            set => Set(ref _filePathFile, value);
        }

        public string FileData
        {
            get => _fileData;
            set => Set(ref _fileData, value);
        }

        public string ImportButtonBackgroundColor
        {
            get => _importButtonBackgroundColor;
            set => Set(ref _importButtonBackgroundColor, value);
        }

        public string ImportButtonForegroundColor
        {
            get => _importButtonForegroundColor;
            set => Set(ref _importButtonForegroundColor, value);
        }

        public string ExportButtonBackgroundColor
        {
            get => _exportButtonBackgroundColor;
            set => Set(ref _exportButtonBackgroundColor, value);
        }

        public string ExportButtonForegroundColor
        {
            get => _exportButtonForegroundColor;
            set => Set(ref _exportButtonForegroundColor, value);
        }

        public string ActionButtonBackgroundColor
        {
            get => _actionButtonBackgroundColor;
            set => Set(ref _actionButtonBackgroundColor, value);
        }

        public string ActionButtonForegroundColor
        {
            get => _actionButtonForegroundColor;
            set => Set(ref _actionButtonForegroundColor, value);
        }

        public GridLength OnePictureWidth
        {
            get => _onePictureWidth;
            set => Set(ref _onePictureWidth, value);
        }

        public GridLength ManyPictureWidth
        {
            get => _manyPictureWidth;
            set => Set(ref _manyPictureWidth, value);
        }

        public GridLength SaveButtonWidth
        {
            get => _saveButtonWidth;
            set => Set(ref _saveButtonWidth, value);
        }

        public GridLength ActionWidth
        {
            get => _actionWidth;
            set => Set(ref _actionWidth, value);
        }

        public GridLength ImagesWidth
        {
            get => _imagesWidth;
            set => Set(ref _imagesWidth, value);
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

        private void OnSplitCommandExecuted(object p = null)
        {
            OnePictureWidth = new GridLength(1, GridUnitType.Star);
            ManyPictureWidth = new GridLength(0, GridUnitType.Star);

            FilePathImageStackPanel = new StackPanel();

            if (_filePathImages.Count > 0)
            {
                ImageData = _filePathImages[0];
                FilePathImage = _filePathImages[0];
            }
            else
            {
                ImageData = "";
                FilePathImage = "";
            }
        }

        private void OnActionCommandExecuted(object p = null)
        {
            if (p is not string actionName) return;

            var value = _isImport;

            switch (actionName)
            {
                case "Import": ImportAction(); break;
                case "Export": ExportAction(); break;
            }

            _isSuccessAction = value == _isImport;
        }

        private void OnClearPathFileCommandExecuted(object p = null)
        {
            IsFileDataReadonly = false;
            FilePathFile = "";
        }

        private bool CanSaveCommandExecute(object arg)
        {
            var res = true;

            res = res && IsSuccessAction;

            if (_isImport)
            {
                res = res && _resultImages.Count > 0;
            }
            else
            {
                res = res && FilePathFile.Length > 0;

                res = res && FileData.Length > 0;
            }

            if (res)
            {
                SaveButtonWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
            }

            return res;
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            try
            {
                if (_isImport)
                {
                    if (Program.SaveDataToFolder(_filePathImages, _resultImages))
                    {
                        Notification.MakeMessage("Картинки сохранены", "Сохранение изображений");
                    }
                }
                else
                {
                    var message = Program.SaveDataToFile(_filePathFile, FileData) ? "Данные успешно сохранены" : "Не удалось сохранить данные";

                    Notification.MakeMessage(message, "Сохранение");
                }
            }
            catch (Exception)
            {
                Notification.MakeMessage("Возникла ошибка при сохранении", "Сохранение");
            }
        }

        public void OnChoseFileCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (_isImport)
            {
                openFileDialog.Title = "Выберите файл для чтения данных";

                if (openFileDialog.ShowDialog() ?? false)
                {
                    if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && Notification.MakeMessage("Заменить текст на данные из файла?", "Файл для чтения данных", NotificationButton.YesNo) == NotificationResult.Yes))
                    {
                        IsFileDataReadonly = true;

                        FilePathFile = openFileDialog.FileName;

                        FileData = File.ReadAllText(_filePathFile);
                    }
                }
            }
            else
            {
                openFileDialog.Title = "Выберите файл для записи данных";

                if (openFileDialog.ShowDialog() ?? false)
                {
                    string content = File.ReadAllText(openFileDialog.FileName);

                    if (content.Length == 0 || content.Length > 0 && Notification.MakeMessage("Файл содержит данные которые будут перезаписаны. Продолжить?", "Файл для записи данных", NotificationButton.YesNo) == NotificationResult.Yes)
                    {
                        FilePathFile = openFileDialog.FileName;
                    }
                }
            }
        }

        private bool CanChoseImageCommandExecute(object arg)
        {
            var res = true;

            if (FilePathImage.Length > 0)
            {
                ChoseImageWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                ChoseImageWidth = new GridLength(0, GridUnitType.Pixel);
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

            if (_isImport)
            {
                openFileDialog.Title = "Выбрать изображение для импорта данных";
            }
            else
            {
                openFileDialog.Title = "Выбрать изображение для экспорта данных";
            }

            if (openFileDialog.ShowDialog() ?? false)
            {
                _isSuccessAction = false;

                foreach (var file in openFileDialog.FileNames)
                {
                    if (!_filePathImages.Contains(file))
                    {
                        _filePathImages.Add(file);
                    }
                }

                ImagesWidth = new GridLength(1, GridUnitType.Star);
                ActionWidth = new GridLength(1, GridUnitType.Star);

                _selectedElementIndex = _selectedElementIndex == -1 ? _filePathImages.Count - 1 : _selectedElementIndex;

                ImageData = _filePathImages[_selectedElementIndex];

                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        public async void OnDoActionCommandExecuted(object p = null)
        {
            if (!_canDoAction) return;

            _isSuccessAction = false;
            IsButtonFree = false;
            FilePathImageStackPanel = LoadFilePathImages();

            if (_isImport) { await ImportData(); }
            else { await ExportData(); }

            IsButtonFree = true;

            if (_isSuccessAction)
            {
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages.Count);
            }
            else
            {
                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        private bool CanDoActionCommandExecute(object arg)
        {
            _canDoAction = true;

            if (_isImport)
            {
                _canDoAction = _canDoAction && FileData.Length > 0;
            }

            if (_canDoAction)
            {
                ActionButtonBackgroundColor = Color2;
                ActionButtonForegroundColor = Color3;
            }
            else
            {
                ActionButtonBackgroundColor = Color4;
                ActionButtonForegroundColor = Color3;
            }

            return true;
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
                else
                {
                    ImagesWidth = new GridLength(0, GridUnitType.Pixel);
                    ActionWidth = new GridLength(0, GridUnitType.Pixel);
                }
            }

            if (_isSuccessAction)
            {
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages.Count);
            }
            else
            {
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

        public void OnShowImageCommandExecuted(object p = null)
        {
            int index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1 || _selectedElementIndex == index) return;

            _selectedElementIndex = index;

            ImageData = _filePathImages[_selectedElementIndex];

            if (_isSuccessAction)
            {
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages.Count);
            }
            else
            {
                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        private void ImportAction()
        {
            ActionButtonName = "Импортировать";

            ImportButtonBackgroundColor = Color2;
            ImportButtonForegroundColor = Color3;
            ExportButtonBackgroundColor = Color4;
            ExportButtonForegroundColor = Color3;

            _isImport = true;
            if (FilePathFile.Length > 0)
            {
                if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && Notification.MakeMessage("Заменить текст на данные из файла?", "Файл для чтения данных", NotificationButton.YesNo) == NotificationResult.Yes))
                {
                    IsFileDataReadonly = true;

                    FilePathFile = _filePathFile;

                    FileData = File.ReadAllText(_filePathFile);
                }
                else
                {
                    FilePathFile = "";
                }
            }
        }

        private void ExportAction()
        {
            ActionButtonName = "Экспортировать";

            ExportButtonBackgroundColor = Color2;
            ExportButtonForegroundColor = Color3;
            ImportButtonBackgroundColor = Color4;
            ImportButtonForegroundColor = Color3;

            if (_filePathFile != null && _filePathFile.Length > 0)
            {
                string content = File.ReadAllText(_filePathFile);

                if (content.Length > 0 && Notification.MakeMessage("файл содержит данные которые будут перезаписаны. Оставить выбранный файл?", "Файл для записи данных", NotificationButton.YesNo) == NotificationResult.Yes)
                {
                    FilePathFile = _filePathFile;
                }
                else
                {
                    FilePathFile = "";
                }
            }

            _isImport = false;
            IsFileDataReadonly = false;
        }

        private StackPanel LoadFilePathImages(int count = 0)
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

                var imageName = new TextBlock()
                {
                    Text = Path.GetFileName(image),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                };

                var button = new Button()
                {
                    Command = ShowImageCommand,
                    CommandParameter = index,
                    Height = double.NaN,
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
                    Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color4),
                    BorderBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
                    BorderThickness = new Thickness(2,1,2,1)
                };

                if (index == _selectedElementIndex)
                {
                    button.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3);
                    button.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color4);
                    button.BorderThickness = new Thickness(0);
                }

                button.Content = imageName;

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

                if (index < count)
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

        private async Task ExportData()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            var BynaryData = new List<string>();

            FileData = "";

            try
            {
                foreach (var filePathImage in _filePathImages)
                {
                    var exportDataImage = await ImageHelper.ExportDataFromImage(filePathImage);
                    BynaryData.Add(exportDataImage);
                    FilePathImageStackPanel = LoadFilePathImages(BynaryData.Count);
                }

                var allData = new StringBuilder();

                foreach (var item in BynaryData)
                {
                    allData.Append(item);
                }

                var exportData = Converter.ConvertBinaryStringToText(allData.ToString());

                exportData = Cryptography.DecryptText(exportData, hashPassword);

                _isSuccessAction = true;

                Notification.MakeMessage("Данные экспортированы", "Экспорт данных");

                FileData = exportData;
            }
            catch
            {
                Notification.MakeMessage($"Не удалось экспортировать данные");
                _isSuccessAction = false;
                FilePathImageStackPanel = LoadFilePathImages();
            }
        }

        private async Task ImportData()
        {
            try
            {
                var inportData = FileData;

                var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

                inportData = Cryptography.EncryptText(inportData, hashPassword);

                string binary = Converter.ConvertTextToBinaryString(inportData);

                _resultImages = new List<Bitmap>();

                var lines = Program.SplitStringIntoParts(binary, _filePathImages.Count);

                for (int i = 0; i < _filePathImages.Count; i++)
                {
                    var importDataImage = await ImageHelper.ImportDataToImage(lines[i], _filePathImages[i]);
                    _resultImages.Add(importDataImage);
                    FilePathImageStackPanel = LoadFilePathImages(_resultImages.Count);
                }

                _isSuccessAction = true;

                Notification.MakeMessage("Данные импортированы", "Испорт данных");
            }
            catch
            {
                _resultImages = new List<Bitmap>();
                Notification.MakeMessage($"Не удалось импортировать данные");
                _isSuccessAction = false;
                FilePathImageStackPanel = LoadFilePathImages();
            }
        }
    }
}
