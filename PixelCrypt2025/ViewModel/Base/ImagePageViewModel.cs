using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Enum;
using PixelCrypt2025.Interfaces;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt2025.ViewModel.Base
{
    internal class ImagePageViewModel : BaseViewModel
    {
        #region fields

        private string _showPasword = "";
        private string _password = "";
        private string _imageName = "";
        private string _imagePath = "";
        private string _imageLength = "";
        private string _imageExtension = "";
        private string _imagePermission = "";
        private string _progress = "";
        private string _timerStop;

        private TimeSpan _timer;

        private CancellationTokenSource _cts;

        protected DateTime? start = null;

        private Model.Image _selectedImage = null;

        private GridLength _closePasswordWidth = Constants.GridLengthZero;
        private GridLength _openPasswordWidth = Constants.GridLengthZero;
        private GridLength _saveDataWidth = Constants.GridLengthZero;
        private GridLength _addGridHeight = Constants.GridLengthZero;
        private GridLength _dataGridHeight = Constants.GridLengthZero;
        private GridLength _viewImageWidth = Constants.GridLengthZero;
        private GridLength _progressWidth = Constants.GridLengthZero;
        private GridLength _actionWidth = Constants.GridLengthAuto;

        private bool _isOpenPassword = false;
        private bool _isSuccessResult = false;
        private bool _isButtonFree = true;

        protected ICommand ShowImageCommand { get; init; }
        protected ICommand RemoveImageCommand { get; init; }

        private ICommand MoveUpImageCommand { get; }
        private ICommand MoveDownImageCommand { get; }

        private Func<string, Task<ActionResult>> _inputAction;
        private Func<string, Task<ActionResult>> _outputAction;

        public IImagePage ImagePage { get; init; }

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand PaswordViewCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand DoActionCommand { get; init; }
        public ICommand SaveCommand { get; }

        #endregion

        public ImagePageViewModel()
        {
            Images = new ObservableCollection<Model.Image>();

            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);
            ClearImageCommand = new LambdaCommand(OnClearImageCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            MoveUpImageCommand = new LambdaCommand(OnMoveUpImageCommandExecuted);
            MoveDownImageCommand = new LambdaCommand(OnMoveDownImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);

            OnPaswordViewCommandExecuted();

            AddGridHeight = Constants.GridLengthStar;
        }

        #region Properties

        public bool IsButtonFree
        {
            get => _isButtonFree;
            set => Set(ref _isButtonFree, value);
        }

        protected bool IsSuccessResult
        {
            get => _isSuccessResult;
            set
            {
                Set(ref _isSuccessResult, value);
                if (!value)
                {
                    ImagePage.OutputImage.Clear();
                }
                SaveDataWidth = value ? Constants.GridLengthAuto : Constants.GridLengthZero;
            }
        }

        public Func<string, Task<ActionResult>> InputAction
        {
            get => _inputAction;
            set => Set(ref _inputAction, value);
        }

        public Func<string, Task<ActionResult>> OutputAction
        {
            get => _outputAction;
            set => Set(ref _outputAction, value);
        }

        public ObservableCollection<Model.Image> Images { get; }

        public string ShowPasword
        {
            get => _showPasword;
            set => Set(ref _showPasword, value);
        }

        public string TimerStop
        {
            get => _timerStop;
            set => Set(ref _timerStop, value);
        }

        public string Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string ImageName
        {
            get => _imageName;
            set => Set(ref _imageName, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => Set(ref _imagePath, value);
        }

        public string ImageLength
        {
            get => _imageLength;
            set => Set(ref _imageLength, value);
        }

        public string ImageExtension
        {
            get => _imageExtension;
            set => Set(ref _imageExtension, value);
        }

        public string ImagePermission
        {
            get => _imagePermission;
            set => Set(ref _imagePermission, value);
        }

        public GridLength ClosePasswordWidth
        {
            get => _closePasswordWidth;
            set => Set(ref _closePasswordWidth, value);
        }

        public GridLength ActionWidth
        {
            get => _actionWidth;
            set => Set(ref _actionWidth, value);
        }

        public GridLength OpenPasswordWidth
        {
            get => _openPasswordWidth;
            set => Set(ref _openPasswordWidth, value);
        }

        public GridLength SaveDataWidth
        {
            get => _saveDataWidth;
            set => Set(ref _saveDataWidth, value);
        }

        public GridLength AddGridHeight
        {
            get => _addGridHeight;
            set => Set(ref _addGridHeight, value);
        }

        public GridLength DataGridHeight
        {
            get => _dataGridHeight;
            set => Set(ref _dataGridHeight, value);
        }

        public GridLength ProgressWidth
        {
            get => _progressWidth;
            set => Set(ref _progressWidth, value);
        }

        public GridLength ViewImageWidth
        {
            get => _viewImageWidth;
            set => Set(ref _viewImageWidth, value);
        }

        public Model.Image SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (Set(ref _selectedImage, value))
                {
                    if (value != null)
                    {
                        ImageName = $"{_selectedImage.Name}";
                        ImagePath = $"{_selectedImage.Path}";
                        ImageLength = $"{_selectedImage.Length} Kb";
                        ImageExtension = $"{_selectedImage.Extension}";
                        ImagePermission = $"{_selectedImage.Permission}";
                    }
                    else
                    {
                        ImageName = "";
                        ImagePath = "";
                        ImageLength = "";
                        ImageExtension = "";
                        ImagePermission = "";
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        protected virtual void OnAddImageCommandExecuted(object p = null)
        {
            if (AccessReset("Добавление элемента приведет к потере рузльтата")) return;

            var filterList = new List<string>() { ".jpg", ".jpeg", ".png" };

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|" + string.Join(";", filterList.Select(ext => $"*{ext}")),
                Multiselect = true,
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                var prefCount = Images.Count;
                var imageList = openFileDialog.FileNames.Where(file => filterList.Contains(Path.GetExtension(file).ToLower()));

                foreach (var filepath in imageList) AddImage(filepath);

                if (Images.Count != prefCount)
                {
                    IsSuccessResult = false;
                    SaveDataWidth = Constants.GridLengthZero;

                    if (SelectedImage is null) OnShowImageCommandExecuted(Images.Last());
                }
                else if (imageList.Count() == 0)
                {
                    Notification.Show("Не удалось найти подходящие элементы", openFileDialog.Title, status: NotificationStatus.Error);
                }
            }
            if (Images.Count > 0)
            {
                AddGridHeight = Constants.GridLengthZero;
                DataGridHeight = Constants.GridLengthStar;
            }
        }

        private void OnClearImageCommandExecuted(object p = null)
        {
            if (AccessReset("Очистка списка приведет к потере рузльтата")) return;

            IsSuccessResult = false;
            Images.Clear();

            AddGridHeight = Constants.GridLengthStar;
            DataGridHeight = Constants.GridLengthZero;
            ViewImageWidth = Constants.GridLengthZero;

            SelectedImage = null;
        }

        private void OnPaswordViewCommandExecuted(object p = null)
        {
            if (_isOpenPassword)
            {
                OpenPasswordWidth = Constants.GridLengthStar;
                ClosePasswordWidth = Constants.GridLengthZero;
                ShowPasword = "Regular_Eye";
            }
            else
            {
                OpenPasswordWidth = Constants.GridLengthZero;
                ClosePasswordWidth = Constants.GridLengthStar;
                ShowPasword = "Regular_EyeSlash";
            }

            _isOpenPassword = !_isOpenPassword;
        }

        protected virtual void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            Images.Remove(parametr);
            ImagePage.OutputImage.Remove(parametr);

            if (SelectedImage == parametr)
            {
                SelectedImage = null;
                ViewImageWidth = Constants.GridLengthZero;
            }

            if (Images.Count == 0)
            {
                IsSuccessResult = false;
                AddGridHeight = Constants.GridLengthStar;
                DataGridHeight = Constants.GridLengthZero;
                SelectedImage = null;
            }
        }

        protected virtual void OnMoveUpImageCommandExecuted(object p = null)
        {
            if (p is Model.Image image) MoveImage(image, -1);
        }

        protected virtual void OnMoveDownImageCommandExecuted(object p = null)
        {
            if (p is Model.Image image) MoveImage(image, 1);
        }

        protected virtual void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (SelectedImage == parametr)
            {
                SelectedImage = null;
                ViewImageWidth = Constants.GridLengthZero;
            }
            else if (System.IO.File.Exists(parametr.Path))
            {
                SelectedImage = parametr;
                ViewImageWidth = Constants.GridLengthStar;
            }
            else
            {
                Notification.Show("Не удалось найти фаил, возможно он удален или перемещен", "Просмотр изображения", status: NotificationStatus.Error);
                OnRemoveImageCommandExecuted(parametr);
            }
        }

        private void OnSaveCommandExecuted(object obj)
        {
            var result = ImagePage.SaveData();

            var status = result.IsSuccessResult ? NotificationStatus.Success : NotificationStatus.Error;

            Notification.Show($"{result.ResultMessage}", result.ResultTitle, status: status);
        }

        private void AddImage(string filepath)
        {
            foreach (var inputImage in Images)
            {
                if (inputImage.Path == filepath) return;
            }

            var image = new Model.Image(filepath);

            if (image.Name.Length > 150)
            {
                Notification.Show($"Файл\n\n{image.Name}\n\nимеет слишком длинное имя, измените имя или замените файл", "Добавление изображения", status: NotificationStatus.Error);
                return;
            }

            Images.Add(image);
        }

        private void MoveImage(Model.Image image, int direction)
        {
            var index = Images.IndexOf(image);
            var newIndex = index + direction;

            if (image is null || index < 0 || newIndex < 0 || newIndex >= Images.Count) return;

            Images.Move(index, newIndex);
        }

        protected bool AccessReset(string message, string title = "PixelCrypt")
        {
#if DEBUG
            return false;
#endif
            return IsSuccessResult && Notification.Show($"{message}.\nПродолжить?", title, NotificationType.YesNo, status: NotificationStatus.Question).Result == NotificationResultType.No;
        }

        private void UpdateProgress()
        {
            if (start is null || Images.Count == 0) return;

            double totalPixels = Images.Sum(i => (double)(i.Width * i.Height));

            if (totalPixels == 0) return;

            double convertedPixels = ImagePage.OutputImage.Sum(i => (double)(i.Key.Width * i.Key.Height));
            double percentDone = convertedPixels * 100.0 / totalPixels;

            if (percentDone > 0)
            {
                TimeSpan elapsed = DateTime.Now - start.Value;
                _timer = elapsed * (100.0 / percentDone) - elapsed;
            }

            Progress = $"{percentDone:0.##} %";
        }

        protected void StartTimer()
        {
            TimerStop = "Анализ";
            _cts = new CancellationTokenSource();
            start = DateTime.Now;
            RunTimerAsync(_cts.Token);
        }

        protected void StopTimer()
        {
            _cts?.Cancel();
            _cts = null;
            start = null;
        }

        private async void RunTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_timer > TimeSpan.Zero)
                {
                    _timer -= TimeSpan.FromSeconds(1);
                    TimerStop = $"{_timer.Hours:D2}:{_timer.Minutes:D2}:{_timer.Seconds:D2}";
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

        #endregion
    }
}