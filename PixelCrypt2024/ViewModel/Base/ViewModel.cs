using FontAwesome5;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using PixelCrypt.View.Page;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt.ViewModel.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        public string Color1 => "#1E1E1E";
        public string Color2 => "#A52A2A";
        public string Color3 => "#FFFFFF";
        public string Color4 => "#303336";
        public string Color5 => "#32CD32";

        public void DoNotification(string message, string title, Type pageType, string pageTitle)
        {
            var isThisPage = Context.MainWindowViewModel.CurrentPage.GetType() == pageType;
            var isWindowMinimized = Context.MainWindow.WindowState == WindowState.Minimized;

            if (!isWindowMinimized && isThisPage && Context.MainWindow.IsActive)
            {
                Notification.MakeMessage(message, title);
                Context.MainWindow.Activate();
                return;
            }

            message += isWindowMinimized ? $".\nОткрыть окно и перейти на страницу {pageTitle}?" : $".\nПерейти на страницу {pageTitle}?";

            if (Notification.MakeMessage(message, title, NotificationButton.YesNo).NotificationResultType == NotificationResultType.Yes)
            {
                isWindowMinimized = Context.MainWindow.WindowState == WindowState.Minimized;

                if (!isWindowMinimized || !isThisPage)
                {
                    Context.MainWindowViewModel.CurrentPage = GetPageByType(pageType);
                }
                if (isWindowMinimized)
                {
                    Context.MainWindow.WindowState = WindowState.Normal;
                }

                Context.MainWindow.Activate();
            }
        }

        private System.Windows.Controls.Page GetPageByType(Type pageType)
        {
            return pageType.Name switch
            {
                "PicturePage" => new PicturePage(),
                "TextInPicturePage" => new TextInPicturePage(),
                _ => new MainPage(),
            };
        }

        protected StackPanel LoadFilePathImages(List<string> filePathImages, ICommand showImageCommand, ICommand removeImageCommand, int selectedElementIndex, bool isButtonFree, int count = 0)
        {
            var stackPanel = new StackPanel();
            int index = 0;

            foreach (var image in filePathImages)
            {
                var grid = new Grid()
                {
                    Margin = new Thickness(0, 10, 0, 0)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                var icon = new ImageAwesome
                {
                    Icon = EFontAwesomeIcon.Regular_CheckCircle,
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(0, 0, 10, 0),
                    Foreground = (Brush)new BrushConverter().ConvertFromString(Color5)
                };

                Grid.SetColumn(icon, 0);

                var imageName = new TextBlock()
                {
                    Text = Path.GetFileName(image),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                };

                var button = new Button()
                {
                    Command = showImageCommand,
                    CommandParameter = index,
                    Height = double.NaN,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Foreground = (Brush)new BrushConverter().ConvertFromString(Color3),
                    Background = (Brush)new BrushConverter().ConvertFromString(Color4),
                    BorderBrush = (Brush)new BrushConverter().ConvertFromString(Color3),
                    BorderThickness = new Thickness(2)
                };

                if (index == selectedElementIndex)
                {
                    button.Background = (Brush)new BrushConverter().ConvertFromString(Color3);
                    button.Foreground = (Brush)new BrushConverter().ConvertFromString(Color4);
                    button.BorderThickness = new Thickness(0);
                }

                button.Content = imageName;

                Grid.SetColumn(button, 1);

                var deleteButton = new Button
                {
                    Margin = new Thickness(5, 0, 0, 0),
                    Width = 35,
                    Height = 35,
                    Padding = new Thickness(0),
                    Content = new ImageAwesome
                    {
                        Icon = EFontAwesomeIcon.Regular_TimesCircle,
                        Width = 25,
                        Height = 25,
                        Foreground = (Brush)new BrushConverter().ConvertFromString(Color3)
                    },
                    Command = removeImageCommand,
                    IsEnabled = isButtonFree,
                    CommandParameter = index,
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = System.Windows.Media.Brushes.Transparent,
                };

                Grid.SetColumn(deleteButton, 2);

                if (index < count)
                {
                    grid.Children.Add(icon);
                }

                grid.Children.Add(button);
                grid.Children.Add(deleteButton);
                stackPanel.Children.Add(grid);

                index++;
            }

            return stackPanel;
        }
    }
}
