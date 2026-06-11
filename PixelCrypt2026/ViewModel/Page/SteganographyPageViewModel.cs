using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.IO;
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

        private GridLength _progressHeight;
        private GridLength _settingsHeightHeight;
        private GridLength _taskControlHeight;
        private string _filePath;
        private string _content;
        private bool _isReadOnly;
        private bool _isEnable = true;

        public ICommand SelectFileCommand { get; }
        public ICommand ClearFileCommand { get; }

        public SteganographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Steganography";

            SelectFileCommand = new LambdaCommand(OnSelectFileCommand, CanSelectFile);
            ClearFileCommand = new LambdaCommand(OnClearFileCommand, CanClearFile);

            ProgressHeight = new GridLength(0, GridUnitType.Star);

            Progress = new ProgressPanelViewModel();
            PasswordBox = new PasswordBoxViewModel();

            ImageList = new ImageListViewModel();

            UpdateImageCount();

            ImageList.ConfirmationClearRequested += ClearConfirmation;
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
        }

        public bool IsEnable
        {
            get => _isEnable;
            set => Set(ref _isEnable, value);
        }

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
                else if (isEmpty && !string.IsNullOrEmpty(_filePath))
                {
                    var res = Notification.Show("Очистить содержимое?", button: NotificationButtonType.YesNo, icon: NotificationIconType.Question);

                    if (res.Result == NotificationResultType.Yes)
                    {
                        Content = "";
                    }
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

        private void SaveCommand()
        {
            Notification.Show($"Сохранение изображений {ImageList.Images.Count(i => i.Status == StatusType.Success)}");
        }

        private bool StopConfirmation()
            => Notification.Show("Вы уверены что хотите остановить?",
                button: NotificationButtonType.YesNo,
                icon: NotificationIconType.Question).Result == NotificationResultType.Yes;

        private void StopCommand()
        {
            Progress.ProgressTime = "Остановка...";
            ProgressHeight = new GridLength(0, GridUnitType.Star);
            Progress.StopTimer();
        }

        private bool StartConfirmation()
        {
            if (string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(FilePath))
            {
                Notification.Show("Нет данных для импорта", button: NotificationButtonType.Ok, icon: NotificationIconType.Error);
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

            IsEnable = false;
            IsReadOnly = true;
            ImageList.IsEnable = IsEnable;

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

                foreach (var el in ImageList.Images)
                {
                    token.ThrowIfCancellationRequested();

                    el.Status = StatusType.InProgress;

                    try
                    {
                        await Task.Delay(500, token);
                    }
                    catch (TaskCanceledException)
                    {
                        el.Status = StatusType.None;
                        break;
                    }

                    processedItems++;

                    el.Status = StatusType.Success;

                    ImageList.SelectedImage = el;

                    Progress.UpdateTimer(processedItems, totalItems);

                    SetToolStatus($"Выполняется ({Progress.ProgressPercent})");
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
                IsEnable = true;
                IsReadOnly = !string.IsNullOrEmpty(FilePath);
                ImageList.IsEnable = IsEnable;
                SettingsHeight = new GridLength(1, GridUnitType.Auto);
                ProgressHeight = new GridLength(0, GridUnitType.Star);
                SetToolStatus();
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
    }
}
