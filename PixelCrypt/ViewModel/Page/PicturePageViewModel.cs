using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using System.Windows;

namespace PixelCrypt.ViewModel.Page
{
    internal class PicturePageViewModel : Base.ViewModel
    {
        private GridLength _resultImageWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);

        public PicturePageViewModel()
        {
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
        }

        public GridLength ResultImageWidth
        {
            get => _resultImageWidth;
            set => Set(ref _resultImageWidth, value);
        }

        public GridLength ActionWidth
        {
            get => _actionWidth;
            set => Set(ref _actionWidth, value);
        }

        public GridLength SaveButtonWidth
        {
            get => _saveButtonWidth;
            set => Set(ref _saveButtonWidth, value);
        }

        public void OnChoseImageCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                ImageData = openFileDialog.FileNames[0];

                FilePathImage = ImageData;

                ActionWidth = new GridLength(1, GridUnitType.Auto);
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                ResultImageWidth = SaveButtonWidth;
            }
        }

        public void OnDoActionCommandExecuted(object p = null)
        {
            try
            {
                if (p is not string action) return;

                _isSuccessAction = false;
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                ResultImageWidth = SaveButtonWidth;

                switch (action)
                {
                    case "Encrypt": Encrypt(); break;
                    case "Decrypt": Decrypt(); break;
                }

                if (_isSuccessAction)
                {
                    SaveButtonWidth = new GridLength(1, GridUnitType.Auto);
                    ResultImageWidth = new GridLength(1, GridUnitType.Star);
                }
            }
            catch
            {
                MessageBox.Show("Не удалось выполнить действие");
            }
        }

        private void Decrypt()
        {
            _isSuccessAction = true;
        }

        private void Encrypt()
        {
            _isSuccessAction = false;
        }
    }
}
