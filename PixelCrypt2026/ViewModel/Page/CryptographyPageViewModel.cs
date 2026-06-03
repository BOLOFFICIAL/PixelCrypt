using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Windows;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private GridLength _progressHeight;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }
        public TaskControlViewModel TaskControl { get; set; }

        public GridLength ProgressHeight
        {
            get => _progressHeight;
            set => Set(ref _progressHeight, value);
        }

        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            ProgressHeight = new GridLength(0, GridUnitType.Star);
            Title = $"Шифрование";
            ImageList = new ImageListViewModel();
            Progress = new ProgressPanelViewModel();
            TaskControl = new TaskControlViewModel();

            TaskControl.StartRequested += StartCommand;
            TaskControl.StopRequested += StopCommand;
            TaskControl.CanStart += () => ImageList.Images.Count > 0;
        }

        private async void StartCommand()
        {
            var token = TaskControl.CancellationTokenSource.Token;

            ImageList.IsEnable = false;

            SetStatus("Выполняется");

            ProgressHeight = new GridLength(1, GridUnitType.Auto);

            try
            {
                int totalItems = ImageList.Images.Count;
                int processedItems = 0;

                Progress.ProgressValue = 0;
                Progress.ProgressTime = "Вычисление...";

                DateTime startTime = DateTime.Now;

                foreach (var el in ImageList.Images)
                {
                    token.ThrowIfCancellationRequested();

                    ImageList.SelectedImage = el;

                    try
                    {
                        await Task.Delay(1000, token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }

                    processedItems++;

                    double progressPercent = (double)processedItems / totalItems * 100;
                    Progress.ProgressValue = progressPercent;

                    TimeSpan elapsed = DateTime.Now - startTime;
                    double avgTimePerItem = elapsed.TotalSeconds / processedItems;
                    int remainingItems = totalItems - processedItems;
                    double estimatedRemainingSeconds = avgTimePerItem * remainingItems;

                    TimeSpan remaining = TimeSpan.FromSeconds(estimatedRemainingSeconds);
                    Progress.ProgressTime = $"{(int)remaining.TotalHours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";

                    SetStatus($"Выполняется {processedItems}/{totalItems} ({progressPercent:F0}%)");
                }

                if (token.IsCancellationRequested)
                {
                    MessageBox.Show("Операция остановлена");
                    Progress.ProgressTime = $"Остановлено ({processedItems}/{totalItems})";
                    SetStatus("Остановлено");
                }
                else
                {
                    MessageBox.Show("Операция завершена");
                    Progress.ProgressValue = 100;
                    Progress.ProgressTime = "Завершено";
                    SetStatus("Завершено");
                }
            }
            catch (OperationCanceledException)
            {
                Progress.ProgressTime = "Операция отменена";
                SetStatus("Отменено");
            }
            catch (Exception ex)
            {
                Progress.ProgressTime = "Ошибка";
                SetStatus("Ошибка");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                TaskControl.FinishCommand();
                ImageList.IsEnable = true;
                ProgressHeight = new GridLength(0, GridUnitType.Star);
                SetStatus();
            }
        }

        private void StopCommand()
        {
            Progress.ProgressTime = "Остановка...";
            ProgressHeight = new GridLength(0, GridUnitType.Star);
        }
    }
}