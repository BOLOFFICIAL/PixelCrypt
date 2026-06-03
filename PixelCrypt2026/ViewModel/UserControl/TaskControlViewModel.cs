using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    internal class TaskControlViewModel : BaseViewModel
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public event Func<bool> CanStart;
        public event Func<bool> CanStop;
        public event Action StartRequested;
        public event Action StopRequested;

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        private GridLength _widthStart;
        private GridLength _widthStop;
        private bool _isProcessing = false;

        public bool IsProcessing
        {
            get => _isProcessing;
            private set
            {
                _isProcessing = value;

                if (_isProcessing)
                {
                    WidthStart = new GridLength(0, GridUnitType.Star);
                    WidthStop = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    WidthStart = new GridLength(1, GridUnitType.Star);
                    WidthStop = new GridLength(0, GridUnitType.Star);
                }
            }
        }

        public TaskControlViewModel()
        {
            StartCommand = new LambdaCommand(OnStartExecute, OnCanStart);
            StopCommand = new LambdaCommand(OnStopExecute, OnCanStop);

            IsProcessing = false;
        }

        private void OnStartExecute(object parameter)
        {
            IsProcessing = true;
            CancellationTokenSource = new CancellationTokenSource();
            StartRequested?.Invoke();
        }

        private bool OnCanStart(object parameter)
            => CanStart?.Invoke() ?? true
            && !IsProcessing;

        public void FinishCommand()
        {
            IsProcessing = false;
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }

        private void OnStopExecute(object parameter)
        {
            CancellationTokenSource?.Cancel();
            StopRequested?.Invoke();
            IsProcessing = false;
        }

        private bool OnCanStop(object parameter)
            => CanStop?.Invoke() ?? true
            && CancellationTokenSource != null
            && IsProcessing;

        public GridLength WidthStart
        {
            get => _widthStart;
            set => Set(ref _widthStart, value);
        }

        public GridLength WidthStop
        {
            get => _widthStop;
            set => Set(ref _widthStop, value);
        }
    }
}