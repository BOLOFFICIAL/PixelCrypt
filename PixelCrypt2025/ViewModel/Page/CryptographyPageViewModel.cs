using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Model;
using PixelCrypt2025.ViewModel.Base;
using System.Windows;

namespace PixelCrypt2025.ViewModel.Page
{
    internal class CryptographyPageViewModel : ImagePageViewModel
    {
        private Cryptography _cryptography;

        private Action _encrypt;
        private Action _decrypt;

        public CryptographyPageViewModel() : base(new Cryptography())
        {
            _cryptography = ImagePage as Cryptography;

            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);

            Encrypt = _cryptography.EncryptAction;
            Decrypt = _cryptography.DecryptAction;
        }

        public Action Encrypt
        {
            get => _encrypt;
            set => Set(ref _encrypt, value);
        }

        public Action Decrypt
        {
            get => _decrypt;
            set => Set(ref _decrypt, value);
        }

        private void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Action action) return;
            action();
            SaveDataWidth = new GridLength(1, GridUnitType.Star);
        }
    }
}
