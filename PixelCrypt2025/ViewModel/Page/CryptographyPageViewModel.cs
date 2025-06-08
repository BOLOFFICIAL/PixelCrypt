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

            InputAction = _cryptography.EncryptAction;
            OutputAction = _cryptography.DecryptAction;

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
            if (p is not Func<string, Task> action) return;
            SaveDataWidth = Constants.GridLengthZero;
            ViewResultImageWidth = Constants.GridLengthZero;
            await action(Password);
            SaveDataWidth = Constants.GridLengthStar;

            if (SelecedImage != null)
            {
                ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
                ImageResultPath = Converter.ConvertBitmapToImageSource(_cryptography.OutputImage[SelecedImage]);
            }

        }

        protected override void OnRemoveImageCommandExecuted(object p = null)
        {
            base.OnRemoveImageCommandExecuted(p);

            ViewResultImageWidth = ViewImageWidth;
        }

        protected override void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            base.OnShowImageCommandExecuted(parametr);

            if (ViewImageWidth.Value == 0)
            {
                ViewResultImageWidth = ViewImageWidth;
            }
            else if (_cryptography.OutputImage.ContainsKey(parametr))
            {
                ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
                ImageResultPath = Converter.ConvertBitmapToImageSource(_cryptography.OutputImage[SelecedImage]);
            }
        }
    }
}
