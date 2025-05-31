using FontAwesome5;
using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.Model;
using PixelCrypt.ProgramData;
using PixelCrypt.View.Page;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt.ViewModel.Page
{
    internal class SteganographyPageViewModel : Base.ViewModel
    {
        private Steganography<Model.File> _steganography;

        private string _showPasword = "";
        private string _password = "";
        private string _inputFile = "";

        private Model.Image _selecedImage = null;

        private StackPanel _filePathImageStackPanel;

        private GridLength _closePasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _openPasswordWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveDataWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _addGridHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _dataGridHeight = new GridLength(0, GridUnitType.Pixel);
        private GridLength _viewImageWidth = new GridLength(0, GridUnitType.Pixel);

        private bool _isOpenPassword = false;

        private Action _encrypt;
        private Action _decrypt;

        private ICommand ShowImageCommand { get; }

        public ICommand ClosePageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand DoActionCommand { get; }
        public ICommand PaswordViewCommand { get; }
        public ICommand ChooseFileCommand { get; }

        public SteganographyPageViewModel()
        {
            _steganography = new Steganography<Model.File>(new Model.File());

            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            AddImageCommand = new LambdaCommand(OnAddImageCommandExecuted);
            RemoveImageCommand = new LambdaCommand(OnRemoveImageCommandExecuted);
            PaswordViewCommand = new LambdaCommand(OnPaswordViewCommandExecuted);
            ClearImageCommand = new LambdaCommand(OnClearImageCommandExecuted);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            ShowImageCommand = new LambdaCommand(OnShowImageCommandExecuted);
            ChooseFileCommand = new LambdaCommand(OnChooseFileCommandExecuted);

            OnPaswordViewCommandExecuted();

            AddGridHeight = new GridLength(1, GridUnitType.Star);
            Encrypt = _steganography.EncryptAction;
            Decrypt = _steganography.DecryptAction;
        }

        public StackPanel FilePathImageStackPanel
        {
            get => _filePathImageStackPanel;
            set => Set(ref _filePathImageStackPanel, value);
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

        public string InputFile
        {
            get => _inputFile;
            set => Set(ref _inputFile, value);
        }

        public string InputData
        {
            get => _steganography.InputData.Content;
            set => Set(ref _steganography.InputData.Content, value);
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

        private void OnClosePageCommandExecuted(object p = null)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnAddImageCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
                Multiselect = true,
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                var prefCount = _steganography.ContextImage.Count;

                foreach (var filepath in openFileDialog.FileNames)
                {
                    _steganography.AddElement(filepath);
                }

                if (_steganography.ContextImage.Count != prefCount)
                {
                    SaveDataWidth = new GridLength(0, GridUnitType.Star);
                    FilePathImageStackPanel = UpdateImageList();
                }
            }
            if (_steganography.ContextImage.Count > 0)
            {
                AddGridHeight = new GridLength(0, GridUnitType.Star);
                DataGridHeight = new GridLength(1, GridUnitType.Star);
            }
        }

        private void OnRemoveImageCommandExecuted(object p = null)
        {
            if (p is not Model.Image parametr) return;

            _steganography.RemoveElement(parametr);
            FilePathImageStackPanel = UpdateImageList();

            if (_selecedImage == parametr)
            {
                _selecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
            }

            if (_steganography.ContextImage.Count == 0)
            {
                AddGridHeight = new GridLength(1, GridUnitType.Star);
                DataGridHeight = new GridLength(0, GridUnitType.Star);
                _selecedImage = null;
            }
        }

        private void OnClearImageCommandExecuted(object p = null)
        {
            _steganography.ContextImage.Clear();

            AddGridHeight = new GridLength(1, GridUnitType.Star);
            DataGridHeight = new GridLength(0, GridUnitType.Star);
            _selecedImage = null;
            ViewImageWidth = new GridLength(0, GridUnitType.Star);
        }

        private void OnDoActionCommandExecuted(object p = null)
        {
            if (p is not Action action) return;
            action();
            SaveDataWidth = new GridLength(1, GridUnitType.Star);
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

            if (_selecedImage == parametr)
            {
                _selecedImage = null;
                ViewImageWidth = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                _selecedImage = parametr;
                ViewImageWidth = new GridLength(1, GridUnitType.Star);
            }
            FilePathImageStackPanel = UpdateImageList();
        }

        private void OnChooseFileCommandExecuted(object p = null)
        {

        }

        private StackPanel UpdateImageList()
        {
            var stackPanel = new StackPanel();

            foreach (var imagePath in _steganography.ContextImage)
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
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(10, 10, 10, 10),
                };

                var button = new Button()
                {
                    Content = textBlock,
                    Command = ShowImageCommand,
                    Style = (Style)Application.Current.FindResource(imagePath == _selecedImage ? "SelectedListButtonStyle" : "BaseListButtonStyle"),
                    CommandParameter = imagePath,
                    BorderThickness = new Thickness(1)
                };

                Grid.SetColumn(button, 1);

                var deleteButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("ToolButtonStyle"),
                    Margin = new Thickness(5, 0, 0, 0),
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
    }
}
