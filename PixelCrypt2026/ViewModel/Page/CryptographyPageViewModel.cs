using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private GridLength _progressHeight;
        private GridLength _settingsHeightHeight;
        private GridLength _taskControlHeight;
        private List<int> _comboBoxItem;
        private int _interference;
        private bool _isEncrypt;
        private GridLength _widthResultImage;
        private ImageSource _resultImageSource;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public PasswordBoxViewModel PasswordBox { get; set; }
        public TaskControlViewModel TaskControl { get; set; }

        public GridLength ProgressHeight
        {
            get => _progressHeight;
            set => Set(ref _progressHeight, value);
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

            _comboBoxItem = Enumerable.Range(0, 21)
                .Select(i => i * 5 == 0 ? 1 : i * 5)
                .Where(x => x <= 100)
                .ToList();

            Interference = ComboBoxItem.Last();

            ProgressHeight = new GridLength(0, GridUnitType.Star);
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
        }

        private void SelectImage()
        {
            if (ImageList.SelectedImage.ImageFile.ResultImageSource != null)
            {
                WidthResultImage = new GridLength(1, GridUnitType.Star);
                ResultImageSource = ImageList.SelectedImage.ImageFile.ResultImageSource;
            }
            else 
            {
                WidthResultImage = new GridLength(0, GridUnitType.Pixel);
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
            }
        }

        private bool CanSave() => ImageList.Images.Any(i => i.Status == StatusType.Success);

        private bool ClearConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("Вы уверены что хотите очистить список?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            return true;
        }

        private async void StartCommand()
        {
            var token = TaskControl.CancellationTokenSource.Token;

            ImageList.IsEnable = false;
            SettingsHeight = new GridLength(0, GridUnitType.Star);

            SetToolStatus("Выполняется");

            Progress.StartTimer();

            try
            {
                int totalItems = ImageList.Images.Count;

                if (totalItems > 1)
                {
                    ProgressHeight = new GridLength(1, GridUnitType.Auto);
                }

                int processedItems = 0;

                ImageList.ResetImages();

                var password = ProgramHelper.GetHash32(PasswordBox.Password ?? "");

                double totalPixels = ImageList.Images.Sum(i => (double)(i.ImageFile.ImageWidth * i.ImageFile.ImageHeight));

                if (_isEncrypt)
                {
                    await Encrypt(totalPixels, token, password);
                }
                else 
                {
                    await Decrypt(totalPixels, token, password);
                }

                if (token.IsCancellationRequested)
                {
                    Notification.Show("Операция остановлена");
                    Progress.ProgressTime = $"Остановлено ({processedItems}/{totalItems})";
                    SetToolStatus("Остановлено");
                }
                else
                {
                    Notification.Show("Операция завершена");
                    Progress.ProgressTime = "Завершено";
                    SetToolStatus("Завершено");
                }
            }
            catch (OperationCanceledException)
            {
                Progress.ProgressTime = "Операция отменена";
                SetToolStatus("Отменено");
            }
            catch (Exception ex)
            {
                Progress.ProgressTime = "Ошибка";
                SetToolStatus("Ошибка");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Progress.StopTimer();
                TaskControl.FinishCommand();
                ImageList.IsEnable = true;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
                ProgressHeight = new GridLength(0, GridUnitType.Star);
                SetToolStatus();
            }
        }

        internal async Task<ActionResult> Decrypt(double totalItems, CancellationToken token, string password)
        {
            var res = await Process(totalItems,token, password, Encryption.DecryptPhoto);
            res.ResultTitle = "Расшифрование";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно расшифрованы";

            return res;
        }

        internal async Task<ActionResult> Encrypt(double totalItems, CancellationToken token, string password)
        {
            var res = await Process(totalItems,token, password, Encryption.EncryptPhoto);
            res.ResultTitle = "Шифрование";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно зашифрованы";

            return res;
        }

        private async Task<ActionResult> Process(double totalItems, CancellationToken token, string password, Func<string, string,int, Task<Bitmap>> action)
        {
            var result = new ActionResult();
            var processedItems = 0;
            var completedImages = new List<ImageFile>();

            var hashPassword = ProgramHelper.GetHash32(password);

            try
            {
                foreach (var image in ImageList.Images)
                {
                    token.ThrowIfCancellationRequested();
                    image.Status = StatusType.InProgress;
                    ImageList.SelectedImage = image;
                    try
                    {
                        image.ImageFile.ResultImage = await action(image.ImageFile.FilePath, hashPassword, Interference);
                        image.ImageFile.ResultImageSource = await Task.Run(() => Converter.ConvertBitmapToImageSource(image.ImageFile.ResultImage));
                        image.Status = StatusType.Success;
                        completedImages.Add(image.ImageFile);
                        double convertedPixels = completedImages.Sum(i => (double)(i.ImageWidth * i.ImageHeight));
                        Progress.UpdateTimer(convertedPixels, totalItems);
                        SetToolStatus($"Выполняется ({Progress.ProgressPercent})");
                    }
                    catch (TaskCanceledException) 
                    {
                        image.Status = StatusType.None;
                        return result;
                    }
                    catch (Exception)
                    {
                        image.Status = StatusType.Failed;
                        return result;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                    ResultTitle = "",
                };
            }
        }

        private bool StartConfirmation()
        {
            _isEncrypt = true;

            if (Notification.Show(
                "Выберите желаемое действие",
                actions: new List<(string, Action)>()
                {
                        ("Encrypt", () => {_isEncrypt = true; }),
                        ("Decrypt",() => {_isEncrypt = false; })
                },
                icon: NotificationIconType.Question).Result == NotificationResultType.Cancel)
            {
                return false;
            }

            if (ImageList.Images.Any(i => i.Status == StatusType.Success))
            {
                var res = Notification.Show("Текущий прогресс будет потерян, продолжить?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                if (res.Result != NotificationResultType.Yes)
                    return false;
            }

            return true;
        }

        private void StopCommand()
        {
            Progress.ProgressTime = "Остановка...";
            ProgressHeight = new GridLength(0, GridUnitType.Star);
            Progress.StopTimer();
        }

        private bool StopConfirmation()
            => Notification.Show("Вы уверены что хотите остановить?",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void SaveCommand()
        {
            FileHelper.SaveBitmapToFolder(ImageList.Images.Select(i => i.ImageFile).ToList());
        }
    }
}