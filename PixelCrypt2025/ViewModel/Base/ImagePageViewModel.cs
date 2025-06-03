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

        private GridLength _closePasswordWidth = Constants.GridLengthZero;
        private GridLength _openPasswordWidth = Constants.GridLengthZero;
        private GridLength _saveDataWidth = Constants.GridLengthZero;
        private GridLength _addGridHeight = Constants.GridLengthZero;
        private GridLength _dataGridHeight = Constants.GridLengthZero;
        private GridLength _viewImageWidth = Constants.GridLengthZero;

        private bool _isOpenPassword = false;

        protected ICommand ShowImageCommand { get; init; }

        private Action _inputAction;
        private Action _outputAction;

        public IImagePage ImagePage { get; init; }

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        protected ICommand RemoveImageCommand { get; init; }
        public ICommand PaswordViewCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand DoActionCommand { get; init; }

        public ImagePageViewModel()
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);
            ClearImageCommand = new LambdaCommand(OnClearImageCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);

            OnPaswordViewCommandExecuted();

            AddGridHeight = Constants.GridLengthStar;
        }

        public Action InputAction
        {
            get => _inputAction;
            set => Set(ref _inputAction, value);
        }

        public Action OutputAction
        {
            get => _outputAction;
            set => Set(ref _outputAction, value);
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

        protected Model.Image SelecedImage
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
                var imageList = openFileDialog.FileNames.Where(file => filterList.Contains(file.Split('.')[1].ToLower()));

                foreach (var filepath in imageList)
                {
                    AddElement(filepath);
                }

                if (ImagePage.InputImage.Count != prefCount)
                {
                    SaveDataWidth = Constants.GridLengthZero;
                    FilePathImageStackPanel = UpdateImageList();
                }
                else if(imageList.Count() == 0)
                {
                    MessageBox.Show("Не удалось найти подходящие элементы", openFileDialog.Title);
                }
            }
            if (ImagePage.InputImage.Count > 0)
            {
                AddGridHeight = Constants.GridLengthZero;
                DataGridHeight = Constants.GridLengthStar;
            }
        }

        private void OnClearImageCommandExecuted(object p = null)
        {
            ImagePage.InputImage.Clear();

            AddGridHeight = Constants.GridLengthStar;
            DataGridHeight = Constants.GridLengthZero;
            SelecedImage = null;
            ViewImageWidth = Constants.GridLengthZero;
        }

        private void OnPaswordViewCommandExecuted(object p = null)
        {
            if (_isOpenPassword)
            {
                OpenPasswordWidth = Constants.GridLengthStar;
                ClosePasswordWidth = Constants.GridLengthZero;
                ShowPasword = "Regular_Eye";
            }
            else
            {
                OpenPasswordWidth = Constants.GridLengthZero;
                ClosePasswordWidth = Constants.GridLengthStar;
                ShowPasword = "Regular_EyeSlash";
            }

            _isOpenPassword = !_isOpenPassword;
        }

        protected virtual void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            RemoveElement(parametr);

            FilePathImageStackPanel = UpdateImageList();

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = Constants.GridLengthZero;
            }

            if (ImagePage.InputImage.Count == 0)
            {
                AddGridHeight = Constants.GridLengthStar;
                DataGridHeight = Constants.GridLengthZero;
                SelecedImage = null;
            }
        }

        protected virtual void OnShowImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = Constants.GridLengthZero;
            }
            else if (System.IO.File.Exists(parametr.Path))
            {
                SelecedImage = parametr;
                ViewImageWidth = new GridLength(4, GridUnitType.Star);
            }
            else
            {
                MessageBox.Show("Не удалось найти фаил, возможно он удален или перемещен");
                OnRemoveImageCommandExecuted(parametr);
            }
            FilePathImageStackPanel = UpdateImageList();
        }

        protected StackPanel UpdateImageList()
        {
            var stackPanel = new StackPanel();

            foreach (var imagePath in ImagePage.InputImage)
            {
                ContextMenu contextMenu = new ContextMenu();

                {
                    MenuItem deleteItem = new MenuItem();

                    Grid deleteItemGrid = new Grid();
                    deleteItemGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    //deleteItemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Constants.Auto });
                    //deleteItemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Constants.Auto });

                    ImageAwesome deleteImageAwesome = new ImageAwesome();
                    deleteImageAwesome.Height = 15;
                    deleteImageAwesome.Width = 15;
                    deleteImageAwesome.Icon = EFontAwesomeIcon.Solid_TrashAlt;
                    deleteImageAwesome.Foreground = new SolidColorBrush(Colors.White);
                    Grid.SetColumn(deleteImageAwesome, 0);

                    Label deleteLabel = new Label();
                    deleteLabel.FontSize = 11;
                    //deleteLabel.Content = WordsDictionary.GetWord(WordsType.DeleteFromList);
                    deleteLabel.Foreground = new SolidColorBrush(Colors.White);
                    Grid.SetColumn(deleteLabel, 1);

                    deleteItemGrid.Children.Add(deleteImageAwesome);
                    deleteItemGrid.Children.Add(deleteLabel);

                    deleteItem.Header = deleteItemGrid;

                    //deleteItem.Command = _deleteItemCommand;
                    deleteItem.Background = new SolidColorBrush(Color.FromRgb(44, 54, 70));
                    deleteItem.Foreground = new SolidColorBrush(Colors.White);
                    deleteItem.BorderBrush = new SolidColorBrush(Colors.White);
                    deleteItem.BorderThickness = new Thickness(1);
                    //deleteItem.CommandParameter = file.FilePath;
                    deleteItem.Margin = new Thickness(1);

                    contextMenu.Items.Add(deleteItem);
                }

                {
                    MenuItem openFolderItem = new MenuItem();

                    Grid openFolderItemGrid = new Grid();
                    openFolderItemGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    //openFolderItemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Constants.Auto });
                    //openFolderItemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Constants.Auto });

                    ImageAwesome openFolderImageAwesome = new ImageAwesome();
                    openFolderImageAwesome.Height = 15;
                    openFolderImageAwesome.Width = 15;
                    openFolderImageAwesome.Icon = EFontAwesomeIcon.Solid_FolderOpen;
                    openFolderImageAwesome.Foreground = new SolidColorBrush(Colors.White);
                    Grid.SetColumn(openFolderImageAwesome, 0);

                    Label openFolderLabel = new Label();
                    openFolderLabel.FontSize = 11;
                    //openFolderLabel.Content = WordsDictionary.GetWord(WordsType.OpenInFolder);
                    openFolderLabel.Foreground = new SolidColorBrush(Colors.White);
                    Grid.SetColumn(openFolderLabel, 1);

                    openFolderItemGrid.Children.Add(openFolderImageAwesome);
                    openFolderItemGrid.Children.Add(openFolderLabel);

                    openFolderItem.Header = openFolderItemGrid;

                    //openFolderItem.Command = _openFolderCommand;
                    openFolderItem.Background = new SolidColorBrush(Color.FromRgb(44, 54, 70));
                    openFolderItem.Foreground = new SolidColorBrush(Colors.White);
                    openFolderItem.BorderBrush = new SolidColorBrush(Colors.White);
                    openFolderItem.BorderThickness = new Thickness(1);
                    //openFolderItem.CommandParameter = file.FilePath;
                    openFolderItem.Margin = new Thickness(1);

                    contextMenu.Items.Add(openFolderItem);
                }

                var grid = new Grid()
                {
                    Margin = new Thickness(10, 5, 5, 5)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = Constants.GridLengthAuto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = Constants.GridLengthStar });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = Constants.GridLengthAuto });

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
                    ContextMenu = contextMenu,
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

        protected void RemoveElement(Model.Image parametr)
        {
            ImagePage.InputImage.Remove(parametr);
        }
    }
}
