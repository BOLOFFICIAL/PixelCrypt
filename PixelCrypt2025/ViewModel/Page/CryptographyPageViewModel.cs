using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;
using System.Windows.Media;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class CryptographyPageViewModel : ImagePageViewModel
    {
        private Cryptography _cryptography = new Cryptography();

        private GridLength _viewResultImageWidth = Constants.GridLengthZero;

        private ImageSource _imageResultPath;

        public CryptographyPageViewModel()
        {
            ImagePage = _cryptography;

            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            InputAction = _cryptography.Encrypt;
            OutputAction = _cryptography.Decrypt;

            _cryptography.UpdateList = UpdateList;
            _cryptography.ShowImage = OnShowImageCommandExecuted;

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
            if (p is not Func<string, Task<ActionResult>> doAction) return;
            SaveDataWidth = Constants.GridLengthZero;

            if (SelecedImage != null) OnShowImageCommandExecuted(SelecedImage);

            IsSuccessResult = false;
            IsButtonFree = false;

            var result = await doAction(Password);

            IsSuccessResult = result.IsSuccessResult;

            Notification.Show($"{result.ResultMessage}", result.ResultTitle);

            IsButtonFree = true;
            UpdateList();
        }

        protected override void OnAddImageCommandExecuted(object p = null)
        {
            base.OnAddImageCommandExecuted(p);
            UpdateResultImage(SelecedImage);
        }

        protected override void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Image parametr) return;

            base.OnRemoveImageCommandExecuted(parametr);
            UpdateResultImage(parametr);
        }

        protected override void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Image parametr) return;

            base.OnShowImageCommandExecuted(parametr);
            UpdateResultImage(parametr);
        }

        private async void UpdateResultImage(Image parametr)
        {
            if (ViewImageWidth.Value == 0)
            {
                ViewResultImageWidth = Constants.GridLengthZero;
            }
            else if (_cryptography.OutputImage.ContainsKey(parametr))
            {
                ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
                ImageResultPath = await Converter.ConvertBitmapToImageSource(_cryptography.OutputImage[SelecedImage]);
            }
            else
            {
                ViewResultImageWidth = Constants.GridLengthZero;
            }
        }
    }
}
