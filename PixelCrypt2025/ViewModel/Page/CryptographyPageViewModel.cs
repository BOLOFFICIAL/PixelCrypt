using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Enum;
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
            ImagePage.InputImage = Images;

            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            InputAction = _cryptography.Encrypt;
            OutputAction = _cryptography.Decrypt;

            _cryptography.ShowImage = (img) => { SelectedImage = img; };

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

            if (SelectedImage != null) OnShowImageCommandExecuted(SelectedImage);

            IsSuccessResult = false;
            IsButtonFree = false;

            StartTimer();

            ProgressWidth = Constants.GridLengthAuto;
            ActionWidth = Constants.GridLengthZero;

            var result = await doAction(Password);
            var successResult = result.IsSuccessResult;
            var status = successResult ? NotificationStatus.Success : NotificationStatus.Error;

            StopTimer();

            Notification.Show($"{result.ResultMessage}", result.ResultTitle, status: status);

            ProgressWidth = Constants.GridLengthZero;
            ActionWidth = Constants.GridLengthAuto;
            IsSuccessResult = successResult;

            IsButtonFree = true;
        }

        protected override void OnAddImageCommandExecuted(object p = null)
        {
            base.OnAddImageCommandExecuted(p);
            UpdateResultImage(SelectedImage);
        }

        protected override void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Image parametr) return;

            base.OnRemoveImageCommandExecuted(parametr);
            UpdateResultImage(SelectedImage);
        }

        protected override void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Image parametr) return;

            base.OnShowImageCommandExecuted(parametr);
            UpdateResultImage(parametr);
        }

        private async void UpdateResultImage(Image parametr)
        {
            ViewResultImageWidth = Constants.GridLengthZero;
            ImageResultPath = null;

            if (parametr is null || SelectedImage is null) return;

            if (_cryptography.OutputImage.TryGetValue(parametr, out var image) && ViewImageWidth.Value != 0)
            {
                var previousImage = SelectedImage;

                ImageResultPath = await Task.Run(() => Converter.ConvertBitmapToImageSource(image));

                if (SelectedImage is null || !ReferenceEquals(previousImage, SelectedImage)) return;

                ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
            }
        }
    }
}
