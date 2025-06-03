using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class SteganographyPageViewModel : ImagePageViewModel
    {
        private static Steganography _steganography;

        private bool _isReadOnlyInputData = false;

        public ICommand ChooseFileCommand { get; }
        public ICommand RemoveFileCommand { get; }

        public SteganographyPageViewModel() : base(new Steganography())
        {
            _steganography = ImagePage as Steganography;

            ChooseFileCommand = new LambdaCommand(OnChooseFileCommandExecuted);
            RemoveFileCommand = new LambdaCommand(OnRemoveFileCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            InputAction = _steganography.ImportAction;
            OutputAction = _steganography.ExportAction;
        }

        public string InputFileName
        {
            get => _steganography.InputFile.Name;
            set => Set(ref _steganography.InputFile.Name, value);
        }

        public string InputFilePath
        {
            get => _steganography.InputFile.Path;
            set
            {
                if (Set(ref _steganography.InputFile.Path, value))
                {
                    InputFileName = System.IO.Path.GetFileName(value);
                    IsReadOnlyInputData = value?.Length > 0;
                }
            }
        }

        public string InputData
        {
            get => _steganography.InputFile.Content;
            set => Set(ref _steganography.InputFile.Content, value);
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

        private void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Action action) return;
            action();
            SaveDataWidth = new GridLength(1, GridUnitType.Star);
        }

        private void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
            }
            else if (System.IO.File.Exists(parametr.Path))
            {
                SelecedImage = parametr;
                ViewImageWidth = new GridLength(4, GridUnitType.Star);
            }
            else
            {
                MessageBox.Show("Не удалось найти фаил, возможно он удален или перемещен");
                OnRemoveImageCommandExecuted(parametr);
            }
            FilePathImageStackPanel = UpdateImageList();
        }
    }
}
