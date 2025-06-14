using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;
using System.Windows.Media;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class CryptographyPageViewModel : ImagePageViewModel
    {
        private Model.Cryptography _cryptography = new Model.Cryptography();

        private GridLength _viewResultImageWidth = Constants.GridLengthZero;

        private ImageSource _imageResultPath;

        public CryptographyPageViewModel()
        {
            ImagePage = _cryptography;

            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            InputAction = Encrypt;
            OutputAction = Decrypt;

            OnAddImageCommandExecuted();
        }

        public ImageSource ImageResultPath
        {
            get => _imageResultPath;
            set => Set(ref _imageResultPath, value);
        }

        public GridLength ViewResultImageWidth
        {
            get => _viewResultImageWidth;
            set => Set(ref _viewResultImageWidth, value);
        }

        private async void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Func<Task<bool>> doAction) return;
            SaveDataWidth = Constants.GridLengthZero;
            saveAction = _cryptography.SaveData;
            if (SelecedImage != null)
            {
                OnShowImageCommandExecuted(SelecedImage);
            }
            IsSuccessResult = false;
            IsButtonFree = false;
            isProcessing = true;

            if (await doAction())
            {
                IsSuccessResult = true;
            }
            else
            {
                MessageBox.Show("Возникла ошибка");
                IsSuccessResult = false;
            }

            isProcessing = false;
            IsButtonFree = true;
            UpdateList();
        }

        protected override void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            base.OnRemoveImageCommandExecuted(parametr);

            UpdateResultImage(parametr);
        }

        protected override void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            base.OnShowImageCommandExecuted(parametr);

            UpdateResultImage(parametr);
        }

        private void UpdateResultImage(Model.Image parametr)
        {
            if (ViewImageWidth.Value == 0)
            {
                ViewResultImageWidth = Constants.GridLengthZero;
            }
            else
            {
                if (_cryptography.OutputImage.ContainsKey(parametr))
                {
                    ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
                    ImageResultPath = Converter.ConvertBitmapToImageSource(_cryptography.OutputImage[SelecedImage]);
                }
                else
                {
                    ViewResultImageWidth = Constants.GridLengthZero;
                }
            }
        }

        private async Task<bool> Decrypt()
        {
            var hashPassword = ProgramHelper.GetHash32(Password);

            _cryptography.OutputImage.Clear();
            await UpdateList();

            try
            {
                foreach (var file in _cryptography.InputImage)
                {
                    _cryptography.OutputImage.Add(file, await CryptoService.DecryptPhoto(file.Path, hashPassword));
                    OnShowImageCommandExecuted(file);
                    await UpdateList();
                }
                return true;
            }
            catch
            {
                _cryptography.OutputImage.Clear();
                return false;
            }
        }

        private async Task<bool> Encrypt()
        {
            var hashPassword = ProgramHelper.GetHash32(Password);

            _cryptography.OutputImage.Clear();
            await UpdateList();

            try
            {
                foreach (var file in _cryptography.InputImage)
                {
                    _cryptography.OutputImage.Add(file, await CryptoService.EncryptPhoto(file.Path, hashPassword));
                    OnShowImageCommandExecuted(file);
                    await UpdateList();
                }
                return true;
            }
            catch (Exception ex)
            {
                _cryptography.OutputImage.Clear();
                return false;
            }
        }
    }
}
