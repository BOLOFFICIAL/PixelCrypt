using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class SteganographyPageViewModel : ImagePageViewModel
    {
        private Steganography _steganography = new Steganography();

        private bool _isReadOnlyInputData = false;

        public ICommand ChooseFileCommand { get; }
        public ICommand RemoveFileCommand { get; }

        public SteganographyPageViewModel()
        {
            ImagePage = _steganography;

            ChooseFileCommand = new LambdaCommand(OnChooseFileCommandExecuted);
            RemoveFileCommand = new LambdaCommand(OnRemoveFileCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);

            InputAction = _steganography.ImportAction;
            OutputAction = _steganography.ExportAction;

            OnAddImageCommandExecuted();
        }

        public string InputFileName
        {
            get => _steganography.DataFile.Name;
            set => Set(ref _steganography.DataFile.Name, value);
        }

        public string InputFilePath
        {
            get => _steganography.DataFile.Path;
            set
            {
                if (Set(ref _steganography.DataFile.Path, value))
                {
                    InputFileName = System.IO.Path.GetFileName(value);
                    IsReadOnlyInputData = value?.Length > 0;
                }
            }
        }

        public string InputData
        {
            get => _steganography.DataFile.Content;
            set => Set(ref _steganography.DataFile.Content, value);
        }

        public bool IsReadOnlyInputData
        {
            get => _isReadOnlyInputData;
            set => Set(ref _isReadOnlyInputData, value);
        }

        private void OnChooseFileCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выбрать файл для чтения данных";

            if (openFileDialog.ShowDialog() ?? false)
            {
                if (InputData == null || InputData?.Length == 0 || (InputData?.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "Файл для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    InputFilePath = openFileDialog.FileName;

                    var fileData = System.IO.File.ReadAllText(InputFilePath);

                    if (fileData.Length > 10000)
                    {
                        fileData = new string(fileData.Take(10000).ToArray());
                    }

                    InputData = fileData;
                }
            }
        }

        private void OnRemoveFileCommandExecuted(object p = null)
        {
            InputFilePath = "";
        }

        private async void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Func<string, Task> action) return;
            SaveDataWidth = Constants.GridLengthZero;
            await action(Password);
            SaveDataWidth = Constants.GridLengthStar;
            var content = _steganography.DataFile.Content;
            InputData = "";
            InputData = content;

            var path = _steganography.DataFile.Path;
            InputFilePath = "";
            InputFilePath = path;

            var file = _steganography.DataFile.Name;
            InputFileName = "";
            InputFileName = file;
        }
    }
}
