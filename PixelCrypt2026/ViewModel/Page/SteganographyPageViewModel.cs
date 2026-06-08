using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.View.UserControl;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            TaskControl.CanSave += () => ImageList.Images.All(i => i.Status == Status.Success);
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
                    var res = MessageBox.Show("Тест будет заменен на содержимое из файла, продолжить?", "", MessageBoxButton.YesNo);

                    if (res != MessageBoxResult.Yes) return;
                }

                FilePath = openFileDialog.FileName;
            }
        }

        private void OnClearFileCommand(object obj)
        {
            FilePath = "";
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
                Set(ref _filePath, value);
                IsReadOnly = !string.IsNullOrEmpty(_filePath);
                if (!string.IsNullOrEmpty(_filePath)) 
                {
                    Content = File.ReadAllText(FilePath);
                }
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
            MessageBox.Show($"Сохранение изображений {ImageList.Images.Count(i => i.Status == Status.Success)}");
        }

        private bool StopConfirmation()
            => MessageBox.Show("Вы уверены что хотите остановить?",
                "",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;

        private void StopCommand()
        {
            
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

                    el.Status = Status.InProgress;

                    try
                    {
                        await Task.Delay(500, token);
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
