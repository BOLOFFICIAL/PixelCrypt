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

        public string Color1 => Palette.Color1;
        public string Color2 => Palette.Color2;
        public string Color3 => Palette.Color3;
    }
}
