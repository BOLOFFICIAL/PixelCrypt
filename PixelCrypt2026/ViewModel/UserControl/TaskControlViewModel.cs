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
        public event Func<bool> ConfirmationSaveRequested;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SaveCommand { get; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

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

        public GridLength WidthSave
        {
            get => _widthSave;
            set => Set(ref _widthSave, value);
        }

        public TaskControlViewModel()
        {
            StartCommand = new LambdaCommand(OnStartExecute, OnCanStart);
            StopCommand = new LambdaCommand(OnStopExecute, OnCanStop);
            SaveCommand = new LambdaCommand(OnSaveExecute, OnCanSave);

            _isProcessing = false;
        }

        public void FinishCommand()
        {
            _isProcessing = false;
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }

        private void OnStartExecute(object parameter)
        {
            if ((!ConfirmationStartRequested?.Invoke()) ?? false)
                return;

            _isProcessing = true;
            CancellationTokenSource = new CancellationTokenSource();

            StartRequested?.Invoke();
        }

        private void OnStopExecute(object parameter)
        {
            if ((!ConfirmationStopRequested?.Invoke()) ?? false)
                return;

            CancellationTokenSource?.Cancel();
            _isProcessing = false;

            StopRequested?.Invoke();
        }

        private void OnSaveExecute(object parameter)
        {
            if ((!ConfirmationSaveRequested?.Invoke()) ?? false)
                return;

            SaveRequested?.Invoke();
        }

        private bool OnCanStart(object parameter)
        {
            var res = (CanStart?.Invoke() ?? true) 
                && (!_isProcessing);

            WidthStart = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }

        private bool OnCanStop(object parameter)
        {
            var res = (CanStop?.Invoke() ?? true)
                && CancellationTokenSource != null
                && _isProcessing;

            WidthStop = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }

        private bool OnCanSave(object parameter)
        {
            var res = (CanSave?.Invoke() ?? true) 
                && (!_isProcessing);

            WidthSave = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }
    }
}