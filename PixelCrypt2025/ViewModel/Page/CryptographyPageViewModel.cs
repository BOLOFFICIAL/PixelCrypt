using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class CryptographyPageViewModel : ImagePageViewModel
    {
        private Cryptography _cryptography;

        private GridLength _viewResultImageWidth = new GridLength(0, GridUnitType.Pixel);

        public CryptographyPageViewModel() : base(new Cryptography())
        {
            _cryptography = ImagePage as Cryptography;

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
            SaveDataWidth = new GridLength(1, GridUnitType.Star);
        }

        private void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
                ViewResultImageWidth = new GridLength(0, GridUnitType.Star);
            }
            else if (System.IO.File.Exists(parametr.Path))
            {
                SelecedImage = parametr;
                ViewImageWidth = new GridLength(4, GridUnitType.Star);
                ViewResultImageWidth = new GridLength(5, GridUnitType.Star);
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
