using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Page
{
    class SteganographyPageViewModel : BasePageLayoutViewModel
    {
        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public PasswordBoxViewModel PasswordBox { get; set; }
        public TaskControlViewModel TaskControl { get; set; }

        private GridLength _settingsHeightHeight;
        private GridLength _taskControlHeight;
        private string _filePath;
        private string _content;
        private bool _isReadOnly;
        private bool _isEnable = true;
        private bool _isImport;
        private bool _isIndexDependence = true;

        public ICommand SelectFileCommand { get; }
        public ICommand ClearFileCommand { get; }

        public SteganographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Steganography";

            SelectFileCommand = new LambdaCommand(OnSelectFileCommand, CanSelectFile);
            ClearFileCommand = new LambdaCommand(OnClearFileCommand, CanClearFile);

            Progress = new ProgressPanelViewModel();
            PasswordBox = new PasswordBoxViewModel();

            ImageList = new ImageListViewModel();

            UpdateImageCount();

            ImageList.ConfirmationClearRequested += ClearConfirmation;
            ImageList.ConfirmationAddRequested += AddConfirmation;
            ImageList.ConfirmationRemoveRequested += RemoveConfirmation;
            ImageList.AddRequested += UpdateImageCount; 
            ImageList.ClearRequested += UpdateImageCount;
            ImageList.RemoveRequested += UpdateImageCount;

            TaskControl = new TaskControlViewModel();

            TaskControl.StartRequested += StartCommand;
            TaskControl.CanStart += () => ImageList.Images.Count > 0;
            TaskControl.ConfirmationStartRequested += StartConfirmation;

            TaskControl.StopRequested += StopCommand;
            TaskControl.ConfirmationStopRequested += StopConfirmation;

            TaskControl.SaveRequested += SaveCommand;
            TaskControl.CanSave += () => ImageList.Images.All(i => i.Status == StatusType.Success);
        }

        private bool RemoveConfirmation()
        {
            if (ImageList.Images.Where(i => i.Status == StatusType.Success).Count() > 0)
            {
                var res = Notification.Show("Удаление изображений приведет к потере данных, продолжить?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);
                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            ImageList.ResetImages();
            SetToolStatus();
            return true;
        }

        private bool AddConfirmation()
        {
            if (ImageList.Images.Where(i => i.Status == StatusType.Success).Count() > 0)
            {
                var res = Notification.Show("Добавление изображений приведет к потере данных, продолжить?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);
                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            ImageList.ResetImages();
            SetToolStatus();
            return true;
        }

        private bool CanClearFile(object arg)
        {
            return true;
        }

        private bool CanSelectFile(object arg)
        {
            return true;
        }

        private void OnSelectFileCommand(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    var res = Notification.Show("Текст будет заменен на содержимое из файла, продолжить?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                    if (res.Result != NotificationResultType.Yes) return;
                }

                FilePath = openFileDialog.FileName;
            }
        }

        private void OnClearFileCommand(object obj)
        {
            FilePath = "";

            if (!string.IsNullOrEmpty(Content)) 
            {
                var res = Notification.Show("Очистить содержимое?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result == NotificationResultType.Yes)
                {
                    Content = "";
                }
            }
        }

        public bool IsEnable
        {
            get => _isEnable;
            set => Set(ref _isEnable, value);
        }

        public GridLength SettingsHeight
        {
            get => _settingsHeightHeight;
            set => Set(ref _settingsHeightHeight, value);
        }

        public GridLength TaskControlHeight
        {
            get => _taskControlHeight;
            set => Set(ref _taskControlHeight, value);
        }

        public bool IsIndexDependence 
        {
            get => _isIndexDependence;
            set => Set(ref _isIndexDependence, value);
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                var isEmpty = string.IsNullOrEmpty(value);

                IsReadOnly = !isEmpty;

                if (!isEmpty)
                {
                    var content = File.ReadAllText(value);
                    Content = content.Substring(0, Math.Min(content.Length, 10000));
                }

                Set(ref _filePath, value);
                OnPropertyChanged("FileName");
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => Set(ref _isReadOnly, value);
        }

        public string FileName => Path.GetFileName(FilePath);

        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }
        
        public string ResultString { get; private set; }

        private void SaveCommand()
        {
            if (_isImport)
            {
                SaveImport();
            }
            else
            {
                SaveExport();
            }
        }

        private bool StopConfirmation()
            => Notification.Show("Вы уверены что хотите остановить?",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void StopCommand()
        {
            Progress.ProgressTime = "Остановка...";
            Progress.StopTimer();
        }

        private bool StartConfirmation()
        {
            _isImport = true;

            if (Notification.Show(
                "Выберите желаемое действие",
                actions: new List<(string, Action)>()
                {
                        ("Import", () => {_isImport = true; }),
                        ("Export",() => {_isImport = false; })
                },
                icon: NotificationIconType.Question).Result == NotificationResultType.Cancel)
            {
                return false;
            }

            if (_isImport && string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(FilePath))
            {
                Notification.Show("Нет данных для импорта", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                return false;
            }

            if (ImageList.Images.All(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("Текущий прогресс будет потерян, продолжить?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            SetToolStatus();
            return true;
        }

        private bool ClearConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("Вы уверены что хотите очистить список?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            SetToolStatus();
            return true;
        }

        private async void StartCommand()
        {
            try
            {
                var token = TaskControl.CancellationTokenSource.Token;

                IsEnable = false;
                IsReadOnly = true;
                ImageList.IsEnable = IsEnable;

                SettingsHeight = new GridLength(0, GridUnitType.Star);

                SetToolStatus("Выполняется");

                Progress.StartTimer();

                int totalItems = ImageList.Images.Count;

                double totalPixels = ImageList.Images.Sum(i => (double)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight));

                var hashPassword = ProgramHelper.GetHash32(PasswordBox.Password ?? "");

                if (_isImport)
                {
                    var res = await Import(totalPixels, hashPassword, token);
                    if (!res) return;
                }
                else
                {
                    var res = await Export(totalPixels, hashPassword, token);
                    if (!res) return;
                }

                if (token.IsCancellationRequested)
                {
                    Notification.Show("Операция остановлена", icon: NotificationIconType.Question);
                }
                else
                {
                    Notification.Show("Операция завершена", icon: NotificationIconType.Success);
                    SetToolStatus("Завершено");
                }
            }
            finally
            {
                Progress.StopTimer();
                TaskControl.FinishCommand();
                IsEnable = true;
                IsReadOnly = !string.IsNullOrEmpty(FilePath);
                ImageList.IsEnable = IsEnable;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
            }
        }

        private async Task<bool> Export(double totalPixels, string passwordHash, CancellationToken token)
        {
            ResultString = "";
            var extractedBinaryParts = new List<string>();
            var successfullyProcessedImages = new List<ImageFile>();

            ImageList.ResetImages();

            foreach (var imageItem in ImageList.Images)
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    imageItem.Status = StatusType.InProgress;
                    ImageList.SelectedImage = imageItem;

                    var exportTask = ImageHelper.ExportDataFromImage(imageItem.ImageFile.FilePath);
                    var cancelTask = Task.Delay(Timeout.Infinite, token);
                    var completedTask = await Task.WhenAny(exportTask, cancelTask);

                    if (completedTask == cancelTask)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    extractedBinaryParts.Add(await exportTask);
                    successfullyProcessedImages.Add(imageItem.ImageFile);
                    double processedPixels = successfullyProcessedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                    Progress.UpdateTimer(processedPixels, totalPixels);
                    SetToolStatus($"Выполнено {Progress.ProgressPercent}");
                    imageItem.Status = StatusType.Success;
                }
                catch (OperationCanceledException)
                {
                    imageItem.Status = StatusType.None;
                    Notification.Show("Операция остановлена", icon: NotificationIconType.Question);
                    SetToolStatus();
                    ImageList.ResetImages();
                    return false;
                }
                catch (Exception ex)
                {
                    imageItem.Status = StatusType.Failed;
                    Notification.Show($"Возникла ошибка: {ex.Message}", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus($"Ошибка");
                    return false;
                }
            }

            try
            {
                var decodedTextParts = new List<string>();

                foreach (var dataPart in extractedBinaryParts)
                    decodedTextParts.Add(Converter.ConvertBinaryStringToText(dataPart));

                var allData = new StringBuilder();

                if (IsIndexDependence)
                {
                    var items = decodedTextParts.Select(el => el.Split("[i]")[1]);
                    foreach (var item in items)
                        allData.Append(item);
                }
                else
                {
                    var ordered = decodedTextParts
                        .Select(el => el.Split("[i]"))
                        .OrderBy(data => int.Parse(data[0]))
                        .Select(data => data[1]);

                    foreach (var item in ordered)
                        allData.Append(item);
                }

                var finalDataString = allData.ToString();
                var fileMetadataParts = finalDataString.Split("[d]");

                if (fileMetadataParts.Length > 1)
                {
                    ResultString = Encryption.DecryptText(fileMetadataParts[2], passwordHash);

                    var shouldAssembleFile = Notification.Show("Экспортированные данные являются файлом.\nСобрать файл?", "Экспорт данных", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

                    if (shouldAssembleFile)
                    {
                        var saveRes = FileHelper.SaveDataToFile(fileMetadataParts[0], $"Файлы (*{fileMetadataParts[1]})|*{fileMetadataParts[1]}", Convert.FromBase64String(ResultString));

                        if (saveRes.Result.IsSuccessResult)
                        {
                            string fileData = File.ReadAllText(saveRes.FilePath) ?? string.Empty;

                            Content = fileData.Length > 10000 ? fileData.Substring(0, 10000) : fileData;
                            FilePath = saveRes.FilePath;
                        }
                        else
                        {
                            Content = ResultString;
                            FilePath = "";
                        }
                    }
                    else
                    {
                        Content = ResultString;
                        FilePath = "";
                    }
                }
                else
                {
                    Content = Encryption.DecryptText(finalDataString, passwordHash);
                    ResultString = Content;
                }
            }
            catch (Exception ex)
            {
                Notification.Show($"Не удалось сформировать данные.\nВозникла ошибка: {ex.Message}", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                ImageList.ResetImages();
                SetToolStatus($"Ошибка");
                return false;
            }

            return true;
        }

        private async Task<bool> Import(double totalPixels, string passwordHash, CancellationToken token)
        {
            var dataToHide = "";
            var dataChunks = new List<string>();
            var completedImages = new List<ImageFile>();

            try
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    dataToHide = Encryption.EncryptText(Content, passwordHash);
                }
                else
                {
                    var sourceFileInfo = new FileInfo(FilePath);
                    dataToHide = $"{sourceFileInfo.Name}[d]{sourceFileInfo.Extension}[d]" + Encryption.EncryptText(Convert.ToBase64String(File.ReadAllBytes(FilePath)), passwordHash);
                }

                var imageCapacities = ImageList.Images.Select(i => (int)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight * 3 * 0.5 / 64)).ToList();
                var dataDistributionPlan = ProgramHelper.DistributeData(imageCapacities, dataToHide.Length);

                if (dataDistributionPlan == null)
                {
                    Notification.Show($"Данных слишком много", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus();
                    return false;
                }

                dataChunks = ProgramHelper.SplitString(dataToHide, dataDistributionPlan);

                if (dataChunks == null)
                {
                    Notification.Show($"Не удалось сформировать данные для импорта", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Notification.Show($"Возникла ошибка: {ex.Message}", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                SetToolStatus($"Ошибка");
                return false;
            }

            ImageList.ResetImages();

            for (int i = 0; i < ImageList.Images.Count; i++)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    ImageList.Images[i].Status = StatusType.InProgress;

                    var content = IsIndexDependence ? dataChunks[i] : $"{i}[i]{dataChunks[i]}";
                    var binaryDataToWrite = Converter.ConvertTextToBinaryString(content);
                    var importTask = ImageHelper.ImportDataToImage(binaryDataToWrite, ImageList.Images[i].ImageFile.FilePath);
                    var cancelTask = Task.Delay(Timeout.Infinite, token);
                    var completedTask = await Task.WhenAny(importTask, cancelTask);

                    if (completedTask == cancelTask)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    ImageList.Images[i].ImageFile.ResultImage = await importTask;

                    completedImages.Add(ImageList.Images[i].ImageFile);
                    dataChunks[i] = "";
                    double convertedPixels = completedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                    Progress.UpdateTimer(convertedPixels, totalPixels);
                    SetToolStatus($"Выполнено {Progress.ProgressPercent}");
                    ImageList.Images[i].Status = StatusType.Success;
                    ImageList.SelectedImage = ImageList.Images[i];
                }
                catch (OperationCanceledException)
                {
                    ImageList.Images[i].Status = StatusType.None;
                    Notification.Show("Операция остановлена", icon: NotificationIconType.Question);
                    SetToolStatus();
                    ImageList.ResetImages();
                    return false;
                }
                catch (Exception ex)
                {
                    ImageList.Images[i].Status = StatusType.Failed;
                    Notification.Show($"Возникла ошибка: {ex.Message}", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus($"Ошибка");
                    return false;
                }
            }
            return true;
        }

        private void UpdateImageCount()
        {
            if (ImageList.Images.Count > 0)
            {
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
                TaskControlHeight = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                SettingsHeight = new GridLength(0, GridUnitType.Star);
                TaskControlHeight = new GridLength(0, GridUnitType.Star);
            }
        }

        private void SaveImport()
        {
            if (ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Count() == 0)
            {
                Notification.Show($"Нет данных для сохранения", icon: NotificationIconType.Error);
                return;
            }
            var res = FileHelper.SaveBitmapToFolder(ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList());

            if (res.IsSuccessResult)
            {
                Notification.Show(res.ResultMessage, icon: NotificationIconType.Success);
                SetToolStatus("Сохранено");
            }
            else
            {
                Notification.Show(res.ResultMessage, button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
            }
        }

        private void SaveExport()
        {
            if (ResultString.Length == 0)
            {
                Notification.Show($"Нет данных для сохранения", icon: NotificationIconType.Error);
                return;
            }

            FileHelper.SaveDataToFile($"PixelCrypt_{DateTime.Now:yyyyMMddHHmmss}", $"Файлы (*.txt)|*.txt", ResultString);
            Notification.Show($"Данные успешно сохранены", icon: NotificationIconType.Success);
            SetToolStatus("Сохранено");
            return;
        }
    }
}
