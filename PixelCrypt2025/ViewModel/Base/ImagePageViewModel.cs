using FontAwesome5;
using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt2025.ViewModel.Base
{
    internal class ImagePageViewModel : ViewModel
    {
        private string _showPasword = "";
        private string _password = "";
        private string _imageName = "";
        private string _imagePath = "";
        private string _imageLength = "";
        private string _imageExtension = "";
        private string _imagePermission = "";

        private Model.Image _selecedImage = null;

        private StackPanel _filePathImageStackPanel;

        private GridLength _closePasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _openPasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveDataWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _addGridHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _dataGridHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _viewImageWidth = new GridLength(0, GridUnitType.Pixel);

        private bool _isOpenPassword = false;

        private ICommand ShowImageCommand { get; }

        public IImagePage ImagePage { get; }

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand PaswordViewCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand DoActionCommand { get; init; }

        public ImagePageViewModel(IImagePage imagePage)
        {
            ImagePage = imagePage;

            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);
            ClearImageCommand = new LambdaCommand(OnClearImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            OnPaswordViewCommandExecuted();

            AddGridHeight = new GridLength(1, GridUnitType.Star);
        }

        public StackPanel FilePathImageStackPanel
        {
            get => _filePathImageStackPanel;
            set => Set(ref _filePathImageStackPanel, value);
        }

        public string ShowPasword
        {
            get => _showPasword;
            set => Set(ref _showPasword, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string ImageName
        {
            get => _imageName;
            set => Set(ref _imageName, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => Set(ref _imagePath, value);
        }

        public string ImageLength
        {
            get => _imageLength;
            set => Set(ref _imageLength, value);
        }

        public string ImageExtension
        {
            get => _imageExtension;
            set => Set(ref _imageExtension, value);
        }

        public string ImagePermission
        {
            get => _imagePermission;
            set => Set(ref _imagePermission, value);
        }

        public GridLength ClosePasswordWidth
        {
            get => _closePasswordWidth;
            set => Set(ref _closePasswordWidth, value);
        }

        public GridLength OpenPasswordWidth
        {
            get => _openPasswordWidth;
            set => Set(ref _openPasswordWidth, value);
        }

        public GridLength SaveDataWidth
        {
            get => _saveDataWidth;
            set => Set(ref _saveDataWidth, value);
        }

        public GridLength AddGridHeight
        {
            get => _addGridHeight;
            set => Set(ref _addGridHeight, value);
        }

        public GridLength DataGridHeight
        {
            get => _dataGridHeight;
            set => Set(ref _dataGridHeight, value);
        }

        public GridLength ViewImageWidth
        {
            get => _viewImageWidth;
            set => Set(ref _viewImageWidth, value);
        }

        private Model.Image SelecedImage
        {
            get => _selecedImage;
            set
            {
                _selecedImage = value;

                if (value != null)
                {
                    ImageName = $"{_selecedImage.Name}";
                    ImagePath = $"{_selecedImage.Path}";
                    ImageLength = $"{_selecedImage.Length} Kb";
                    ImageExtension = $"{_selecedImage.Extension}";
                    ImagePermission = $"{_selecedImage.Permission}";
                }
                else
                {
                    ImagePath = "";
                }
            }
        }

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnAddImageCommandExecuted(object p = null)
        {
            var filterList = new List<string>() { "jpg", "jpeg", "png" };

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|" + string.Join(";", filterList.Select(ext => $"*.{ext}")),
                Multiselect = true,
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                var prefCount = ImagePage.InputImage.Count;

                foreach (var filepath in openFileDialog.FileNames.Where(file => filterList.Contains(file.Split('.')[1])))
                {
                    AddElement(filepath);
                }

                if (ImagePage.InputImage.Count != prefCount)
                {
                    SaveDataWidth = new GridLength(0, GridUnitType.Star);
                    FilePathImageStackPanel = UpdateImageList();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить элементы", openFileDialog.Title);
                }
            }
            if (ImagePage.InputImage.Count > 0)
            {
                AddGridHeight = new GridLength(0, GridUnitType.Star);
                DataGridHeight = new GridLength(1, GridUnitType.Star);
            }
        }

        private void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            RemoveElement(parametr);

            FilePathImageStackPanel = UpdateImageList();

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
            }

            if (ImagePage.InputImage.Count == 0)
            {
                AddGridHeight = new GridLength(1, GridUnitType.Star);
                DataGridHeight = new GridLength(0, GridUnitType.Star);
                SelecedImage = null;
            }
        }

        private void OnClearImageCommandExecuted(object p = null)
        {
            ImagePage.InputImage.Clear();

            AddGridHeight = new GridLength(1, GridUnitType.Star);
            DataGridHeight = new GridLength(0, GridUnitType.Star);
            SelecedImage = null;
            ViewImageWidth = new GridLength(0, GridUnitType.Star);
        }

        private void OnPaswordViewCommandExecuted(object p = null)
        {
            if (_isOpenPassword)
            {
                OpenPasswordWidth = new GridLength(1, GridUnitType.Star);
                ClosePasswordWidth = new GridLength(0, GridUnitType.Star);
                ShowPasword = "Regular_Eye";
            }
            else
            {
                OpenPasswordWidth = new GridLength(0, GridUnitType.Star);
                ClosePasswordWidth = new GridLength(1, GridUnitType.Star);
                ShowPasword = "Regular_EyeSlash";
            }

            _isOpenPassword = !_isOpenPassword;
        }

        private void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
            }
            else if (System.IO.File.Exists(parametr.Path))
            {
                SelecedImage = parametr;
                ViewImageWidth = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                MessageBox.Show("Не удалось найти фаил, возможно он удален или перемещен");
                OnRemoveImageCommandExecuted(parametr);
            }
            FilePathImageStackPanel = UpdateImageList();
        }

        private StackPanel UpdateImageList()
        {
            var stackPanel = new StackPanel();

            foreach (var imagePath in ImagePage.InputImage)
            {
                var grid = new Grid()
                {
                    Margin = new Thickness(10, 5, 5, 5)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                var textBlock = new TextBlock()
                {
                    Text = imagePath.ToString(),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(10, 10, 10, 10),
                };

                var button = new Button()
                {
                    Content = textBlock,
                    Command = ShowImageCommand,
                    Style = (Style)Application.Current.FindResource(imagePath == SelecedImage ? "SelectedListButtonStyle" : "BaseListButtonStyle"),
                    CommandParameter = imagePath,
                    BorderThickness = new Thickness(1)
                };

                Grid.SetColumn(button, 1);

                var deleteButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("ToolButtonStyle"),
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 35,
                    Height = 35,
                    Padding = new Thickness(0),
                    Content = new ImageAwesome
                    {
                        Icon = EFontAwesomeIcon.Regular_TimesCircle,
                        Width = 25,
                        Height = 25,
                        Foreground = (Brush)new BrushConverter().ConvertFromString(Foreground)
                    },
                    Command = RemoveImageCommand,
                    CommandParameter = imagePath,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                };

                Grid.SetColumn(deleteButton, 2);

                grid.Children.Add(button);
                grid.Children.Add(deleteButton);
                stackPanel.Children.Add(grid);
            }

            return stackPanel;
        }

        private void AddElement(string filepath)
        {
            foreach (var image in ImagePage.InputImage)
            {
                if (image.Path == filepath) return;
            }

            ImagePage.InputImage.Add(new Model.Image(filepath));
        }

        private void RemoveElement(Model.Image parametr)
        {
            ImagePage.InputImage.Remove(parametr);
        }
    }
}
