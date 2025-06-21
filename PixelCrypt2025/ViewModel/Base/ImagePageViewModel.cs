using Microsoft.Win32;
using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.Enum;
using PixelCrypt2025.Interfaces;
using PixelCrypt2025.Model;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt2025.ViewModel.Base
{
    internal class ImagePageViewModel : BaseViewModel
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
        private bool _isSuccessResult = false;
        private bool _isButtonFree = true;

        protected ICommand ShowImageCommand { get; init; }
        protected ICommand RemoveImageCommand { get; init; }

        private ICommand MoveUpImageCommand { get; }
        private ICommand MoveDownImageCommand { get; }

        private Func<string, Task<ActionResult>> _inputAction;
        private Func<string, Task<ActionResult>> _outputAction;

        public IImagePage ImagePage { get; init; }

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand PaswordViewCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand DoActionCommand { get; init; }
        public ICommand SaveCommand { get; }

        public ImagePageViewModel()
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);
            ClearImageCommand = new LambdaCommand(OnClearImageCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            MoveUpImageCommand = new LambdaCommand(OnMoveUpImageCommandExecuted);
            MoveDownImageCommand = new LambdaCommand(OnMoveDownImageCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);

            OnPaswordViewCommandExecuted();

            AddGridHeight = Constants.GridLengthStar;
        }

        public bool IsButtonFree
        {
            get => _isButtonFree;
            set => Set(ref _isButtonFree, value);
        }

        protected bool IsSuccessResult
        {
            get => _isSuccessResult;
            set
            {
                Set(ref _isSuccessResult, value);
                if (!value)
                {
                    ImagePage.OutputImage.Clear();
                    UpdateList();
                }
                SaveDataWidth = value ? Constants.GridLengthAuto : Constants.GridLengthZero;
            }
        }

        public Func<string, Task<ActionResult>> InputAction
        {
            get => _inputAction;
            set => Set(ref _inputAction, value);
        }

        public Func<string, Task<ActionResult>> OutputAction
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

        protected void OnAddImageCommandExecuted(object p = null)
        {
            if (AccessReset("Добавление элемента приведет к потере рузльтата")) return;

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

                foreach (var filepath in imageList) AddImage(filepath);

                if (ImagePage.InputImage.Count != prefCount)
                {
                    IsSuccessResult = false;
                    SaveDataWidth = Constants.GridLengthZero;
                    UpdateList();

                    if (SelecedImage is null) OnShowImageCommandExecuted(ImagePage.InputImage.Last());
                }
                else if (imageList.Count() == 0)
                {
                    Notification.Show("Не удалось найти подходящие элементы", openFileDialog.Title, status: NotificationStatus.Error);
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
            if (AccessReset("Очистка списка приведет к потере рузльтата")) return;

            IsSuccessResult = false;
            ImagePage.InputImage.Clear();

            AddGridHeight = Constants.GridLengthStar;
            DataGridHeight = Constants.GridLengthZero;
            ViewImageWidth = Constants.GridLengthZero;

            SelecedImage = null;
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

            if (ImagePage.OutputImage.ContainsKey(parametr) && AccessReset("Удаление элемента приведет к потере рузльтата")) return;

            if (IsSuccessResult && ImagePage.OutputImage.ContainsKey(parametr))
            {
                IsSuccessResult = false;
            }

            ImagePage.InputImage.Remove(parametr);

            UpdateList();

            if (SelecedImage == parametr)
            {
                SelecedImage = null;
                ViewImageWidth = Constants.GridLengthZero;
            }

            if (ImagePage.InputImage.Count == 0)
            {
                IsSuccessResult = false;
                AddGridHeight = Constants.GridLengthStar;
                DataGridHeight = Constants.GridLengthZero;
                SelecedImage = null;
            }
        }

        protected virtual void OnMoveUpImageCommandExecuted(object p = null)
        {
            if (p is Model.Image image) MoveImage(image, -1);
        }

        protected virtual void OnMoveDownImageCommandExecuted(object p = null)
        {
            if (p is Model.Image image) MoveImage(image, 1);
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
                Notification.Show("Не удалось найти фаил, возможно он удален или перемещен", "Просмотр изображения", status: NotificationStatus.Error);
                OnRemoveImageCommandExecuted(parametr);
            }

            UpdateList();
        }

        private void OnSaveCommandExecuted(object obj)
        {
            var result = ImagePage.SaveData();

            var status = result.IsSuccessResult ? NotificationStatus.Success : NotificationStatus.Error;

            Notification.Show($"{result.ResultMessage}", result.ResultTitle, status: status);
        }

        private async Task<StackPanel> UpdateImageList()
        {
            var stackPanel = new StackPanel();

            var index = 0;

            foreach (var imageData in ImagePage.InputImage)
            {
                ContextMenu contextMenu = new ContextMenu
                {
                    Style = (Style)Application.Current.FindResource("BaseContextMenuStyle"),
                };

                contextMenu.Items.Add(CreateMenuItem("Удалить", RemoveImageCommand, imageData));

                if (ImagePage.InputImage.Count > 1)
                {
                    if (index != 0)
                        contextMenu.Items.Add(CreateMenuItem("Поднять", MoveUpImageCommand, imageData));

                    if (index != ImagePage.InputImage.Count - 1)
                        contextMenu.Items.Add(CreateMenuItem("Опусутить", MoveDownImageCommand, imageData));
                }

                var grid = new Grid()
                {
                    Margin = new Thickness(10, 0, 10, (index != ImagePage.InputImage.Count - 1) ? 10 : 0)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = Constants.GridLengthAuto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = Constants.GridLengthStar });

                if (ImagePage.OutputImage.ContainsKey(imageData))
                {
                    var border = new Border
                    {
                        BorderBrush = (Brush)new BrushConverter().ConvertFromString(Color3),
                        BorderThickness = new Thickness(1),
                        VerticalAlignment = VerticalAlignment.Top,
                        CornerRadius = new CornerRadius(10),
                        Margin = new Thickness(0, 0, 10, 0),
                        Child = new TextBlock()
                        {
                            Text = "Ok",
                            FontSize = 15,
                            Padding = new Thickness(10),
                            Foreground = (Brush)new BrushConverter().ConvertFromString(Color3),
                        },
                    };

                    Grid.SetColumn(border, 0);

                    grid.Children.Add(border);
                }

                var button = new Button()
                {
                    Content = new TextBlock()
                    {
                        Text = imageData.ToString(),
                        FontSize = 15,
                        TextWrapping = TextWrapping.Wrap,
                        Padding = new Thickness(10, 10, 10, 10),
                    },
                    ContextMenu = contextMenu,
                    IsEnabled = IsButtonFree,
                    Command = ShowImageCommand,
                    Style = (Style)Application.Current.FindResource(imageData == SelecedImage ? "SelectedListButtonStyle" : "BaseListButtonStyle"),
                    CommandParameter = imageData,
                    BorderThickness = new Thickness(1)
                };

                Grid.SetColumn(button, 1);

                grid.Children.Add(button);

                stackPanel.Children.Add(grid);

                index++;
            }

            return stackPanel;
        }

        private MenuItem CreateMenuItem(string title, ICommand command, object parametr)
        {
            return new MenuItem()
            {
                Header = new Label()
                {
                    FontSize = 13,
                    Content = title,
                    Foreground = (Brush)new BrushConverter().ConvertFromString(Color3),
                },
                Command = command,
                Style = (Style)Application.Current.FindResource("BaseMenuItemStyle"),
                CommandParameter = parametr,
                Margin = new Thickness(1)
            };
        }

        private void AddImage(string filepath)
        {
            foreach (var image in ImagePage.InputImage)
            {
                if (image.Path == filepath) return;
            }

            ImagePage.InputImage.Add(new Model.Image(filepath));
        }

        private void MoveImage(Model.Image image, int direction)
        {
            var list = ImagePage.InputImage;
            var index = list.IndexOf(image);
            var newIndex = index + direction;

            if (image is null || index < 0 || newIndex < 0 || newIndex >= list.Count) return;

            (list[index], list[newIndex]) = (list[newIndex], list[index]);
            UpdateList();
        }

        protected async Task UpdateList()
        {
            FilePathImageStackPanel = await UpdateImageList();
        }

        protected bool AccessReset(string message, string title = "PixelCrypt")
        {
            return IsSuccessResult && Notification.Show($"{message}.\nПродолжить?", title, NotificationType.YesNo, status: NotificationStatus.Question).Result == NotificationResultType.No;
        }
    }
}