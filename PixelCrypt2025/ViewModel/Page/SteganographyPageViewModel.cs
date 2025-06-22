using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Enum;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
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

            InputAction = _steganography.Import;
            OutputAction = _steganography.Export;

            _steganography.UpdateList = UpdateList;
            _steganography.ShowImage = OnShowImageCommandExecuted;

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
                if (InputData == null || InputData?.Length == 0 || InputData?.Length > 0 && Notification.Show("Заменить текст на данные из файла?", "Файл для чтения данных", NotificationType.YesNo, status: NotificationStatus.Question).Result == NotificationResultType.Yes)
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
            if (p is not Func<string, Task<ActionResult>> doAction) return;
            SaveDataWidth = Constants.GridLengthZero;

            if (SelecedImage != null) OnShowImageCommandExecuted(SelecedImage);

            IsSuccessResult = false;
            IsReadOnlyInputData = true;
            IsButtonFree = false;

            var result = await doAction(Password);

            IsSuccessResult = result.IsSuccessResult;

            var status = NotificationStatus.Error;

            if (IsSuccessResult)
            {
                status = NotificationStatus.Success;
            }

            IsReadOnlyInputData = InputFilePath?.Length > 0;

            Notification.Show($"{result.ResultMessage}", result.ResultTitle, status: status);

            OnPropertyChanged("InputData");
            OnPropertyChanged("InputFilePath");
            OnPropertyChanged("InputFileName");
            IsButtonFree = true;
            UpdateList();
        }

        protected override void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (ImagePage.OutputImage.ContainsKey(parametr) && AccessReset("Удаление элемента приведет к потере рузльтата")) return;

            if (IsSuccessResult && ImagePage.OutputImage.ContainsKey(parametr))
            {
                IsSuccessResult = false;
            }

            base.OnRemoveImageCommandExecuted(parametr);
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
    }
}
