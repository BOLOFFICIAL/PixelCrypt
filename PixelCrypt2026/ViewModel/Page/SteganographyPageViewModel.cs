using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.ComponentModel;
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
        public ModeControlViewModel ModeControl { get; set; }

        private GridLength _settingsHeight;
        private GridLength _taskControlHeight;
        private string _filePath;
        private string _content;
        private bool _isReadOnly;
        private bool _isEnable = true;
        private bool _isIndexDependence = true;
        private bool _isProcessing;

        public ICommand SelectFileCommand { get; }
        public ICommand ClearFileCommand { get; }

        public SteganographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Steganography";

            SelectFileCommand = new LambdaCommand(OnSelectFileCommand);
            ClearFileCommand = new LambdaCommand(OnClearFileCommand);

            Progress = new ProgressPanelViewModel();
            PasswordBox = new PasswordBoxViewModel();

            ImageList = new ImageListViewModel()
            {
                Add = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount,
                    ConfirmationRequired = AddConfirmation,
                },
                Clear = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount,
                    ConfirmationRequired = ClearConfirmation,
                },
                Remove = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount,
                    ConfirmationRequired = RemoveConfirmation,
                },
            };

            ImageList.PropertyChanged += OnImageListPropertyChanged;

            UpdateImageCount();

            TaskControl = new TaskControlViewModel()
            {
                Start = new ControlAction()
                {
                    ExecuteRequested = StartCommand,
                    ConfirmationRequired = StartConfirmation,
                    CanExecute = () => ImageList.Images.Count > 0,
                },
                Stop = new ControlAction()
                {
                    ExecuteRequested = StopCommand,
                    ConfirmationRequired = StopConfirmation,
                },
                Save = new ControlAction()
                {
                    ExecuteRequested = SaveCommand,
                    ConfirmationRequired = SaveConfirmation,
                    CanExecute = () => ImageList.Images.All(i => i.Status == StatusType.Success),
                },
                Copy = new ControlAction()
                {
                    ExecuteRequested = CopyCommand,
                    ConfirmationRequired = CopyConfirmation,
                },
            };

            ModeControl = new ModeControlViewModel(new List<string>() { "Import", "Export" });
        }

        public bool IsEnable
        {
            get => _isEnable;
            set => Set(ref _isEnable, value);
        }

        public GridLength SettingsHeight
        {
            get => _settingsHeight;
            set => Set(ref _settingsHeight, value);
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

                try
                {
                    if (!isEmpty)
                    {
                        var content = File.ReadAllText(value);
                        Content = content.Substring(0, Math.Min(content.Length, 10000));
                    }

                    IsReadOnly = !isEmpty;
                    Set(ref _filePath, value);
                }
                catch (Exception ex)
                {
                    Notification.Show($"Failed to read file:\n{ex.Message}", "Open file", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                }
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

        private bool RemoveConfirmation()
        {
            if (ImageList.Images.Where(i => i.Status == StatusType.Success).Count() > 0)
            {
                var res = Notification.Show("Removing images will reset progress. Continue?", "Remove images", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);
                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            ImageList.ResetImages();
            ResultString = "";
            SetToolStatus();
            return true;
        }

        private bool AddConfirmation()
        {
            if (ImageList.Images.Where(i => i.Status == StatusType.Success).Count() > 0)
            {
                var res = Notification.Show("Adding new images will reset progress. Continue?", "Add images", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);
                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            ImageList.ResetImages();
            ResultString = "";
            SetToolStatus();
            return true;
        }

        private void OnSelectFileCommand(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a file",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    var message = "This will replace the text with the file content";

                    if (ImageList.Images.All(i => i.Status == StatusType.Success))
                    {
                        message += " and reset current progress";
                    }

                    message += ". Continue?";

                    var res = Notification.Show(message, "Select file", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                    if (res.Result != NotificationResultType.Yes) return;
                }

                FilePath = openFileDialog.FileName;
                ImageList.ResetImages();
                ResultString = "";
                SetToolStatus();
            }
        }

        private void OnClearFileCommand(object obj)
        {
            FilePath = "";

            if (!string.IsNullOrEmpty(Content))
            {
                var res = Notification.Show("Clear contents?", "Clear contents", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result == NotificationResultType.Yes)
                {
                    Content = "";
                }
            }
        }

        private void SaveCommand()
        {
            var res = new ActionResult();

            switch (ModeControl.SelectedMode)
            {
                case 0:
                    {
                        res = FileHelper.SaveImageToFolder(ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList());
                    }
                    break;
                case 1:
                    {
                        res = FileHelper.SaveDataToFile($"PixelCrypt_{DateTime.Now:yyyyMMddHHmmss}", $"Files (*.txt)|*.txt", ResultString).Result;
                    }
                    break;
            }

            if (res.IsSuccessResult)
            {
                Notification.Show(res.ResultMessage, "Save", button: NotificationButtonType.Ok, icon: NotificationIconType.Success);
                ImageList.ResetImages();
                ResultString = "";
                SetToolStatus();
            }
            else
            {
                Notification.Show(res.ResultMessage, "Save", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
            }
        }

        private bool SaveConfirmation()
        {
            switch (ModeControl.SelectedMode)
            {
                case 0:
                    {
                        if (ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Count() == 0)
                        {
                            Notification.Show($"No data to save", "Save", icon: NotificationIconType.Error);
                            return false;
                        }

                        var saveImages = ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList();

                        var errorList = saveImages
                            .Where(si => !File.Exists(si.ResultImage))
                            .Select(si => si.FileName)
                            .ToList();

                        if (errorList.Any())
                        {
                            var message = "The following images have no data to save:\n• " + string.Join("\n• ", errorList) + "\nCheck the data and try again.";
                            Notification.Show(message, "Save", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                            ImageList.ResetImages();
                            ResultString = "";
                            return false;
                        }
                    }
                    break;
                case 1:
                    {
                        if (ResultString.Length == 0)
                        {
                            Notification.Show($"No data to save", "Save", icon: NotificationIconType.Error);
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private void CopyCommand()
        {
            switch (ModeControl.SelectedMode)
            {
                case 0:
                    {
                        ProgramHelper.CopyFileToClipboard(ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile.ResultImage).ToList());
                        Notification.Show("Images copied", "Copy", icon: NotificationIconType.Success);
                    }
                    break;
                case 1:
                    {
                        ProgramHelper.CopyText(ResultString);
                        Notification.Show("Data copied", "Copy", icon: NotificationIconType.Success);
                    }
                    break;
            }
        }

        private bool CopyConfirmation()
        {
            switch (ModeControl.SelectedMode)
            {
                case 0:
                    {
                        var saveImages = ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList();

                        var errorList = saveImages
                            .Where(si => !File.Exists(si.ResultImage))
                            .Select(si => si.FileName)
                            .ToList();

                        if (errorList.Any())
                        {
                            var message = "The following images have no data to copy:\n• " + string.Join("\n• ", errorList) + "\nCheck the data and try again.";
                            Notification.Show(message, "Copy", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                            ImageList.ResetImages();
                            ResultString = "";
                            return false;
                        }
                        else if (saveImages.Count == 0)
                        {
                            var message = "The following images have no data to copy:\n• "
                                + string.Join("\n• ", ImageList.Images.Select(i => i.ImageFile.FileName).ToList());

                            Notification.Show(message, "Copy", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                            return false;
                        }
                    }
                    break;
                case 1:
                    {
                        if (string.IsNullOrEmpty(ResultString))
                        {
                            Notification.Show("No data to copy", "Copy", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private bool StopConfirmation()
            => Notification.Show("Stop the current operation?", "Stop operation",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void StopCommand()
        {
            Progress.ProgressTime = "Stopping...";
            Progress.StopTimer();
        }

        private bool StartConfirmation()
        {
            if (ModeControl.SelectedMode == 0 && string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(FilePath))
            {
                Notification.Show("No data to import", "Import", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                return false;
            }

            if (ImageList.Images.All(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("This will reset current progress. Continue?", "Start operation", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

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
                var res = Notification.Show("Are you sure you want to clear the list?", title: "Clear list", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

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

                _isProcessing = true;
                SettingsHeight = new GridLength(0, GridUnitType.Star);

                ImageList.ResetImages();
                ResultString = "";
                SetToolStatus("In progress");

                Progress.StartTimer();

                int totalItems = ImageList.Images.Count;

                double totalPixels = ImageList.Images.Sum(i => (double)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight));

                var hashPassword = ProgramHelper.GetHash32(PasswordBox.Password ?? "");

                switch (ModeControl.SelectedMode)
                {
                    case 0:
                        if (!await Import(totalPixels, hashPassword, token)) return;
                        break;
                    case 1:
                        if (!await Export(totalPixels, hashPassword, token)) return;
                        break;
                }

                if (token.IsCancellationRequested)
                {
                    Notification.Show("Operation stopped", "Operation", icon: NotificationIconType.Question);
                }
                else
                {
                    Notification.Show("Operation completed", "Operation", icon: NotificationIconType.Success);
                    SetToolStatus("Completed");
                }
            }
            finally
            {
                Progress.StopTimer();
                TaskControl.FinishCommand();
                IsEnable = true;
                IsReadOnly = !string.IsNullOrEmpty(FilePath);
                ImageList.IsEnable = IsEnable;
                _isProcessing = false;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
            }
        }

        private async Task<bool> Export(double totalPixels, string passwordHash, CancellationToken token)
        {
            ResultString = "";
            var extractedBinaryParts = new List<string>();
            var successfullyProcessedImages = new List<ImageFile>();

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
                    SetToolStatus($"Completed {Progress.ProgressPercent}");
                    imageItem.Status = StatusType.Success;
                }
                catch (OperationCanceledException)
                {
                    imageItem.Status = StatusType.None;
                    Notification.Show("Operation stopped", "Operation", icon: NotificationIconType.Question);
                    ImageList.ResetImages();
                    ResultString = "";
                    SetToolStatus();
                    return false;
                }
                catch (Exception ex)
                {
                    imageItem.Status = StatusType.Failed;
                    Notification.Show($"Failed to export:\n{ex.Message}", "Export error", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    ImageList.ResetImages();
                    ResultString = "";
                    SetToolStatus($"Error");
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
                    var items = decodedTextParts
                        .Select(el => el.Contains("[i]") ? el.Split("[i]")[1] : el);

                    foreach (var item in items)
                        allData.Append(item);
                }
                else
                {
                    var ordered = decodedTextParts
                        .Select(el =>
                        {
                            var parts = el.Split("[i]");
                            return parts.Length > 1 ? parts : new[] { "0", el };
                        })
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

                    var fileName = Encoding.UTF8.GetString(Convert.FromBase64String(fileMetadataParts[0]));
                    var fileExtension = Encoding.UTF8.GetString(Convert.FromBase64String(fileMetadataParts[1]));

                    var shouldAssembleFile = Notification.Show("The exported data is a file\nBuild the file?", "Export data", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

                    if (shouldAssembleFile)
                    {
                        var saveRes = FileHelper.SaveDataToFile(fileName, $"File (*{fileExtension})|*{fileExtension}", Convert.FromBase64String(ResultString));

                        if (saveRes.Result.IsSuccessResult)
                        {
                            string fileData = (await Task.Run(() => File.ReadAllText(saveRes.FilePath))) ?? string.Empty;

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
                    FilePath = "";
                    ResultString = Content;
                }
            }
            catch (Exception ex)
            {
                Notification.Show($"Failed to build data.\nError: {ex.Message}", "Export error", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                ImageList.ResetImages();
                ResultString = "";
                SetToolStatus($"");
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
                    var encodedName = Convert.ToBase64String(Encoding.UTF8.GetBytes(sourceFileInfo.Name));
                    var encodedExtension = Convert.ToBase64String(Encoding.UTF8.GetBytes(sourceFileInfo.Extension));
                    var fileBytes = await Task.Run(() => File.ReadAllBytes(FilePath));
                    dataToHide = $"{encodedName}[d]{encodedExtension}[d]" + Encryption.EncryptText(Convert.ToBase64String(fileBytes), passwordHash);
                }

                var imageCapacities = ImageList.Images.Select(i => (int)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight * 0.045)).ToList();
                var dataDistributionPlan = ProgramHelper.DistributeData(imageCapacities, dataToHide.Length);

                if (dataDistributionPlan == null)
                {
                    Notification.Show($"The data is too large for the selected images", "Import", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus();
                    return false;
                }

                dataChunks = ProgramHelper.SplitString(dataToHide, dataDistributionPlan);

                if (dataChunks == null)
                {
                    Notification.Show($"Failed to prepare data for import", "Import", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Notification.Show($"Failed to import:\n{ex.Message}", "Import error", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                SetToolStatus($"Error");
                return false;
            }

            for (int i = 0; i < ImageList.Images.Count; i++)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    ImageList.Images[i].Status = StatusType.InProgress;
                    ImageList.SelectedImage = ImageList.Images[i];

                    var content = IsIndexDependence ? dataChunks[i] : $"{i}[i]{dataChunks[i]}";
                    var binaryDataToWrite = Converter.ConvertTextToBinaryString(content);
                    var importTask = ImageHelper.ImportDataToImage(binaryDataToWrite, ImageList.Images[i].ImageFile.FilePath);
                    var cancelTask = Task.Delay(Timeout.Infinite, token);
                    var completedTask = await Task.WhenAny(importTask, cancelTask);

                    if (completedTask == cancelTask)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    var importBitmap = await importTask;
                    ImageList.Images[i].ImageFile.ResultImage = (await Task.Run(() => FileHelper.SaveBitmapToFolder(null, importBitmap)))?.FirstOrDefault();

                    completedImages.Add(ImageList.Images[i].ImageFile);
                    dataChunks[i] = "";
                    double convertedPixels = completedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                    Progress.UpdateTimer(convertedPixels, totalPixels);
                    SetToolStatus($"Completed {Progress.ProgressPercent}");
                    ImageList.Images[i].Status = StatusType.Success;
                }
                catch (OperationCanceledException)
                {
                    ImageList.Images[i].Status = StatusType.None;
                    Notification.Show("Operation stopped", "Operation", icon: NotificationIconType.Question);
                    ImageList.ResetImages();
                    ResultString = "";
                    SetToolStatus();
                    return false;
                }
                catch (Exception ex)
                {
                    ImageList.Images[i].Status = StatusType.Failed;
                    Notification.Show($"Failed to import:\n{ex.Message}", "Import error", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    SetToolStatus($"Error");
                    return false;
                }
            }
            return true;
        }

        private void UpdateImageCount()
        {
            var showSettings = ImageList.Images.Count > 0 && !ImageList.IsImporting && !_isProcessing;
            var showTaskControl = ImageList.Images.Count > 0 && !ImageList.IsImporting;

            SettingsHeight = new GridLength(showSettings ? 1 : 0, showSettings ? GridUnitType.Auto : GridUnitType.Star);
            TaskControlHeight = new GridLength(showTaskControl ? 1 : 0, showTaskControl ? GridUnitType.Auto : GridUnitType.Star);

            SetToolStatus();
        }

        private void OnImageListPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageListViewModel.IsImporting))
                UpdateImageCount();
        }

    }
}
