using PixelCrypt2026.ViewModel.Base;
using System;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ProgressPanelViewModel : BaseViewModel
    {
        private string _progressTime = "00:00:00";
        private string _progressPercent = "0%";
        private CancellationTokenSource _cts;
        private DateTime? _startTime = null;

        public TimeSpan Timer { get; set; }

        public string ProgressTime
        {
            get => _progressTime;
            set => Set(ref _progressTime, value);
        }

        public string ProgressPercent
        {
            get => _progressPercent;
            private set => Set(ref _progressPercent, value);
        }

        public void StartTimer()
        {
            ProgressTime = "";
            Timer = new TimeSpan();
            ProgressPercent = $"{0:F1}%";
            _startTime = DateTime.Now;
            _cts = new CancellationTokenSource();
            RunTimerAsync(_cts.Token);
        }
        public void UpdateTimer(int processedItems,int totalItems) 
        {
            if (_startTime is null || totalItems == 0 || processedItems == 0) return;

            double percentDone = processedItems * 100.0 / totalItems;

            TimeSpan elapsed = DateTime.Now - _startTime.Value;
            Timer = elapsed * (100.0 / percentDone) - elapsed;
            ProgressPercent = $"{percentDone:F1}%";
        }
        public void StopTimer()
        {
            _cts?.Cancel();
            _cts = null;
        }

        public async void RunTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (Timer > TimeSpan.Zero)
                {
                    Timer -= TimeSpan.FromSeconds(1);
                    ProgressTime = $"{Timer.Hours:D2}:{Timer.Minutes:D2}:{Timer.Seconds:D2}";
                }

                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
