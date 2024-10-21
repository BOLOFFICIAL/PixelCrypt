using FontAwesome5;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System.Drawing;
using System.Drawing.Imaging;
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

        #region Fields

        #region Propertyes

        #region Private

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

        #endregion

        #endregion

        #region Commands

        #region Public

        public ICommand ActionCommand { get; }
        public ICommand SplitCommand { get; }
        public ICommand ClearPathFileCommand { get; }
        public ICommand ChoseFileCommand { get; }

        #endregion

        #region Private

        private ICommand RemoveImageCommand { get; }

        #endregion

        #endregion

        #endregion

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

        #region Propertyes

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

        #endregion

        #region Commands

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
            return CanSave();
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            try
            {
                if (_isImport)
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
                        for (int i = 0; i < _resultImages.Count; i++)
                        {
                            var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(_filePathImages[i]) + $"_PixelCrypt_{now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "")}");
                            var format = ImageFormat.Png;

                            format = ImageFormat.Png;
                            name += ".png";

                            _resultImages[i].Save(name, format);
                        }

                        Notification.MakeMessage("Картинки сохранены", "Сохранение изображений");
                    }
                }
                else
                {
                    if (File.Exists(_filePathFile))
                    {
                        File.WriteAllText(_filePathFile, FileData);

                        Notification.MakeMessage("Данные успешно сохранены", "Сохранение");
                    }
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
            return CanChoseImage();
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
            return CanDoAction();
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

        #endregion

        #region Methods

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

        private bool CanDoAction()
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

        private bool CanSave()
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

        private bool CanChoseImage()
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

        private async Task ExportData()
        {
            var BynaryData = new List<string>();

            FileData = "";

            try
            {
                foreach (var filePathImage in _filePathImages)
                {
                    var exportDataImage = await ExportDataFromImage(filePathImage);
                    BynaryData.Add(exportDataImage);
                    FilePathImageStackPanel = LoadFilePathImages(BynaryData.Count);
                }

                var allData = new StringBuilder();

                foreach (var item in BynaryData)
                {
                    allData.Append(item);
                }

                var exportData = Program.BinaryToText(allData.ToString());

                var hashPassword = Program.Hash32(Password?.Length > 0 ? Password : "PyxelCrypt");

                exportData = Program.Decrypt(exportData, hashPassword);

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

                var hashPassword = Program.Hash32(Password?.Length > 0 ? Password : "PyxelCrypt");

                inportData = Program.Encrypt(inportData, hashPassword);

                string binary = Program.TextToBinary(inportData);

                _resultImages = new List<Bitmap>();

                var lines = Program.SplitStringIntoParts(binary, _filePathImages.Count);

                for (int i = 0; i < _filePathImages.Count; i++)
                {
                    var importDataImage = await ImportDataToImage(lines[i], _filePathImages[i]);
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

        private async Task<Bitmap> ImportDataToImage(string data, string filepath)
        {
            var importDataImage = await Task.Run(() =>
            {
                var pixels = Program.GetPixelsFromImageArray(filepath);

                var listPixels = new List<System.Drawing.Color>();

                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var color = System.Drawing.Color.FromArgb(
                            NormalizeColorByte(pixels[x, y].A),
                            NormalizeColorByte(pixels[x, y].R),
                            NormalizeColorByte(pixels[x, y].G),
                            NormalizeColorByte(pixels[x, y].B));

                        listPixels.Add(color);
                    }
                }

                var uniqBinaryLength = Program.ToBinary(listPixels.Count).Length;

                var spl = Program.SplitStringIntoParts(data, 4);

                var l0 = Program.ToBinary(spl[0].Length).PadLeft(uniqBinaryLength, '0');
                var l1 = Program.ToBinary(spl[1].Length).PadLeft(uniqBinaryLength, '0');
                var l2 = Program.ToBinary(spl[2].Length).PadLeft(uniqBinaryLength, '0');
                var l3 = Program.ToBinary(spl[3].Length).PadLeft(uniqBinaryLength, '0');

                var dataA = l0 + spl[0];
                var dataR = l1 + spl[1];
                var dataG = l2 + spl[2];
                var dataB = l3 + spl[3];

                var limit = listPixels.Count / slash;

                var avrage = (dataA.Length + dataR.Length + dataG.Length + dataB.Length) / 4;

                if (limit < avrage)
                {
                    throw new Exception($"Данных слишком  много для импорта в картинку");
                }

                var newPixels = new System.Drawing.Color[width, height];

                int index = 0;
                int elementIndex = 0;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var color = System.Drawing.Color.FromArgb(
                            listPixels[index].A,
                            listPixels[index].R,
                            listPixels[index].G,
                            listPixels[index].B);

                        if (index % slash == 0)
                        {
                            var a = (elementIndex < dataA.Length) ? (byte)(color.A - byte.Parse(dataA[elementIndex].ToString())) : color.A;
                            var r = (elementIndex < dataR.Length) ? (byte)(color.R - byte.Parse(dataR[elementIndex].ToString())) : color.R;
                            var g = (elementIndex < dataG.Length) ? (byte)(color.G - byte.Parse(dataG[elementIndex].ToString())) : color.G;
                            var b = (elementIndex < dataB.Length) ? (byte)(color.B - byte.Parse(dataB[elementIndex].ToString())) : color.B;

                            color = System.Drawing.Color.FromArgb(a, r, g, b);

                            elementIndex++;
                        }

                        newPixels[x, y] = color;
                        index++;
                    }
                }
                return newPixels;
            });

            return Program.CreateBitmapFromPixels(importDataImage);
        }

        private async Task<string> ExportDataFromImage(string path)
        {
            try
            {
                var exportDataImage = await Task.Run(() =>
                {
                    var res = "";

                    var pixels = Program.GetPixelsFromImageArray(path);

                    var listPixels = Program.GetPixelsFromImageList(pixels);

                    var uniqBinaryLength = Program.ToBinary(listPixels.Count).Length;

                    var lDiv = "";
                    var lMod = "";

                    var index = 0;

                    for (index = 0; index < slash * uniqBinaryLength - (slash - 1); index += slash)
                    {
                        lDiv += (listPixels[index].A % 2 == 0) ? "1" : "0";
                        lMod += (listPixels[index].B % 2 == 0) ? "1" : "0";
                    }

                    var sizeDiv = Program.FromBinary(lDiv);
                    var SizeMod = Program.FromBinary(lMod);

                    var dataA = new StringBuilder();
                    var dataR = new StringBuilder();
                    var dataG = new StringBuilder();
                    var dataB = new StringBuilder();

                    for (int i = index; i < index + (slash * sizeDiv - (slash - 1)); i += slash)
                    {
                        dataA.Append((listPixels[i].A % 2 == 0) ? "1" : "0");
                        dataR.Append((listPixels[i].R % 2 == 0) ? "1" : "0");
                        dataG.Append((listPixels[i].G % 2 == 0) ? "1" : "0");
                    }

                    for (int i = index; i < index + (slash * SizeMod - (slash - 1)); i += 8)
                    {
                        dataB.Append((listPixels[i].B % 2 == 0) ? "1" : "0");
                    }

                    res = dataA.ToString() + dataR.ToString() + dataG.ToString() + dataB.ToString();
                    return res;
                });

                return exportDataImage;
            }
            catch
            {
                return "";
            }
        }

        private byte NormalizeColorByte(byte value)
        {
            var res = value;
            if (res % 2 != 0)
            {
                return res;
            }
            else
            {
                if (res == 0)
                {
                    return 1;
                }
                else
                {
                    return (byte)(res - 1);
                }
            }
        }

        #endregion

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
    }
}
