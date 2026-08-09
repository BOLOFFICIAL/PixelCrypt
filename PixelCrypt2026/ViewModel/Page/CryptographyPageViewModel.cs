using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private GridLength _settingsHeightHeight;
        private GridLength _taskControlHeight;
        private List<int> _comboBoxItem;
        private int _interference;
        private GridLength _widthResultImage;
        private ImageSource _resultImageSource;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public PasswordBoxViewModel PasswordBox { get; set; }
        public TaskControlViewModel TaskControl { get; set; }
        public ModeControlViewModel ModeControl { get; set; }

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

            ImageList = new ImageListViewModel();

            UpdateImageCount();

            ImageList.ConfirmationClearRequested += ClearConfirmation;
            ImageList.AddRequested += UpdateImageCount;
            ImageList.ClearRequested += UpdateImageCount;
            ImageList.RemoveRequested += UpdateImageCount;
            ImageList.SelectImage += SelectImage;

            TaskControl = new TaskControlViewModel();

            TaskControl.StartRequested += StartCommand;
            TaskControl.CanStart += () => ImageList.Images.Count > 0;
            TaskControl.ConfirmationStartRequested += StartConfirmation;

            TaskControl.StopRequested += StopCommand;
            TaskControl.ConfirmationStopRequested += StopConfirmation;

            TaskControl.SaveRequested += SaveCommand;
            TaskControl.CanSave += CanSave;

            TaskControl.CopyRequested += CopyCommand;

            ModeControl = new ModeControlViewModel(new List<string>() { "Encrypt", "Decrypt" });
        }

        private void SelectImage()
        {
            var resultPath = ImageList.SelectedImage.ImageFile.ResultImage;
            if (string.IsNullOrEmpty(resultPath) || !File.Exists(resultPath))
            {
                WidthResultImage = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                WidthResultImage = new GridLength(1, GridUnitType.Star);

                var bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;  
                bmpImage.UriSource = new Uri(resultPath);
                bmpImage.EndInit();
                bmpImage.Freeze();                                

                ResultImageSource = bmpImage;
            }
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
                WidthResultImage = new GridLength(0, GridUnitType.Pixel);
            }

            SetToolStatus();
        }

        private bool CanSave() => ImageList.Images.Any(i => i.Status == StatusType.Success);

        private bool ClearConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("Are you sure you want to clear the list?", title: "List clearing", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

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
            SettingsHeight = new GridLength(0, GridUnitType.Star);

            ImageList.ResetImages();
            SetToolStatus("In progress");

            Progress.StartTimer();

            try
            {
                var password = ProgramHelper.GetHash32(PasswordBox.Password ?? "");

                double totalPixels = ImageList.Images.Sum(i => (double)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight));

                var processResult = false;

                switch (ModeControl.SelectedMode)
                {
                    case 0:
                        processResult = await Process(totalPixels, token, password, Encryption.EncryptPhoto);
                        break;
                    case 1:
                        processResult = await Process(totalPixels, token, password, Encryption.DecryptPhoto);
                        break;
                }

                if (token.IsCancellationRequested)
                {
                    Notification.Show("Operation stopped", icon: NotificationIconType.Question);
                    SetToolStatus();
                }
                else if (processResult)
                {
                    Notification.Show("Operation completed", icon: NotificationIconType.Success);
                    SetToolStatus("Completed");
                }
            }
            finally
            {
                Progress.StopTimer();
                TaskControl.FinishCommand();
                ImageList.IsEnable = true;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
            }
        }

        private async Task<bool> Process(double totalItems, CancellationToken token, string password, Func<string, string, int, Task<Bitmap>> action)
        {
            var completedImages = new List<ImageFile>();

            var hashPassword = ProgramHelper.GetHash32(password);

            foreach (var image in ImageList.Images)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    image.Status = StatusType.InProgress;
                    ImageList.SelectedImage = image;

                    var processTask = action(image.ImageFile.FilePath, hashPassword, Interference);

                    var cancelTask = Task.Delay(Timeout.Infinite, token);

                    var completedTask = await Task.WhenAny(processTask, cancelTask);

                    if (completedTask == cancelTask)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    image.ImageFile.ResultImage = FileHelper.SaveBitmapToFolder(null, await processTask)?.FirstOrDefault();

                    image.Status = StatusType.Success;
                    completedImages.Add(image.ImageFile);

                    double convertedPixels = completedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                    Progress.UpdateTimer(convertedPixels, totalItems);
                    SelectImage();
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
                    Notification.Show($"Error: {ex.Message}", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
                    return false;
                }
            }

            return true;
        }

        private bool StartConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("This will reset current progress. Continue?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

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
            => Notification.Show("Stop the current operation?",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void SaveCommand()
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
                var resMessage = Notification.Show(message, button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (resMessage.Result == NotificationResultType.Yes) return;
            }

            var res = FileHelper.SaveImageToFolder(saveImages);

            if (res.IsSuccessResult)
            {
                Notification.Show(res.ResultMessage, button: NotificationButtonType.Ok, icon: NotificationIconType.Success);
                ImageList.ResetImages();
                SetToolStatus();
            }
            else
            {
                Notification.Show(res.ResultMessage, button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
            }
        }

        private void CopyCommand() 
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
                var resMessage = Notification.Show(message, button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (resMessage.Result == NotificationResultType.Yes) return;
            }

            ProgramHelper.CopyFileToClipboard(saveImages.Where(i => File.Exists(i.ResultImage)).Select(i => i.ResultImage).ToList());
            Notification.Show("Images copied", icon: NotificationIconType.Success);
        }
    }
}