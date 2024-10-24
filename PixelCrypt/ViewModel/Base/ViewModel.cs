using PixelCrypt.ProgramData;
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

        public void DoNotification(string message, string title, System.Windows.Controls.Page page, string pageTitle)
        {
            var res = Context.MainWindowViewModel.CurrentPage.GetType() == page.GetType();

            if (Context.MainWindow.WindowState != WindowState.Minimized && res && Context.MainWindow.IsActive)
            {
                Notification.MakeMessage(message, title);
                Context.MainWindow.Activate();
            }
            else
            {
                if (Context.MainWindow.WindowState != WindowState.Minimized)
                {
                    message += $".\nПерейти на страницу {pageTitle}?";
                }
                else
                {
                    message += $".\nОткрыть окно и перейти на страницу {pageTitle}?";
                }

                if (Notification.MakeMessage(message, title, NotificationButton.YesNo) == NotificationResult.Yes)
                {
                    if (Context.MainWindow.WindowState != WindowState.Minimized || !res)
                    {
                        Context.MainWindowViewModel.CurrentPage = page;
                    }
                    if (Context.MainWindow.WindowState == WindowState.Minimized)
                    {
                        Context.MainWindow.WindowState = WindowState.Normal;
                    }

                    Context.MainWindow.Activate();
                }
            }
        }
    }
}
