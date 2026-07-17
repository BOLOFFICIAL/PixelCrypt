using PixelCrypt2026.ViewModel.Base;
using System.Windows.Input;
using System.Windows.Media;

namespace PixelCrypt2026.Model
{
    internal class UIButton : BaseViewModel
    {
        private string _background;
        private string _foreground;

        public string Text { get; set; }

        public ICommand Command { get; set; }

        public string Background
        {
            get => _background;
            set => Set(ref _background, value);
        }

        public string Foreground
        {
            get => _foreground;
            set => Set(ref _foreground, value);
        }
    }
}
