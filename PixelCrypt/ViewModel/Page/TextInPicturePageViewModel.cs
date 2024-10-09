using FontAwesome5;
using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt.ViewModel.Page
{
    internal class TextInPicturePageViewModel : Base.ViewModel
    {
        #region Fields

        #region Propertyes

        #region Private

        private string _actionButtonName = "";
        private string _filePathFile = "";
        private string _filePathImage = "";
        private string _password = "";
        private string _fileData = "";
        private string _imageData = "";
        private string _showPasword = "";
        private string _importButtonBackgroundColor = "";
        private string _importButtonForegroundColor = "";
        private string _exportButtonBackgroundColor = "";
        private string _exportButtonForegroundColor = "";
        private string _splitButtonBackgroundColor = "";
        private string _splitButtonForegroundColor = "";
        private string _actionButtonBackgroundColor = "";
        private string _actionButtonForegroundColor = "";
        private string _errorDoActionMessage = "";
        private string _errorSaveMessage = "";

        private bool _isSplit = false;
        private bool _isImport = false;
        private bool _isClosePassword = false;
        private bool _isFileDataReadonly = false;

        private GridLength _onePictureWidth;
        private GridLength _manyPictureWidth;
        private GridLength _closePasswordWidth;
        private GridLength _openPasswordWidth;
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);

        private List<string> _filePathImages = new List<string>();

        private StackPanel _filePathImageStackPanel = new StackPanel();

        #endregion

        #endregion

        #region Commands

        #region Public

        public ICommand ClosePageCommand { get; }
        public ICommand ActionCommand { get; }
        public ICommand SplitCommand { get; }
        public ICommand ClearPathFileCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ShowPaswordCommand { get; }
        public ICommand ChoseFileCommand { get; }
        public ICommand ChoseImageCommand { get; }
        public ICommand DoActionCommand { get; }

        #endregion

        #region Private

        private ICommand RemoveImageCommand { get; }

        #endregion

        #endregion

        #endregion

        public TextInPicturePageViewModel()
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ActionCommand = new LambdaCommand(OnActionCommandExecuted);
            SplitCommand = new LambdaCommand(OnSplitCommandExecuted);
            ClearPathFileCommand = new LambdaCommand(OnClearPathFileCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);
            ChoseFileCommand = new LambdaCommand(OnChoseFileCommandExecuted);
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted, CanDoActionCommandExecute);

            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);

            OnSplitCommandExecuted(null);
            OnShowPaswordCommandExecuted(null);
            OnActionCommandExecuted("Import");
        }

        #region Propertyes

        public bool IsFileDataReadonly
        {
            get => _isFileDataReadonly;
            set => Set(ref _isFileDataReadonly, value);
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

        public string FilePathImage
        {
            get => Path.GetFileName(_filePathImage);
            set => Set(ref _filePathImage, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string FileData
        {
            get => _fileData;
            set => Set(ref _fileData, value);
        }

        public string ImageData
        {
            get => _imageData;
            set => Set(ref _imageData, value);
        }

        public string ShowPasword
        {
            get => _showPasword;
            set => Set(ref _showPasword, value);
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

        public string SplitButtonBackgroundColor
        {
            get => _splitButtonBackgroundColor;
            set => Set(ref _splitButtonBackgroundColor, value);
        }

        public string SplitButtonForegroundColor
        {
            get => _splitButtonForegroundColor;
            set => Set(ref _splitButtonForegroundColor, value);
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

        #endregion

        #region Commands

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnSplitCommandExecuted(object p = null)
        {
            if (_isSplit)
            {
                OnePictureWidth = new GridLength(0, GridUnitType.Star);
                ManyPictureWidth = new GridLength(1, GridUnitType.Star);

                SplitButtonBackgroundColor = "#000000";
                SplitButtonForegroundColor = "#FFFFFF";

                if (ImageData?.Length > 0)
                {
                    if (_filePathImages != null && _filePathImages.Count == 0)
                    {
                        _filePathImages.Add(ImageData);
                    }
                    else if (_filePathImages == null)
                    {
                        _filePathImages = new List<string>()
                        {
                            ImageData
                        };
                    }
                    else if (_filePathImages != null && _filePathImages.Count > 0)
                    {
                        _filePathImages[0] = ImageData;
                    }
                }
                else
                {
                    _filePathImages = new List<string>();
                }

                FilePathImageStackPanel = LoadFilePathImages();
            }
            else
            {
                OnePictureWidth = new GridLength(1, GridUnitType.Star);
                ManyPictureWidth = new GridLength(0, GridUnitType.Star);

                SplitButtonBackgroundColor = "#FFFFFF";
                SplitButtonForegroundColor = "#000000";

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

            _isSplit = !_isSplit;
        }

        private void OnActionCommandExecuted(object p = null)
        {
            if (p is not string actionName) return;

            switch (actionName)
            {
                case "Import": ImportAction(); break;
                case "Export": ExportAction(); break;
            }
        }

        private void OnClearPathFileCommandExecuted(object p = null)
        {
            IsFileDataReadonly = false;
            FilePathFile = "";
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            if (_isImport)
            {

            }
            else
            {

            }
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

        public void OnChoseFileCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (_isImport)
            {
                openFileDialog.Title = "Выберите фаил для чтения данных";

                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() ?? false)
                {
                    IsFileDataReadonly = true;

                    if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "Фаил для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    {
                        FilePathFile = openFileDialog.FileName;

                        FileData = File.ReadAllText(_filePathFile);
                    }
                }
            }
            else
            {
                openFileDialog.Title = "Выберите фаил для записи данных";

                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() ?? false)
                {
                    string content = File.ReadAllText(openFileDialog.FileName);

                    if (content.Length > 0 && MessageBox.Show("Фаил содержит данные которые будут перезаписаны. Продолжить?", "Фаил для записи данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        FilePathFile = openFileDialog.FileName;
                    }
                }
            }
        }

        public void OnChoseImageCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
            };
            if (!_isSplit)
            {
                openFileDialog.Multiselect = true;
            }

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
                if (_isSplit)
                {
                    ImageData = openFileDialog.FileNames[0];

                    FilePathImage = ImageData;
                }
                else
                {
                    foreach (var file in openFileDialog.FileNames)
                    {
                        if (!_filePathImages.Contains(file))
                        {
                            _filePathImages.Add(file);
                        }
                    }

                    FilePathImageStackPanel = LoadFilePathImages();
                }
            }
        }

        public void OnDoActionCommandExecuted(object p = null)
        {
            SaveButtonWidth = new GridLength(1, GridUnitType.Auto);
        }

        private bool CanDoActionCommandExecute(object arg)
        {
            return CanDoAction();
        }

        public void OnRemoveImageCommandExecuted(object p = null)
        {
            int index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1) return;

            _filePathImages.RemoveAt(index);

            FilePathImageStackPanel = LoadFilePathImages();
        }

        #endregion

        #region Methods

        private void ImportAction()
        {
            ActionButtonName = "Импортировать";

            ImportButtonBackgroundColor = "#000000";
            ImportButtonForegroundColor = "#FFFFFF";
            ExportButtonBackgroundColor = ImportButtonForegroundColor;
            ExportButtonForegroundColor = ImportButtonBackgroundColor;

            _isImport = true;
            if (FilePathFile.Length > 0)
            {
                if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "Фаил для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
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

            ExportButtonBackgroundColor = "#000000";
            ExportButtonForegroundColor = "#FFFFFF";
            ImportButtonBackgroundColor = ExportButtonForegroundColor;
            ImportButtonForegroundColor = ExportButtonBackgroundColor;

            _isImport = false;
            IsFileDataReadonly = false;
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

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                var textBlock = new TextBlock
                {
                    Text = image,
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10, 5, 10, 5)
                };

                Grid.SetColumn(border, 0);

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
                        Height = 25
                    },
                    Command = RemoveImageCommand,
                    CommandParameter = index,
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.Transparent,
                };

                Grid.SetColumn(deleteButton, 1);

                border.Child = textBlock;
                grid.Children.Add(border);
                grid.Children.Add(deleteButton);
                stackPanel.Children.Add(grid);

                index++;
            }

            return stackPanel;
        }

        private bool CanDoAction()
        {
            return true;
        }

        #endregion
    }
}
