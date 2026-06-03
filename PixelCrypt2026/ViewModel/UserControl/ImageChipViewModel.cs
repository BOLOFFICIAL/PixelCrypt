using PixelCrypt2026.Model;
using PixelCrypt2026.ViewModel.Base;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageChipViewModel : BaseViewModel
    {
        private string _background = "#FFFFFF";
        private string _borderBrush = "#000000";
        private string _foreground = "#000000";

        public ImageFile ImageFile { get; set; }
        public string Background
        {
            get => _background;
            private set => Set(ref _background, value);
        }

        public string BorderBrush
        {
            get => _borderBrush;
            private set => Set(ref _borderBrush, value);
        }

        public string Foreground
        {
            get => _foreground;
            private set => Set(ref _foreground, value);
        }

        public bool IsSelected
        {
            set
            {
                if (value)
                {
                    Background = "#000000";
                    BorderBrush = "#000000";
                    Foreground = "#FFFFFF";
                }
                else
                {
                    Background = "#FFFFFF";
                    BorderBrush = "#000000";
                    Foreground = "#000000";
                }
            }
        }

        public ImageChipViewModel(string filePath)
        {
            ImageFile = new ImageFile(filePath);
        }
    }
}
