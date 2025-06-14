using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
using System.IO;
using System.Text;
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

            InputAction = Import;
            OutputAction = Export;

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
            set
            {
                if (AccessReset("Изменение данных приведет к потере рузльтата")) return;
                IsSuccessResult = false;
                Set(ref _steganography.DataFile.Content, value);
            }
        }

        public bool IsReadOnlyInputData
        {
            get => _isReadOnlyInputData;
            set => Set(ref _isReadOnlyInputData, value);
        }

        private void OnChooseFileCommandExecuted(object p = null)
        {
            if (AccessReset("Добавление файла приведет к потере рузльтата")) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выбрать файл для чтения данных";

            if (openFileDialog.ShowDialog() ?? false)
            {
                if (InputData == null || InputData?.Length == 0 || (InputData?.Length > 0 && MessageBox.Show("Заменить текст на данные из файла?", "Файл для чтения данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    InputFilePath = openFileDialog.FileName;

                    IsSuccessResult = false;

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
            if (InputFilePath.Length != 0) 
            {
                if (AccessReset("Удаление файла приведет к потере рузльтата")) return;
                IsSuccessResult = false;
            }
            InputFilePath = "";
        }

        private async void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Func<Task<bool>> doAction) return;
            SaveDataWidth = Constants.GridLengthZero;
            if (SelecedImage != null)
            {
                OnShowImageCommandExecuted(SelecedImage);
            }
            IsSuccessResult = false;
            IsReadOnlyInputData = true;
            IsButtonFree = false;
            isProcessing = true;

            if (await doAction())
            {
                IsSuccessResult = true;
                IsReadOnlyInputData = InputFilePath?.Length > 0;
            }
            else
            {
                MessageBox.Show("Возникла ошибка");
                IsSuccessResult = false;
            }

            OnPropertyChanged("InputData");
            isProcessing = false;
            IsButtonFree = true;
            UpdateList();
        }

        protected override void OnMoveUpImageCommandExecuted(object p = null)
        {
            if (AccessReset("Изменение порядка приведет к потере рузльтата")) return;
            IsSuccessResult = false;
            base.OnMoveUpImageCommandExecuted(p);
        }

        protected override void OnMoveDownImageCommandExecuted(object p = null)
        {
            if (AccessReset("Изменение порядка приведет к потере рузльтата")) return;
            IsSuccessResult = false;
            base.OnMoveDownImageCommandExecuted(p);
        }

        private async Task<bool> Export()
        {
            InputData = "";
            InputFilePath = "";
            saveAction = _steganography.SaveExport;

            var hashPassword = ProgramHelper.GetHash32(Password);

            var bynaryData = new List<string>();
            _steganography.OutputImage.Clear();
            await UpdateList();

            try
            {
                foreach (var filePathImage in _steganography.InputImage)
                {
                    bynaryData.Add(await ImageHelper.ExportDataFromImage(filePathImage.Path));
                    _steganography.OutputImage.Add(filePathImage, null);
                    OnShowImageCommandExecuted(filePathImage);
                    await UpdateList();
                }

                var allData = new StringBuilder();

                foreach (var item in bynaryData)
                {
                    allData.Append(item);
                }

                var exportData = Converter.ConvertBinaryStringToText(allData.ToString());

                var exportFileData = exportData.Split("[*]");

                if (exportFileData.Length > 1)
                {
                    exportFileData[2] = CryptoService.DecryptText(exportFileData[2], hashPassword);

                    if (MessageBox.Show("Экспортированные данные являются файлом.\nСформировать файл?", "Экспорт данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var res = ProgramHelper.SaveDataToFile(exportFileData[0], $"Файлы (*{exportFileData[1]})|*{exportFileData[1]}", Convert.FromBase64String(exportFileData[2]));
                        if (res.Result)
                        {
                            MessageBox.Show($"Фаил {res.FileName} сохранен", "Экспорт данных");

                            var fileData = System.IO.File.ReadAllText(res.FilePath);

                            if (fileData.Length > 10000)
                            {
                                fileData = new string(fileData.Take(10000).ToArray());
                            }

                            _steganography.DataFile.Content = fileData;

                            InputFilePath = res.FilePath;
                        }
                    }
                    else
                    {
                        _steganography.DataFile.Content = exportFileData[2];
                    }
                }
                else
                {
                    _steganography.DataFile.Content = CryptoService.DecryptText(exportData, hashPassword);
                }

                return true;
            }
            catch
            {
                bynaryData.Clear();
                return false;
            }
        }

        private async Task<bool> Import()
        {
            try
            {
                saveAction = _steganography.SaveImport;

                var hashPassword = ProgramHelper.GetHash32(Password);

                var inportData = InputData;

                if (inportData.Length == 0)
                {
                    return false;
                }

                if (InputFilePath.Length > 0)
                {
                    inportData = Convert.ToBase64String(System.IO.File.ReadAllBytes(InputFilePath));
                    inportData = CryptoService.EncryptText(inportData, hashPassword);

                    var fileInfo = new FileInfo(InputFilePath);
                    inportData = $"{fileInfo.Name}[*]{fileInfo.Extension}[*]" + inportData;
                }
                else
                {
                    inportData = CryptoService.EncryptText(inportData, hashPassword);
                }

                string binary = Converter.ConvertTextToBinaryString(inportData);

                _steganography.OutputImage.Clear();
                await UpdateList();

                var lines = ProgramHelper.SplitStringIntoParts(binary, _steganography.InputImage.Count);

                for (int i = 0; i < _steganography.InputImage.Count; i++)
                {
                    var bitmapResult = await ImageHelper.ImportDataToImage(lines[i], _steganography.InputImage[i].Path);
                    _steganography.OutputImage.Add(_steganography.InputImage[i], bitmapResult);
                    OnShowImageCommandExecuted(_steganography.InputImage[i]);
                    await UpdateList();
                }

                return true;
            }
            catch (Exception ex)
            {
                _steganography.OutputImage.Clear();
                UpdateList();
                return false;
            }
        }
    }
}
