using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class CryptographyPageViewModel : ImagePageViewModel
    {
        private Cryptography _cryptography = new Cryptography();

        private GridLength _viewResultImageWidth = Constants.GridLengthZero;

        public CryptographyPageViewModel()
        {
            ImagePage = _cryptography;

            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            InputAction = _cryptography.EncryptAction;
            OutputAction = _cryptography.DecryptAction;
        }

        public GridLength ViewResultImageWidth
        {
            get => _viewResultImageWidth;
            set => Set(ref _viewResultImageWidth, value);
        }

        private void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Action action) return;
            action();
            SaveDataWidth = Constants.GridLengthStar;
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
                ViewResultImageWidth = ViewImageWidth;
            }
        }
    }
}
