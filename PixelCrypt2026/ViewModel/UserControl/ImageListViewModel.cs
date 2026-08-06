using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageListViewModel : BaseViewModel
    {
        private ImageChipViewModel? _selectedImage;
        private bool _isEnable = true;
        private GridLength _heightButtons;
        private GridLength _widthAdd;
        private GridLength _widthClear;

        private readonly List<string> _validExtension = new List<string>() { ".png", ".jpg", ".jpeg", ".bmp" };

        public event Func<bool> CanAdd;
        public event Func<bool> CanClear;

        public event Func<bool> CanMoveUp;
        public event Func<bool> CanMoveDown;
        public event Func<bool> CanRemove;
        public event Func<bool> CanOpenOriginal;

        public event Action SelectImage;

        public event Action AddRequested;
        public event Action ClearRequested;

        public event Action MoveUpRequested;
        public event Action MoveDownRequested;
        public event Action RemoveRequested;
        public event Action OpenOriginalRequested;

        public event Func<bool> ConfirmationAddRequested;
        public event Func<bool> ConfirmationClearRequested;

        public event Func<bool> ConfirmationMoveUpRequested;
        public event Func<bool> ConfirmationMoveDownRequested;
        public event Func<bool> ConfirmationRemoveRequested;
        public event Func<bool> ConfirmationOpenOriginalRequested;

        public ObservableCollection<ImageChipViewModel> Images { get; }
        public long TotalSize = 0;

        public ICommand AddImageCommand { get; }
        public ICommand ClearImagesCommand { get; }
        public ICommand PasteImagesCommand { get; }
        public ICommand DropCommand { get;}

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand OpenOriginalCommand { get; }

        public ImageListViewModel()
        {
            Images = new ObservableCollection<ImageChipViewModel>();

            AddImageCommand = new LambdaCommand(AddImage);
            ClearImagesCommand = new LambdaCommand(ClearImages, CanClearImages);
            PasteImagesCommand = new LambdaCommand(PasteImages);
            DropCommand = new LambdaCommand(DropImages);

            MoveUpCommand = new LambdaCommand(OnMoveUp, OnCanMoveUp);
            MoveDownCommand = new LambdaCommand(OnMoveDown, OnCanMoveDown);
            RemoveCommand = new LambdaCommand(OnRemove, OnCanRemove);
            OpenOriginalCommand = new LambdaCommand(OnOpenOriginal, OnCanOpenOriginal);
        }

        private void DropImages(object obj)
        {
            var e = obj as DragEventArgs;

            if (e is null || !e.Data.GetDataPresent(DataFormats.FileDrop) || !IsEnable) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            AddImageFiles(files);
        }

        public GridLength HeightButtons
        {
            get => _heightButtons;
            set => Set(ref _heightButtons, value);
        }

        public GridLength WidthAdd
        {
            get => _widthAdd;
            set => Set(ref _widthAdd, value);
        }

        public GridLength WidthClear
        {
            get => _widthClear;
            set => Set(ref _widthClear, value);
        }

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                Set(ref _isEnable, value);

                HeightButtons = new GridLength(_isEnable ? 1 : 0, _isEnable ? GridUnitType.Auto : GridUnitType.Star);
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
                {
                    _selectedImage.IsSelected = true;
                    SelectImage?.Invoke();
                }
            }
        }

        private void AddImage(object p)
        {
            if ((!ConfirmationAddRequested?.Invoke()) ?? false)
                return;

            var filter = string.Join(";", _validExtension.Select(extension => $"*{extension}"));

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select an image",
                Filter = $"Image ({filter})|{filter}",
                Multiselect = true,
            };

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            AddImageFiles(openFileDialog.FileNames);
        }

        private void AddImageFiles(string[] fileNames)
        {
            var errorList = new List<string>();

            foreach (string filePath in fileNames)
            {
                if (Path.GetExtension(filePath) == "")
                {
                    AddImageFiles(Directory.GetFiles(filePath)
                        .Where(f => _validExtension.Contains(Path.GetExtension(f)))
                        .ToArray());
                }

                bool alreadyExists = Images.Any(x => x.ImageFile.FilePath == filePath);

                if (alreadyExists || !File.Exists(filePath) || !_validExtension.Contains(Path.GetExtension(filePath)))
                    continue;

                var newItem = new ImageChipViewModel(filePath);

                var volume = newItem.ImageFile.ImageWidth * newItem.ImageFile.ImageHeight;

                if (volume > 0 && filePath.Length < 236)
                {
                    TotalSize += volume;
                    Images.Add(newItem);
                }
                else
                {
                    errorList.Add(newItem.ImageFile.FileName);
                }
            }

            if (errorList.Any())
            {
                Notification.Show($"The following images are empty or incorrect and could not be added:\n• {string.Join("\n• ", errorList)}",
                    "Adding Images",
                    Program.Enum.NotificationButtonType.Ok,
                    Program.Enum.NotificationIconType.Question);
            }

            SelectedImage = SelectedImage ?? Images.FirstOrDefault();

            AddRequested?.Invoke();
        }

        private void ClearImages(object p)
        {
            if ((!ConfirmationClearRequested?.Invoke()) ?? false)
                return;

            Images.Clear();
            SelectedImage = null;
            TotalSize = 0;

            ClearRequested?.Invoke();
        }

        private bool CanClearImages(object p)
        {
            var res = (CanClear?.Invoke() ?? true)
                && Images.Count > 0;

            WidthClear = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }

        private void PasteImages(object p)
        {
            if ((!ConfirmationAddRequested?.Invoke()) ?? false)
                return;

            var fileDropList = Clipboard.GetFileDropList();

            if (fileDropList != null && fileDropList.Count > 0)
            {
                AddImageFiles(fileDropList.Cast<string>().ToArray());
                return;
            }

            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                using var image = System.Windows.Forms.Clipboard.GetImage();

                if (image == null)
                    return;

                var tempDir = Path.Combine(
                    Path.GetTempPath(),
                    "PixelCrypt");

                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                var hash = ProgramHelper.GetHash32(ProgramHelper.GetSha256(image));

                var tempPath = Path.Combine(tempDir, $"pasted_{hash}.png");

                if (!File.Exists(tempPath))
                    image.Save(tempPath, ImageFormat.Png);

                AddImageFiles(new[] { tempPath });
                return;
            }

            AddImage(null);
        }

        private void OnMoveUp(object p)
        {
            if ((!ConfirmationMoveUpRequested?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index <= 0)
                return;

            Images.Move(index, index - 1);

            MoveUpRequested?.Invoke();
        }

        private bool OnCanMoveUp(object p)
        {
            if (p is not ImageChipViewModel image || !IsEnable)
                return false;

            return (CanMoveUp?.Invoke() ?? true)
                && Images.IndexOf(image) > 0;
        }

        private void OnMoveDown(object p)
        {
            if ((!ConfirmationMoveDownRequested?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index < 0 || index >= Images.Count - 1)
                return;

            Images.Move(index, index + 1);

            MoveDownRequested?.Invoke();
        }

        private bool OnCanMoveDown(object p)
        {
            if (p is not ImageChipViewModel image || !IsEnable)
                return false;

            return (CanMoveDown?.Invoke() ?? true)
                && Images.IndexOf(image) < Images.Count - 1;
        }

        private void OnRemove(object p)
        {
            if ((!ConfirmationRemoveRequested?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            var index = Images.IndexOf(image);

            SelectedImage = index > 0
                ? Images[index - 1]
                : index < Images.Count - 1
                    ? SelectedImage = Images[index + 1]
                    : null;

            TotalSize -= image.ImageFile.ImageWidth * image.ImageFile.ImageHeight;

            Images.Remove(image);

            RemoveRequested?.Invoke();
        }

        private bool OnCanRemove(object p)
            => (CanRemove?.Invoke() ?? true)
                && IsEnable;

        private void OnOpenOriginal(object p)
        {
            if ((!ConfirmationOpenOriginalRequested?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            Process.Start(new ProcessStartInfo()
            {
                FileName = image.ImageFile.FilePath,
                UseShellExecute = true
            });

            OpenOriginalRequested?.Invoke();
        }

        private bool OnCanOpenOriginal(object p)
            => (CanOpenOriginal?.Invoke() ?? true)
                && IsEnable;

        public void ResetImages()
        {
            foreach (var image in Images)
            {
                image.Status = Program.Enum.StatusType.None;
                image.ImageFile.ResultImage = null;
                image.ImageFile.ResultImageSource = null;
            }
        }
    }
}
