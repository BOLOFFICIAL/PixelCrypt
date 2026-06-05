using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Windows;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private GridLength _progressHeight;
        private GridLength _passwordHeight;
        private GridLength _taskControlHeight;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public PasswordBoxViewModel PasswordBox { get; set; }
        public TaskControlViewModel TaskControl { get; set; }

        public GridLength ProgressHeight
        {
            get => _progressHeight;
            set => Set(ref _progressHeight, value);
        }

        public GridLength PasswordHeight
        {
            get => _passwordHeight;
            set => Set(ref _passwordHeight, value);
        }

        public GridLength TaskControlHeight
        {
            get => _taskControlHeight;
            set => Set(ref _taskControlHeight, value);
        }

        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Cryptography";

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
            TaskControl.CanSave += () => ImageList.Images.Any(i => i.Status == Status.Success);
        }

        private void UpdateImageCount()
        {
            if (ImageList.Images.Count > 0)
            {
                PasswordHeight = new GridLength(1, GridUnitType.Auto);
                TaskControlHeight = new GridLength(1, GridUnitType.Auto);
            }
            else 
            {
                PasswordHeight = new GridLength(0, GridUnitType.Star);
                TaskControlHeight = new GridLength(0, GridUnitType.Star);
            }
        }

        private bool ClearConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == Status.Success))
            {
                var res = MessageBox.Show("Вы уверены что хотите очистить список?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res != MessageBoxResult.Yes)
                    return false;
            }

            return true;
        }

        private async void StartCommand()
        {
            var token = TaskControl.CancellationTokenSource.Token;

            ImageList.IsEnable = false;
            PasswordHeight = new GridLength(0, GridUnitType.Star);

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

                    el.Status = Status.InProgress;

                    try
                    {
                        await Task.Delay(1750, token);
                    }
                    catch (TaskCanceledException)
                    {
                        el.Status = Status.None;
                        break;
                    }

                    processedItems++;

                    el.Status = Status.Success;

                    ImageList.SelectedImage = el;

                    Progress.UpdateTimer(processedItems, totalItems);

                    SetToolStatus($"Выполняется {processedItems}/{totalItems} ({Progress.ProgressPercent})");
                }

                if (token.IsCancellationRequested)
                {
                    MessageBox.Show("Операция остановлена");
                    Progress.ProgressTime = $"Остановлено ({processedItems}/{totalItems})";
                    SetToolStatus("Остановлено");
                }
                else
                {
                    MessageBox.Show("Операция завершена");
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
                PasswordHeight = new GridLength(1, GridUnitType.Auto);
                ProgressHeight = new GridLength(0, GridUnitType.Star);
                SetToolStatus();
            }
        }

        private bool StartConfirmation()
        {
            if (ImageList.Images.Any(i => i.Status == Status.Success))
            {
                var res = MessageBox.Show("Текущий прогресс будет потерян, продолжить?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res != MessageBoxResult.Yes)
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
            => MessageBox.Show("Вы уверены что хотите остановить?",
                "",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question)
            == MessageBoxResult.Yes;

        private void SaveCommand()
        {
            MessageBox.Show($"Сохранение изображений {ImageList.Images.Count(i => i.Status == Status.Success)}");
        }
    }
}