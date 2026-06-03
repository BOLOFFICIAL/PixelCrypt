using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageListViewModel : BaseViewModel
    {
        public ObservableCollection<ImageChipViewModel> Images { get; }

        private ImageChipViewModel? _selectedImage;
        private bool _isEnable = true;

        public GridLength _heightButtons = new GridLength(1, GridUnitType.Auto);

        public ICommand AddImageCommand { get; set; }

        public ICommand ClearImagesCommand { get; set; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand OpenOriginalCommand { get; }

        public ImageListViewModel()
        {
            Images = new ObservableCollection<ImageChipViewModel>();

            AddImageCommand = new LambdaCommand(AddImage);
            ClearImagesCommand = new LambdaCommand(ClearImages, CanClearImages);

            MoveUpCommand = new LambdaCommand(OnMoveUp, CanMoveUp);
            MoveDownCommand = new LambdaCommand(OnMoveDown, CanMoveDown);
            RemoveCommand = new LambdaCommand(OnRemove, CanRemove);
            OpenOriginalCommand = new LambdaCommand(OnOpenOriginal, CanOpenOriginal);
        }

        public GridLength HeightButtons
        {
            get => _heightButtons;
            set => Set(ref _heightButtons, value);
        }

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                Set(ref _isEnable, value);

                if (_isEnable)
                {
                    HeightButtons = new GridLength(1, GridUnitType.Auto);
                }
                else
                {
                    HeightButtons = new GridLength(0, GridUnitType.Star);
                }
            }
        }

        public ImageChipViewModel? SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (_selectedImage != null)
                    _selectedImage.IsSelected = false;

                Set(ref _selectedImage, value);

                if (_selectedImage != null)
                    _selectedImage.IsSelected = true;
            }
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
                bool alreadyExists = Images.Any(x => x.ImageFile.ImagePath == filePath);

                if (alreadyExists)
                    continue;

                Images.Add(new ImageChipViewModel(filePath));
            }

            SelectedImage = SelectedImage ?? Images.FirstOrDefault();
        }

        private void ClearImages(object p)
        {
            Images.Clear();
            SelectedImage = null;
        }

        private bool CanClearImages(object p)
            => Images.Count > 0;

        private void OnMoveUp(object p)
        {
            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index <= 0)
                return;

            Images.Move(index, index - 1);
        }

        private bool CanMoveUp(object p)
        {
            if (p is not ImageChipViewModel image || !IsEnable)
                return false;

            return Images.IndexOf(image) > 0;
        }

        private void OnMoveDown(object p)
        {
            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index < 0 || index >= Images.Count - 1)
                return;

            Images.Move(index, index + 1);
        }

        private bool CanMoveDown(object p)
        {
            if (p is not ImageChipViewModel image || !IsEnable)
                return false;

            return Images.IndexOf(image) < Images.Count - 1;
        }

        private void OnRemove(object p)
        {
            if (p is not ImageChipViewModel image) return;

            Images.Remove(image);
        }

        private bool CanRemove(object p)
            => IsEnable;

        private void OnOpenOriginal(object p)
        {
            if (p is not ImageChipViewModel image) return;

            Process.Start(new ProcessStartInfo()
            {
                FileName = image.ImageFile.ImagePath,
                UseShellExecute = true
            });
        }

        private bool CanOpenOriginal(object p)
            => IsEnable;
    }
}
