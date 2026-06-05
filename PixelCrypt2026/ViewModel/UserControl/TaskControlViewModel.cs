using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    internal class TaskControlViewModel : BaseViewModel
    {
        private bool _isProcessing = false;
        private GridLength _widthStart;
        private GridLength _widthStop;
        private GridLength _widthSave;

        public event Func<bool> CanStart;
        public event Func<bool> CanStop;
        public event Func<bool> CanSave;
        public event Action StartRequested;
        public event Action StopRequested;
        public event Action SaveRequested;
        public event Func<bool> ConfirmationStartRequested;
        public event Func<bool> ConfirmationStopRequested;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SaveCommand { get; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public GridLength WidthStart
        {
            get => _widthStart;
            set
            {
                Set(ref _widthStart, value);

                if (_widthStart.Value == 1)
                {
                    WidthStop = new GridLength(0, GridUnitType.Star);
                }
            }
        }

        public GridLength WidthStop
        {
            get => _widthStop;
            set
            {
                Set(ref _widthStop, value);

                if (_widthStop.Value == 1)
                {
                    WidthStart = new GridLength(0, GridUnitType.Star);
                }
            }
        }

        public GridLength WidthSave
        {
            get => _widthSave;
            set
            {
                Set(ref _widthSave, value);
            }
        }
        public bool IsProcessing
        {
            get => _isProcessing;
            private set
            {
                _isProcessing = value;

                if (_isProcessing)
                {
                    WidthStop = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    WidthStart = new GridLength(1, GridUnitType.Star);
                }
            }
        }

        public TaskControlViewModel()
        {
            StartCommand = new LambdaCommand(OnStartExecute, OnCanStart);
            StopCommand = new LambdaCommand(OnStopExecute, OnCanStop);
            SaveCommand = new LambdaCommand(OnSaveExecute, OnCanSave);

            IsProcessing = false;
        }

        private void OnStartExecute(object parameter)
        {
            if ((!ConfirmationStartRequested?.Invoke()) ?? false)
                return;

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
            if ((!ConfirmationStopRequested?.Invoke()) ?? false)
                return;

            CancellationTokenSource?.Cancel();
            StopRequested?.Invoke();
            IsProcessing = false;
        }

        private bool OnCanStop(object parameter)
            => CanStop?.Invoke() ?? true
            && CancellationTokenSource != null
            && IsProcessing;

        private void OnSaveExecute(object parameter)
        {
            SaveRequested?.Invoke();
        }

        private bool OnCanSave(object parameter)
        {
            var res = CanSave?.Invoke() ?? true;

            WidthSave = new GridLength(res && !IsProcessing ? 1 : 0, GridUnitType.Star);

            return res;
        }
    }
}