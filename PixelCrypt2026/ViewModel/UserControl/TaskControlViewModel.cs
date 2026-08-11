using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
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


        public ControlAction Start { get; set; }
        public ControlAction Stop { get; set; }
        public ControlAction Save { get; set; }
        public ControlAction Copy { get; set; }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CopyCommand { get; }

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
            CopyCommand = new LambdaCommand(OnCopyExecute);

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
            if ((!Start?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            _isProcessing = true;
            CancellationTokenSource = new CancellationTokenSource();

            Start?.ExecuteRequested?.Invoke();
        }

        private void OnStopExecute(object parameter)
        {
            if ((!Stop?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            CancellationTokenSource?.Cancel();
            _isProcessing = false;

            Stop?.ExecuteRequested?.Invoke();
        }

        private void OnSaveExecute(object parameter)
        {
            if ((!Save?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            Save?.ExecuteRequested?.Invoke();
        }

        private void OnCopyExecute(object parameter)
        {
            if ((!Copy?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            Copy?.ExecuteRequested?.Invoke();
        }

        private bool OnCanStart(object parameter)
        {
            var res = (Start?.CanExecute?.Invoke() ?? true)
                && (!_isProcessing);

            WidthStart = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }

        private bool OnCanStop(object parameter)
        {
            var res = (Stop?.CanExecute?.Invoke() ?? true)
                && CancellationTokenSource != null
                && _isProcessing;

            WidthStop = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }

        private bool OnCanSave(object parameter)
        {
            var res = (Save?.CanExecute?.Invoke() ?? true)
                && (!_isProcessing);

            WidthSave = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }
    }
}