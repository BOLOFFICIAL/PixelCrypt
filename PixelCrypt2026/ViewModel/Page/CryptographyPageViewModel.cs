// FILE: ViewModel\Page\CryptographyPageViewModel.cs
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isProcessing;

        public ImageListViewModel ImageList { get; set; }
        public ProgressPanelViewModel Progress { get; set; }

        public ICommand DoCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Шифрование";
            ImageList = new ImageListViewModel();
            Progress = new ProgressPanelViewModel();

            DoCommand = new LambdaCommand(OnDoCommand, CanDoCommand);
            StopCommand = new LambdaCommand(OnStopCommand, CanStopCommand);
        }

        private async void OnDoCommand(object obj)
        {
            if (_isProcessing)
                return;

            _isProcessing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            ImageList.IsEnable = false;

            SetStatus("Выполняется");

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
                    Progress.ProgressTime = $"Остановлено ({processedItems}/{totalItems})";
                    SetStatus("Остановлено");
                }
                else
                {
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
                _isProcessing = false;
                ImageList.IsEnable = true;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                // Очищаем статус через 3 секунды
                await Task.Delay(3000);
                SetStatus();
            }
        }

        private bool CanDoCommand(object obj)
            => ImageList.Images.Count > 0;

        private void OnStopCommand(object obj)
        {
            _cancellationTokenSource?.Cancel();
            Progress.ProgressTime = "Остановка...";
        }

        private bool CanStopCommand(object obj)
            => _isProcessing && _cancellationTokenSource != null;
    }
}