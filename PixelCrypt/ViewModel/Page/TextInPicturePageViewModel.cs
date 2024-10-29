using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using PixelCrypt.View.Page;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _imagesWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _clearWidth = new GridLength(0, GridUnitType.Pixel);
        public bool _isSuccessAction = false;
        private string[] _exportFileData = new string[0];
        private bool _isButtonFree = true;
        private bool _canBack = true;
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
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);
        private List<string> _filePathImages = new List<string>();
        private List<Bitmap> _resultImages = new List<Bitmap>();
        private List<string> _bynaryData = new List<string>();
        private StackPanel _filePathImageStackPanel = new StackPanel();
        private int _selectedElementIndex = -1;
        public ICommand ActionCommand { get; }
        public ICommand ClearPathFileCommand { get; }
        public ICommand ChoseFileCommand { get; }
        public ICommand ClearAllCommand { get; }
        private ICommand RemoveImageCommand { get; }

        public TextInPicturePageViewModel()
        {
            ActionCommand = new LambdaCommand(OnActionCommandExecuted);
            ClearPathFileCommand = new LambdaCommand(OnClearPathFileCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);
            ChoseFileCommand = new LambdaCommand(OnChoseFileCommandExecuted);
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ShowPaswordCommand = new LambdaCommand(OnShowPaswordCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);
            ClearAllCommand = new LambdaCommand(OnClearAllCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);

            OnShowPaswordCommandExecuted();
            OnActionCommandExecuted("Import");

            ActionButtonBackgroundColor = Color2;
            ActionButtonForegroundColor = Color3;
        }

        public string PageTitle => "Steganography";

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

        public GridLength ClearWidth
        {
            get => _clearWidth;
            set => Set(ref _clearWidth, value);
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

        private void OnActionCommandExecuted(object p = null)
        {
            if (p is not string actionName) return;

            if (_isSuccessAction && Notification.MakeMessage("Смена режима приведет к потере прогресса.\nПродолжить?", NotificationButton.YesNo) != NotificationResult.Yes) return;

            switch (actionName)
            {
                case "Import": ImportAction(); break;
                case "Export": ExportAction(); break;
            }

            _resultImages.Clear();
            _bynaryData.Clear();
            _isSuccessAction = false;
            UpdateSaveWidth();

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
        }

        private void OnClearPathFileCommandExecuted(object p = null)
        {
            IsFileDataReadonly = false;
            FilePathFile = "";
            UpdateSaveWidth();
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            try
            {
                if (_isImport)
                {
                    if (Program.SaveBitmapToFolder(_filePathImages, _resultImages))
                    {
                        Notification.MakeMessage("Картинки сохранены", "Сохранение изображений");
                    }
                }
                else
                {
                    var message = "Нет данных для сохранения";

                    if (FileData.Length > 0)
                    {
                        message = Program.SaveDataToFile(_filePathFile, FileData) ? "Данные успешно сохранены" : "Не удалось сохранить данные";
                    }

                    Notification.MakeMessage(message, "Сохранение данных");
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
                openFileDialog.Title = "Выбрать файл для чтения данных";

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
                openFileDialog.Title = "Выбрать файл для записи данных";

                if (openFileDialog.ShowDialog() ?? false)
                {
                    string content = File.ReadAllText(openFileDialog.FileName);

                    if (content.Length == 0 || content.Length > 0 && Notification.MakeMessage("Файл содержит данные которые будут перезаписаны.\nПродолжить?", "Файл для записи данных", NotificationButton.YesNo) == NotificationResult.Yes)
                    {
                        FilePathFile = openFileDialog.FileName;
                    }
                }
            }

            UpdateSaveWidth();
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
                var prefCount = _filePathImages.Count;

                foreach (var file in openFileDialog.FileNames)
                {
                    if (!_filePathImages.Contains(file))
                    {
                        _filePathImages.Add(file);
                    }
                }

                if (prefCount != _filePathImages.Count)
                {
                    _isSuccessAction = false;
                    _resultImages.Clear();
                    _bynaryData.Clear();

                    ImagesWidth = new GridLength(1, GridUnitType.Star);
                    ActionWidth = new GridLength(1, GridUnitType.Star);

                    _selectedElementIndex = _selectedElementIndex == -1 ? _filePathImages.Count - 1 : _selectedElementIndex;

                    ImageData = _filePathImages[_selectedElementIndex];

                    FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
                }

                UpdateClearWidth();
                UpdateSaveWidth();
            }
        }

        public async void OnDoActionCommandExecuted(object p = null)
        {
            if (_isImport && FileData.Length == 0)
            {
                Notification.MakeMessage("Нет данных для импорта", ActionButtonName);
                return;
            };

            _isSuccessAction = false;
            IsButtonFree = false;
            CanBack = false;

            UpdateSaveWidth();

            var currentElementIndex = _selectedElementIndex;

            _selectedElementIndex = -1;

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);

            _bynaryData.Clear();
            _resultImages.Clear();

            if (_isImport) { await ImportData(); }
            else
            {
                await ExportData();

                if (_exportFileData.Length == 3)
                {
                    if (Context.MainWindow.IsActive &&
                        Context.MainWindowViewModel.CurrentPage is TextInPicturePage &&
                        Notification.MakeMessage("Экспортированные данные являются файлом.\nCохранить файл?", "Экспорт данных", NotificationButton.YesNo) == NotificationResult.Yes)
                    {
                        var saveFileDialog = new SaveFileDialog
                        {
                            Title = "Выбрать файл для сохранения данных",
                            FileName = _exportFileData[0],
                            Filter = $"Файлы (*{_exportFileData[1]})|*{_exportFileData[1]}"
                        };

                        if (saveFileDialog.ShowDialog() ?? false)
                        {
                            var selectedFilePath = saveFileDialog.FileName;

                            if (!File.Exists(selectedFilePath))
                            {
                                using (File.Create(selectedFilePath)) { }
                            }

                            byte[] fileBytes = Convert.FromBase64String(_exportFileData[2]);
                            File.WriteAllBytes(selectedFilePath, fileBytes);

                            Notification.MakeMessage("Фаил сохранен", "Экспорт данных");
                        }
                    }
                    else
                    {
                        FileData = _exportFileData[2];
                    }
                }
            }

            IsButtonFree = true;

            var count = _isSuccessAction ? _filePathImages.Count : 0;

            _selectedElementIndex = currentElementIndex;

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, count);

            UpdateSaveWidth();
        }

        public void OnRemoveImageCommandExecuted(object p = null)
        {
            var index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

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

            _isSuccessAction = false;
            _resultImages.Clear();
            _bynaryData.Clear();

            var count = _isSuccessAction ? _filePathImages.Count : 0;

            UpdateClearWidth();
            UpdateSaveWidth();

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, count);
        }

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();

            if (ActionWidth.Value == 0)
            {
                Context.TextInPicturePageViewModel = new TextInPicturePageViewModel();
            }
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
            var index = (p == null) ? (-1) : ((p is int value) ? (value) : (-1));

            if (index == -1 || _selectedElementIndex == index || !IsButtonFree) return;

            _selectedElementIndex = index;

            ImageData = _filePathImages[_selectedElementIndex];

            var count = _isImport ? _resultImages.Count : _bynaryData.Count;

            FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, count);
        }

        public void OnClearAllCommandExecuted(object p = null)
        {
            ActionWidth = new GridLength(0, GridUnitType.Pixel);
            ImagesWidth = new GridLength(0, GridUnitType.Pixel);
            ClearWidth = new GridLength(0, GridUnitType.Pixel);

            _filePathImages.Clear();
            _resultImages.Clear();
            _bynaryData.Clear();

            _selectedElementIndex = -1;
            FilePathImageStackPanel = new StackPanel();
        }

        private void ImportAction()
        {
            ActionButtonName = "Импортировать";

            ImportButtonBackgroundColor = Color2;
            ImportButtonForegroundColor = Color3;
            ExportButtonBackgroundColor = Color4;
            ExportButtonForegroundColor = Color3;

            if (FilePathFile.Length > 0)
            {
                if (FileData == null || FileData?.Length == 0 || (FileData?.Length > 0 && Notification.MakeMessage("Заменить текст на данные из файла?", "Файл для чтения данных", NotificationButton.YesNo) == NotificationResult.Yes))
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

            _isImport = true;
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

                if (content.Length > 0 && Notification.MakeMessage("Файл содержит данные которые будут перезаписаны.\nОставить выбранный файл?", "Файл для записи данных", NotificationButton.YesNo) == NotificationResult.Yes || content.Length == 0)
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

        private async Task ExportData()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            CanBack = true;

            var message = "Данные экспортированы";
            var title = "Экспорт данных";

            FileData = "";

            try
            {
                foreach (var filePathImage in _filePathImages)
                {
                    _bynaryData.Add(await ImageHelper.ExportDataFromImage(filePathImage));

                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(TextInPicturePage) && Context.MainWindow.IsActive)
                    {
                        FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _bynaryData.Count);
                    }
                }

                var allData = new StringBuilder();

                foreach (var item in _bynaryData)
                {
                    allData.Append(item);
                }

                var exportData = Converter.ConvertBinaryStringToText(allData.ToString());

                _exportFileData = exportData.Split("[*]");

                if (_exportFileData.Length > 1)
                {
                    _exportFileData[2] = Cryptography.DecryptText(_exportFileData[2], hashPassword);

                    if (!Context.MainWindow.IsActive || Context.MainWindowViewModel.CurrentPage.GetType() != typeof(TextInPicturePage))
                    {
                        message += ".\nДанные являются файлом. Необходимо перейти на cтраницу для его формирования";
                    }
                }
                else
                {
                    FileData = Cryptography.DecryptText(exportData, hashPassword);
                }

                _isSuccessAction = true;
            }
            catch
            {
                message = "Не удалось экспортировать данные";
                _bynaryData.Clear();
                _exportFileData = new string[0];
                _isSuccessAction = false;
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
            }
            finally
            {
                DoNotification(message, title, typeof(TextInPicturePage), PageTitle);
            }
        }

        private async Task ImportData()
        {
            var hashPassword = Program.GetHash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            CanBack = true;

            var message = "Данные импортированы";
            var title = "Импорт данных";

            try
            {
                var inportData = FileData;

                if (FilePathFile.Length > 0)
                {
                    inportData = Convert.ToBase64String(File.ReadAllBytes(_filePathFile));
                    inportData = Cryptography.EncryptText(inportData, hashPassword);

                    var fileInfo = new FileInfo(_filePathFile);
                    inportData = $"{fileInfo.Name}[*]{fileInfo.Extension}[*]" + inportData;
                }
                else
                {
                    inportData = Cryptography.EncryptText(inportData, hashPassword);
                }

                string binary = Converter.ConvertTextToBinaryString(inportData);

                _resultImages.Clear();

                var lines = Program.SplitStringIntoParts(binary, _filePathImages.Count);

                for (int i = 0; i < _filePathImages.Count; i++)
                {
                    _resultImages.Add(await ImageHelper.ImportDataToImage(lines[i], _filePathImages[i]));

                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(TextInPicturePage) && Context.MainWindow.IsActive)
                    {
                        FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree, _resultImages.Count);
                    }
                }

                _isSuccessAction = true;
            }
            catch (Exception ex)
            {
                _resultImages.Clear();
                message = $"Не удалось импортировать данные.\nError: {ex.Message}";
                _isSuccessAction = false;
                FilePathImageStackPanel = LoadFilePathImages(_filePathImages, ShowImageCommand, RemoveImageCommand, _selectedElementIndex, IsButtonFree);
            }
            finally
            {
                DoNotification(message, title, typeof(TextInPicturePage), PageTitle);
            }
        }

        private void UpdateClearWidth()
        {
            if (_filePathImages.Count > 1)
            {
                ClearWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                ClearWidth = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void UpdateSaveWidth()
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
        }
    }
}
