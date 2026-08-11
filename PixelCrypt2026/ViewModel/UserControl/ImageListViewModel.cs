using Microsoft.Win32;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.Program;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageListViewModel : BaseViewModel
    {
        private ImageChipViewModel? _selectedImage;
        private bool _isEnable = true;
        private GridLength _heightButtons;
        private GridLength _widthAdd;
        private GridLength _widthClear;

        private readonly List<string> _validExtensions = new List<string>() { ".png", ".jpg", ".jpeg", ".bmp" };

        public ControlAction Add { get; set; }
        public ControlAction Clear { get; set; }

        public ControlAction MoveUp { get; set; }
        public ControlAction MoveDown { get; set; }
        public ControlAction Remove { get; set; }
        public ControlAction OpenOriginal { get; set; }

        public ControlAction SelectImage { get; set; }

        public ObservableCollection<ImageChipViewModel> Images { get; }
        public long TotalSize = 0;
        private bool _isImporting;

        public ICommand AddImageCommand { get; }
        public ICommand ClearImagesCommand { get; }
        public ICommand PasteImagesCommand { get; }
        public ICommand DropCommand { get; }

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand OpenOriginalCommand { get; }

        public ImageListViewModel()
        {
            Images = new ObservableCollection<ImageChipViewModel>();

            AddImageCommand = new LambdaCommand(AddImage, _ => IsInteractive);
            ClearImagesCommand = new LambdaCommand(ClearImages, CanClearImages);
            PasteImagesCommand = new LambdaCommand(PasteImages, _ => IsInteractive);
            DropCommand = new LambdaCommand(DropImages, _ => IsInteractive);

            MoveUpCommand = new LambdaCommand(OnMoveUp, OnCanMoveUp);
            MoveDownCommand = new LambdaCommand(OnMoveDown, OnCanMoveDown);
            RemoveCommand = new LambdaCommand(OnRemove, OnCanRemove);
            OpenOriginalCommand = new LambdaCommand(OnOpenOriginal, OnCanOpenOriginal);
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
                if (Set(ref _isEnable, value))
                {
                    HeightButtons = new GridLength(_isEnable ? 1 : 0, _isEnable ? GridUnitType.Auto : GridUnitType.Star);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsImporting
        {
            get => _isImporting;
            set
            {
                if (Set(ref _isImporting, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool IsInteractive => IsEnable && !IsImporting;

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
                    SelectImage?.ExecuteRequested?.Invoke();
                }
            }
        }

        public void ResetImages()
        {
            foreach (var image in Images)
            {
                try
                {
                    var imagePath = image.ImageFile.ResultImage;
                    image.ImageFile.ResultImage = null;

                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                }
                catch { }
                finally
                {
                    image.Status = Program.Enum.StatusType.None;
                }
            }
        }


        private async void DropImages(object obj)
        {
            var e = obj as DragEventArgs;

            if (e is null || !e.Data.GetDataPresent(DataFormats.FileDrop) || !IsInteractive) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            await ImportImageFilesAsync(files);

            Application.Current?.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested, DispatcherPriority.Background);
        }

        private async void PasteImages(object p)
        {
            if ((!Add?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            var fileDropList = Clipboard.GetFileDropList();

            if (fileDropList != null && fileDropList.Count > 0)
            {
                await ImportImageFilesAsync(fileDropList.Cast<string>().ToArray());
                return;
            }

            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                using var image = System.Windows.Forms.Clipboard.GetImage();

                if (image == null)
                    return;

                var tempDir = Path.Combine(Path.GetTempPath(), "PixelCrypt");

                Directory.CreateDirectory(tempDir);

                var hash = ProgramHelper.GetHash32(ProgramHelper.GetSha256(image));

                var tempPath = Path.Combine(tempDir, $"pasted_{hash}.png");

                if (!File.Exists(tempPath))
                    image.Save(tempPath, ImageFormat.Png);

                await ImportImageFilesAsync(new[] { tempPath });
                return;
            }

            AddImage(null);
        }

        private async void AddImage(object p)
        {
            if (!IsInteractive || ((!Add?.ConfirmationRequired?.Invoke()) ?? false))
                return;

            var filter = string.Join(";", _validExtensions.Select(extension => $"*{extension}"));

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select an image",
                Filter = $"Image ({filter})|{filter}",
                Multiselect = true,
            };

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            await ImportImageFilesAsync(openFileDialog.FileNames);
        }

        private void ClearImages(object p)
        {
            if (!IsInteractive || ((!Clear?.ConfirmationRequired?.Invoke()) ?? false))
                return;

            foreach (var image in Images)
            {
                try
                {
                    if (File.Exists(image.ImageFile.ResultImage))
                        File.Delete(image.ImageFile.ResultImage);
                }
                catch
                {
                    continue;
                }
            }

            Images.Clear();
            SelectedImage = null;
            TotalSize = 0;

            Clear?.ExecuteRequested?.Invoke();
        }

        private bool CanClearImages(object p)
        {
            var res = IsInteractive && (Clear?.CanExecute?.Invoke() ?? true)
                && Images.Count > 0;

            WidthClear = new GridLength(res ? 1 : 0, GridUnitType.Star);

            return res;
        }


        private void OnMoveUp(object p)
        {
            if ((!MoveUp?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index <= 0)
                return;

            Images.Move(index, index - 1);

            MoveUp?.ExecuteRequested?.Invoke();
        }

        private bool OnCanMoveUp(object p)
        {
            if (p is not ImageChipViewModel image || !IsInteractive)
                return false;

            return (MoveUp?.CanExecute?.Invoke() ?? true) && Images.IndexOf(image) > 0;
        }

        private void OnMoveDown(object p)
        {
            if ((!MoveDown?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            int index = Images.IndexOf(image);

            if (index < 0 || index >= Images.Count - 1)
                return;

            Images.Move(index, index + 1);

            MoveDown?.ExecuteRequested?.Invoke();
        }

        private bool OnCanMoveDown(object p)
        {
            if (p is not ImageChipViewModel image || !IsInteractive)
                return false;

            return (MoveDown?.CanExecute?.Invoke() ?? true) && Images.IndexOf(image) < Images.Count - 1;
        }

        private void OnRemove(object p)
        {
            if ((!Remove?.ConfirmationRequired?.Invoke()) ?? false)
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

            try
            {
                var imagePath = image.ImageFile.ResultImage;

                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            catch { }

            Remove?.ExecuteRequested?.Invoke();
        }

        private bool OnCanRemove(object p)
        {
            if (p is not ImageChipViewModel image || !IsInteractive)
                return false;

            return Remove?.CanExecute?.Invoke() ?? true;
        }

        private void OnOpenOriginal(object p)
        {
            if ((!OpenOriginal?.ConfirmationRequired?.Invoke()) ?? false)
                return;

            if (p is not ImageChipViewModel image) return;

            Process.Start(new ProcessStartInfo()
            {
                FileName = image.ImageFile.FilePath,
                UseShellExecute = true
            });

            OpenOriginal?.ExecuteRequested?.Invoke();
        }

        private bool OnCanOpenOriginal(object p)
        {
            if (p is not ImageChipViewModel image || !IsInteractive)
                return false;

            return OpenOriginal?.CanExecute?.Invoke() ?? true;
        }


        private async Task ImportImageFilesAsync(string[] fileNames)
        {
            IsImporting = true;

            try
            {
                var invalidFiles = new List<string>();

                if (fileNames == null || fileNames.Length == 0)
                    return;

                var (validFiles, selectedFolders, selectedFileSet) = await Task.Run(() =>
                {
                    var files = GetNewImagePaths(fileNames);
                    ClassifyPathsByType(fileNames, out var folders, out var fileSet);
                    return (files, folders, fileSet);
                });

                var validByFolder = validFiles
                    .GroupBy(f => Path.GetDirectoryName(f) ?? "", StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                foreach (var folder in selectedFolders)
                {
                    if (validByFolder.TryGetValue(folder, out var filesInFolder))
                        selectedFileSet.UnionWith(filesInFolder);
                }

                var selectedFiles = selectedFileSet.ToList();

                if (selectedFiles.Count < validFiles.Count)
                {
                    string message = $"The selected folder contains files in subfolders.\nWhich files should be processed?";

                    var result = Notification.Show(message,
                        title: "Adding images",
                        actions: new List<(string, Action)>()
                        {
                            ($"All ({validFiles.Count})", () =>{}),
                            ($"Top-level ({selectedFiles.Count})", () => {validFiles = selectedFiles;})
                        },
                        icon: Program.Enum.NotificationIconType.Question);

                    if (result.Result == Program.Enum.NotificationResultType.Cancel)
                        return;
                }

                var prefCount = Images.Count();

                foreach (var filePath in validFiles.Where(i => !Images.Select(img => img.ImageFile.FilePath).Contains(i)))
                {
                    var imageVm = new ImageChipViewModel(filePath);

                    long width = imageVm.ImageFile.ImageWidth;
                    long height = imageVm.ImageFile.ImageHeight;
                    long volume = width * height;

                    bool isValid = volume > 0 && filePath.Length < 236;

                    if (isValid)
                    {
                        TotalSize += volume;
                        Images.Add(imageVm);

                        await Dispatcher.Yield(DispatcherPriority.Background);
                    }
                    else
                    {
                        invalidFiles.Add(imageVm.ImageFile.FileName);
                    }
                }

                if (invalidFiles.Any())
                {
                    string message = "The following images could not be added (empty or unsupported):\n• " + string.Join("\n• ", invalidFiles);

                    Notification.Show(message, "Adding images", Program.Enum.NotificationButtonType.Ok, Program.Enum.NotificationIconType.Error);
                }
                else if (validFiles.Count == 0)
                {
                    Notification.Show($"No valid images found to add", "Adding images", icon: Program.Enum.NotificationIconType.Error);
                }
                else if (Images.Count == prefCount && validFiles.Any() && invalidFiles.Count < validFiles.Count)
                {
                    Notification.Show($"All selected images are already in the list", "Adding Images", icon: Program.Enum.NotificationIconType.Question);
                }

                SelectedImage ??= Images.FirstOrDefault();
                Add?.ExecuteRequested?.Invoke();
            }
            finally
            {
                IsImporting = false;
            }
        }

        private void ClassifyPathsByType(string[] fileNames, out HashSet<string> selectedFolders, out HashSet<string> selectedFileSet)
        {
            selectedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            selectedFileSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var path in fileNames)
            {
                try
                {
                    var attrs = File.GetAttributes(path);

                    if (attrs.HasFlag(FileAttributes.Directory))
                        selectedFolders.Add(path);
                    else if (_validExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()))
                        selectedFileSet.Add(path);
                }
                catch
                {
                    continue;
                }
            }
        }

        private List<string> GetNewImagePaths(string[] paths)
        {
            var result = new List<string>();

            var existingPaths = new HashSet<string>();

            foreach (string path in paths)
            {
                try
                {
                    var attr = File.GetAttributes(path);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        var files = Directory.GetFiles(path)
                            .Where(f => _validExtensions.Contains(Path.GetExtension(f)?.ToLowerInvariant() ?? ""))
                            .ToArray();

                        result.AddRange(GetNewImagePaths(files));

                        result.AddRange(GetNewImagePaths(Directory.GetDirectories(path)));

                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                if (existingPaths.Contains(path))
                    continue;

                if (!File.Exists(path) || !_validExtensions.Contains((Path.GetExtension(path)?.ToLowerInvariant() ?? "")))
                    continue;

                result.Add(path);
            }

            return result;
        }
    }
}
