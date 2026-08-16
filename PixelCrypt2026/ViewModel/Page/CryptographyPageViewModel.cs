using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private GridLength _settingsHeight;
        private GridLength _taskControlHeight;
        private List<int> _comboBoxItem;
        private int _interference;
        private GridLength _widthResultImage;
        private ImageSource _resultImageSource;
        private bool _isProcessing;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public PasswordBoxViewModel PasswordBox { get; set; }
        public TaskControlViewModel TaskControl { get; set; }
        public ModeControlViewModel ModeControl { get; set; }

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

        public GridLength WidthResultImage
        {
            get => _widthResultImage;
            set => Set(ref _widthResultImage, value);
        }

        public ImageSource ResultImageSource
        {
            get => _resultImageSource;
            set => Set(ref _resultImageSource, value);
        }

        public List<int> ComboBoxItem => _comboBoxItem;

        public int Interference
        {
            get => _interference;
            set => Set(ref _interference, value);
        }

        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Cryptography";

            _comboBoxItem = Enumerable.Range(1, 10).Select(i => i * 10).ToList();

            Interference = ComboBoxItem.Last();

            WidthResultImage = new GridLength(0, GridUnitType.Pixel);

            Progress = new ProgressPanelViewModel();
            PasswordBox = new PasswordBoxViewModel();

            ImageList = new ImageListViewModel()
            {
                Add = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount
                },
                Clear = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount,
                    ConfirmationRequired = ClearConfirmation,
                },
                Remove = new ControlAction()
                {
                    ExecuteRequested = UpdateImageCount
                },
                SelectImage = new ControlAction()
                {
                    ExecuteRequested = async () => await SelectImage()
                }
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
                    CanExecute = () => ImageList.Images.Any(i => i.Status == StatusType.Success),
                },
                Copy = new ControlAction()
                {
                    ExecuteRequested = CopyCommand,
                    ConfirmationRequired = CopyConfirmation,
                }
            };

            ModeControl = new ModeControlViewModel(new List<string>() { "Encrypt", "Decrypt" });
        }

        private void OnImageListPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageListViewModel.IsImporting))
                UpdateImageCount();
        }

        private async Task SelectImage()
        {
            var resultPath = ImageList?.SelectedImage?.ImageFile.ResultImage;

            if (string.IsNullOrEmpty(resultPath) || !File.Exists(resultPath))
            {
                WidthResultImage = new GridLength(0, GridUnitType.Pixel);
                return;
            }

            WidthResultImage = new GridLength(1, GridUnitType.Star);

            var bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.UriSource = new Uri(resultPath);
            bmpImage.EndInit();
            bmpImage.Freeze();

            ResultImageSource = bmpImage;
        }

        private void UpdateImageCount()
        {
            var showSettings = ImageList.Images.Count > 0 && !ImageList.IsImporting && !_isProcessing;
            var showTaskControl = ImageList.Images.Count > 0 && !ImageList.IsImporting;

            SettingsHeight = new GridLength(showSettings ? 1 : 0, showSettings ? GridUnitType.Auto : GridUnitType.Star);
            TaskControlHeight = new GridLength(showTaskControl ? 1 : 0, showTaskControl ? GridUnitType.Auto : GridUnitType.Star);

            if (ImageList.Images.Count == 0)
                WidthResultImage = new GridLength(0, GridUnitType.Pixel);

            SetToolStatus();
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
            var token = TaskControl.CancellationTokenSource.Token;

            ImageList.IsEnable = false;
            _isProcessing = true;
            SettingsHeight = new GridLength(0, GridUnitType.Star);

            ImageList.ResetImages();
            SetToolStatus("In progress");

            Progress.StartTimer();

            try
            {
                var hashPassword = ProgramHelper.GetHash32(PasswordBox.Password ?? "");

                double totalPixels = ImageList.Images.Sum(i => (double)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight));

                var processResult = false;

                switch (ModeControl.SelectedMode)
                {
                    case 0:
                        processResult = await Process(totalPixels, token, hashPassword, Encryption.EncryptPhoto);
                        break;
                    case 1:
                        processResult = await Process(totalPixels, token, hashPassword, Encryption.DecryptPhoto);
                        break;
                }

                if (token.IsCancellationRequested)
                {
                    Notification.Show("Operation stopped", "Operation", icon: NotificationIconType.Question);
                    SetToolStatus();
                }
                else if (processResult)
                {
                    Notification.Show("Operation completed", "Operation", icon: NotificationIconType.Success);
                    SetToolStatus("Completed");
                }
            }
            finally
            {
                Progress.StopTimer();
                TaskControl.FinishCommand();
                ImageList.IsEnable = true;
                _isProcessing = false;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
            }
        }

        private async Task<bool> Process(double totalItems, CancellationToken token, string password, Func<string, string, int, Task<Bitmap>> action)
        {
            var completedImages = new List<ImageFile>();

            foreach (var image in ImageList.Images)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    image.Status = StatusType.InProgress;
                    ImageList.SelectedImage = image;

                    var processTask = action(image.ImageFile.FilePath, password, Interference);

                    var cancelTask = Task.Delay(Timeout.Infinite, token);

                    var completedTask = await Task.WhenAny(processTask, cancelTask);

                    if (completedTask == cancelTask)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    var resultBitmap = await processTask;
                    image.ImageFile.ResultImage = (await Task.Run(() => FileHelper.SaveBitmapToFolder(null, resultBitmap)))?.FirstOrDefault();

                    image.Status = StatusType.Success;
                    completedImages.Add(image.ImageFile);

                    double convertedPixels = completedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                    Progress.UpdateTimer(convertedPixels, totalItems);
                    await SelectImage();
                    SetToolStatus($"Completed {Progress.ProgressPercent}");
                }
                catch (OperationCanceledException)
                {
                    image.Status = StatusType.None;
                    return true;
                }
                catch (Exception ex)
                {
                    image.Status = StatusType.Failed;
                    SetToolStatus($"Error");
                    Notification.Show($"Failed to process image:\n{ex.Message}", "Processing error", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    return false;
                }
            }

            return true;
        }

        private bool StartConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("This will reset current progress. Continue?", "Start operation", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            return true;
        }

        private void StopCommand()
        {
            Progress.ProgressTime = "Stopping...";
            Progress.StopTimer();
        }

        private bool StopConfirmation()
            => Notification.Show("Stop the current operation?", "Stop operation",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void SaveCommand()
        {
            var res = FileHelper.SaveImageToFolder(ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList());

            if (res.IsSuccessResult)
            {
                Notification.Show(res.ResultMessage, "Save", button: NotificationButtonType.Ok, icon: NotificationIconType.Success);
                ImageList.ResetImages();
                SetToolStatus();
            }
            else
            {
                Notification.Show(res.ResultMessage, "Save", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
            }
        }

        private bool SaveConfirmation()
        {
            var saveImages = ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList();

            var errorList = new List<string>();

            foreach (var saveImage in saveImages)
            {
                if (File.Exists(saveImage.ResultImage))
                    continue;

                errorList.Add(saveImage.FileName);
            }

            if (errorList.Any())
            {
                var message = "The following images have no data to save:\n• " + string.Join("\n• ", errorList) + "\nCancel saving?";
                var resMessage = Notification.Show(message, "Save", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (resMessage.Result == NotificationResultType.Yes) return false;
            }

            return true;
        }

        private void CopyCommand()
        {
            var list = ImageList.Images
                .Where(i => i.ImageFile?.ResultImage != null && File.Exists(i.ImageFile.ResultImage))
                .Select(i => i.ImageFile.ResultImage)
                .ToList();

            ProgramHelper.CopyFileToClipboard(list);
            Notification.Show("Images copied", "Copy", icon: NotificationIconType.Success);
        }

        private bool CopyConfirmation()
        {
            var saveImages = ImageList.Images.Where(i => i.ImageFile.ResultImage != null).Select(i => i.ImageFile).ToList();

            var errorList = new List<string>();

            foreach (var saveImage in saveImages)
            {
                if (File.Exists(saveImage.ResultImage))
                    continue;

                errorList.Add(saveImage.FileName);
            }

            if (errorList.Any())
            {
                var message = "The following images have no data to copy:\n• " + string.Join("\n• ", errorList) + "\nCancel copying?";
                var resMessage = Notification.Show(message, "Copy", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (resMessage.Result == NotificationResultType.Yes) return false;
            }

            return true;
        }
    }
}