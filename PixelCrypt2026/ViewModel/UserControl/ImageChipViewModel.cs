using PixelCrypt2026.Model;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;
using System.Windows.Media;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageChipViewModel : BaseViewModel
    {
        private string _background;
        private string _foreground;
        private string _statusValue;
        private string _statusBackground;
        private GridLength _statusWidth;
        private Status _status;

        public ImageChipViewModel(string filePath)
        {
            ImageFile = new ImageFile(filePath);
            StatusWidth = new GridLength(0, GridUnitType.Star);

            Background = (Application.Current.TryFindResource("Background2") as SolidColorBrush).Color.ToString();
            Foreground = (Application.Current.TryFindResource("Foreground") as SolidColorBrush).Color.ToString();
        }

        public ImageFile ImageFile { get; set; }
        public string Background
        {
            get => _background;
            private set => Set(ref _background, value);
        }

        public string Foreground
        {
            get => _foreground;
            private set => Set(ref _foreground, value);
        }

        public GridLength StatusWidth
        {
            get => _statusWidth;
            private set => Set(ref _statusWidth, value);
        }

        public string StatusValue
        {
            get => _statusValue;
            private set => Set(ref _statusValue, value);
        }

        public string StatusBackground
        {
            get => _statusBackground;
            private set => Set(ref _statusBackground, value);
        }

        public Status Status
        {
            get => _status;
            set
            {
                Set(ref _status, value);
                SetStatus(_status);
            }
        }

        public bool IsSelected
        {
            set
            {
                if (value)
                {
                    var originalBackground = (Application.Current.TryFindResource("Background2") as SolidColorBrush).Color;

                    Background = Color.FromRgb(
                        (byte)(255 - originalBackground.R),
                        (byte)(255 - originalBackground.G),
                        (byte)(255 - originalBackground.B)
                    ).ToString();

                    var originalForeground = (Application.Current.TryFindResource("Foreground") as SolidColorBrush).Color;

                    Foreground = Color.FromRgb(
                        (byte)(255 - originalForeground.R),
                        (byte)(255 - originalForeground.G),
                        (byte)(255 - originalForeground.B)
                    ).ToString();
                }
                else
                {
                    Background = (Application.Current.TryFindResource("Background2") as SolidColorBrush).Color.ToString();
                    Foreground = (Application.Current.TryFindResource("Foreground") as SolidColorBrush).Color.ToString();
                }
            }
        }

        private void SetStatus(Status status = Status.None)
        {
            StatusWidth = status == Status.None
                ? new GridLength(0, GridUnitType.Star)
                : new GridLength(1, GridUnitType.Auto);

            switch (status)
            {
                case Status.None:
                    {
                        StatusValue = "";
                        StatusBackground = "#00000000";
                        break;
                    }
                case Status.InProgress:
                    {
                        StatusValue = "⏳";
                        StatusBackground = (Application.Current.TryFindResource("Background2") as SolidColorBrush).Color.ToString();
                        break;
                    }
                case Status.Success:
                    {
                        StatusValue = "Ok";
                        StatusBackground = "#228B22";
                        break;
                    }
                case Status.Failed:
                    {
                        StatusValue = "No";
                        StatusBackground = "#DC143C";
                        break;
                    }

                default:
                    {
                        StatusValue = status.ToString();
                        StatusBackground = "#00000000";
                        break;
                    }
            }
        }
    }
}
