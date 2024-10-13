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
        private string _splitButtonBackgroundColor = "";
        private string _splitButtonForegroundColor = "";
        private string _actionButtonBackgroundColor = "";
        private string _actionButtonForegroundColor = "";
        private string _errorDoActionMessage = "";
        private string _errorSaveMessage = "";

        private bool _isSplit = false;
        private bool _isImport = false;
        private bool _isFileDataReadonly = false;
        private bool _canDoAction = true;

        private GridLength _onePictureWidth;
        private GridLength _manyPictureWidth;
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);

        private List<string> _filePathImages = new List<string>();

        private List<Bitmap> _bitmapImages = new List<Bitmap>();

        private StackPanel _filePathImageStackPanel = new StackPanel();

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

        private void OnSplitCommandExecuted(object p = null)
        {
            if (_isSplit)
            {
                OnePictureWidth = new GridLength(0, GridUnitType.Star);
                ManyPictureWidth = new GridLength(1, GridUnitType.Star);

                SplitButtonBackgroundColor = Color2;
                SplitButtonForegroundColor = Color3;

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

                SplitButtonBackgroundColor = Color4;
                SplitButtonForegroundColor = Color3;

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
                    if (_isSplit)
                    {
                        var saveFileDialog = new SaveFileDialog();

                        var dir = Path.GetDirectoryName(_filePathImage);
                        var name = "PixelCrypt_" + (Path.GetFileNameWithoutExtension(_filePathImage) + "_" + DateTime.Now).Replace(":", "").Replace(" ", "").Replace(".", "");
                        var format = ImageFormat.Png;

                        format = ImageFormat.Png;
                        saveFileDialog.Filter = "PNG Image|*.png";

                        saveFileDialog.Title = "Сохранение изображения";
                        saveFileDialog.RestoreDirectory = true;
                        saveFileDialog.InitialDirectory = dir;
                        saveFileDialog.FileName = name;

                        if (saveFileDialog.ShowDialog() ?? false)
                        {
                            _bitmapImages[0].Save(saveFileDialog.FileName, format);
                            MessageBox.Show("Картинка сохранена", "Сохранение изображения");
                        }
                    }
                    else
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
                            for (int i = 0; i < _bitmapImages.Count; i++)
                            {
                                var name = Path.Combine(folder, Path.GetFileNameWithoutExtension(_filePathImages[i]) + $"_PixelCrypt_{now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "")}");
                                var format = ImageFormat.Png;

                                format = ImageFormat.Png;
                                name += ".png";

                                _bitmapImages[i].Save(name, format);
                            }

                            MessageBox.Show("Картинки сохранены", "Сохранение изображений");
                        }
                    }
                }
                else
                {
                    if (File.Exists(_filePathFile))
                    {
                        File.WriteAllText(_filePathFile, FileData);

                        MessageBox.Show("Данные успешно сохранены", "Сохранение");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Возникла ошибка при сохранении", "Сохранение");
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
                    IsFileDataReadonly = true;

                    if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "файл для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    {
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

                    if (content.Length > 0 && MessageBox.Show("файл содержит данные которые будут перезаписаны. Продолжить?", "файл для записи данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
            if (!_canDoAction) return;

            _isSuccessAction = false;

            if (_isImport)
            {
                ImportData();
            }
            else
            {
                ExportData();
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

            FilePathImageStackPanel = LoadFilePathImages();
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
                if (FileData == null || FileData.Length == 0 || (FileData.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "файл для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
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

                if (content.Length > 0 && MessageBox.Show("файл содержит данные которые будут перезаписаны. Оставить выбранный файл?", "файл для записи данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                var border = new Border
                {
                    BorderBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3),
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
                        Height = 25,
                        Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(Color3)
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
            _canDoAction = true;

            if (_isImport)
            {
                _canDoAction = _canDoAction && FileData.Length > 0;

                if (_isSplit)
                {
                    _canDoAction = _canDoAction && FilePathImage.Length > 0;
                }
                else
                {
                    _canDoAction = _canDoAction && _filePathImages.Count > 1;
                }
            }
            else
            {
                if (_isSplit)
                {
                    _canDoAction = _canDoAction && FilePathImage.Length > 0;
                }
                else
                {
                    _canDoAction = _canDoAction && _filePathImages.Count > 1;
                }
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
                res = res && _bitmapImages.Count > 0;
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
                ChoseImageVisibility = Visibility.Collapsed;
                ChoseImageWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                ChoseImageVisibility = Visibility.Visible;
                ChoseImageWidth = new GridLength(0, GridUnitType.Pixel);
            }

            return res;
        }

        private void ExportData()
        {
            var BynaryData = new List<string>();

            FileData = "";

            try
            {
                if (_isSplit)
                {
                    BynaryData.Add(ExportDataFromImage(_filePathImage));

                }
                else
                {
                    foreach (var filePathImage in _filePathImages)
                    {
                        BynaryData.Add(ExportDataFromImage(filePathImage));
                    }
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

                MessageBox.Show("Данные экспортированы", "Экспорт данных");

                FileData = exportData;
            }
            catch
            {
                MessageBox.Show($"Не удалось экспортировать данные");
            }
        }

        private void ImportData()
        {
            try
            {
                var inportData = FileData;

                var hashPassword = Program.Hash32(Password?.Length > 0 ? Password : "PyxelCrypt");

                inportData = Program.Encrypt(inportData, hashPassword);

                string binary = Program.TextToBinary(inportData);

                _bitmapImages = new List<Bitmap>();

                if (_isSplit)
                {
                    _bitmapImages.Add(ImportDataToImage(binary, _filePathImage));
                }
                else
                {
                    var lines = Program.SplitStringIntoParts(binary, _filePathImages.Count);

                    for (int i = 0; i < _filePathImages.Count; i++)
                    {
                        _bitmapImages.Add(ImportDataToImage(lines[i], _filePathImages[i]));
                    }
                }

                _isSuccessAction = true;

                MessageBox.Show("Данные импортированы", "Испорт данных");
            }
            catch
            {
                _bitmapImages = new List<Bitmap>();
                MessageBox.Show($"Не удалось импортировать данные");
            }
        }

        private Bitmap ImportDataToImage(string data, string filepath)
        {
            var pixels = Program.GetPixelsFromImage(filepath);
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

                        //var a = (elementIndex < dataA.Length) ? (byte)(255) : color.A;
                        //var r = (elementIndex < dataR.Length) ? (byte)(0) : color.R;
                        //var g = (elementIndex < dataG.Length) ? (byte)(0) : color.G;
                        //var b = (elementIndex < dataB.Length) ? (byte)(0) : color.B;

                        color = System.Drawing.Color.FromArgb(a, r, g, b);

                        elementIndex++;
                    }

                    newPixels[x, y] = color;
                    index++;
                }
            }

            return Program.CreateImageFromPixels(newPixels);
        }

        private string ExportDataFromImage(string path)
        {
            var res = "";

            var pixels = Program.GetPixelsFromImage(path);
            var listPixels = new List<System.Drawing.Color>();

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    listPixels.Add(pixels[x, y]);
                }
            }

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
    }
}
