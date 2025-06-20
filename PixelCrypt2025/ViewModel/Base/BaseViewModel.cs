using PixelCrypt2025.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PixelCrypt2025.ViewModel.Base
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
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

        public string Background => Theme.Background;
        public string ContentBackground => Theme.ContentBackground;
        public string Foreground => Theme.Foreground;
    }
}
