using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    internal class ImageListViewModel : BaseViewModel
    {
        public ObservableCollection<ImageChipViewModel> Images { get; set; }

        private ImageChipViewModel selectedImage;

        public ICommand AddImageCommand { get; set; }

        public ICommand ClearImagesCommand { get; set; }

        public ImageListViewModel()
        {
            Images = new ObservableCollection<ImageChipViewModel>();

            AddImageCommand = new LambdaCommand(AddImage);
            ClearImagesCommand = new LambdaCommand(ClearImages);
        }

        public ImageChipViewModel SelectedImage
        {
            get => selectedImage;
            set => Set(ref selectedImage, value);
        }

        private void AddImage(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выберите изображение";

            openFileDialog.Filter = "Изображение (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

            openFileDialog.Multiselect = true;

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            foreach (string filePath in openFileDialog.FileNames)
            {
                bool alreadyExists = Images.Any(x => x.ImagePath == filePath);

                if (alreadyExists)
                    continue;

                Images.Add(new ImageChipViewModel(filePath, this));
            }
        }

        private void ClearImages(object p)
        {
            Images.Clear();
        }
    }
}
