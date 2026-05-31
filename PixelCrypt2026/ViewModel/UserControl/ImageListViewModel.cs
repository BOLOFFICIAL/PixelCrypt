using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageListViewModel : BaseViewModel
    {
        public ObservableCollection<ImageFile> Images { get; }

        private ImageFile? selectedImage;

        public ICommand AddImageCommand { get; set; }

        public ICommand ClearImagesCommand { get; set; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand OpenOriginalCommand { get; }

        public ImageListViewModel()
        {
            Images = new ObservableCollection<ImageFile>();

            AddImageCommand = new LambdaCommand(AddImage);
            ClearImagesCommand = new LambdaCommand(ClearImages);

            MoveUpCommand = new LambdaCommand(OnMoveUp, CanMoveUp);
            MoveDownCommand = new LambdaCommand(OnMoveDown, CanMoveDown);
            RemoveCommand = new LambdaCommand(OnRemove);
            OpenOriginalCommand = new LambdaCommand(OnOpenOriginal);
        }

        public ImageFile? SelectedImage
        {
            get => selectedImage;
            set => Set(ref selectedImage, value);
        }

        private void AddImage(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Выберите изображение",
                Filter = "Изображение (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
                Multiselect = true,
            };

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            foreach (string filePath in openFileDialog.FileNames)
            {
                bool alreadyExists = Images.Any(x => x.ImagePath == filePath);

                if (alreadyExists)
                    continue;

                Images.Add(new ImageFile(filePath));
            }

            SelectedImage = SelectedImage ?? Images.FirstOrDefault();
        }

        private void ClearImages(object p)
        {
            Images.Clear();
            SelectedImage = null;
        }

        private void OnMoveUp(object p)
        {
            if (p is not ImageFile image) return;

            int index = Images.IndexOf(image);

            if (index <= 0)
                return;

            Images.Move(index, index - 1);
        }

        private bool CanMoveUp(object p)
        {
            if (p is not ImageFile image)
                return false;

            return Images.IndexOf(image) > 0;
        }

        private void OnMoveDown(object p)
        {
            if (p is not ImageFile image) return;

            int index = Images.IndexOf(image);

            if (index < 0 || index >= Images.Count - 1)
                return;

            Images.Move(index, index + 1);
        }

        private bool CanMoveDown(object p)
        {
            if (p is not ImageFile image)
                return false;

            return Images.IndexOf(image) < Images.Count - 1;
        }

        private void OnRemove(object p)
        {
            if (p is not ImageFile image) return;

            Images.Remove(image);
        }

        private void OnOpenOriginal(object p)
        {
            if (p is not ImageFile image) return;

            Process.Start(new ProcessStartInfo()
            {
                FileName = image.ImagePath,
                UseShellExecute = true
            });
        }
    }
}
