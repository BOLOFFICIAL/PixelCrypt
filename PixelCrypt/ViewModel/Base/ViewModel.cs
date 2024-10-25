using PixelCrypt.ProgramData;
using PixelCrypt.View;
using PixelCrypt.View.Page;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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

            if (Notification.MakeMessage(message, title, NotificationButton.YesNo) == NotificationResult.Yes)
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
    }
}
