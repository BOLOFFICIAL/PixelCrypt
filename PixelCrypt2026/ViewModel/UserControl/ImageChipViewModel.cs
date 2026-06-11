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
        private StatusType _status;

        public ImageChipViewModel(string filePath)
        {
            ImageFile = new ImageFile(filePath);
            StatusWidth = new GridLength(0, GridUnitType.Star);

            Background = (Application.Current.TryFindResource("ImageChipBackground") as SolidColorBrush).Color.ToString();
            Foreground = (Application.Current.TryFindResource("ImageChipForeground") as SolidColorBrush).Color.ToString();
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

        public StatusType Status
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
                    Background = (Application.Current.TryFindResource("ImageChipSelectedBackground") as SolidColorBrush).Color.ToString();
                    Foreground = (Application.Current.TryFindResource("ImageChipSelectedForeground") as SolidColorBrush).Color.ToString();
                }
                else
                {
                    Background = (Application.Current.TryFindResource("ImageChipBackground") as SolidColorBrush).Color.ToString();
                    Foreground = (Application.Current.TryFindResource("ImageChipForeground") as SolidColorBrush).Color.ToString();
                }
            }
        }

        private void SetStatus(StatusType status = StatusType.None)
        {
            StatusWidth = status == StatusType.None
                ? new GridLength(0, GridUnitType.Star)
                : new GridLength(1, GridUnitType.Auto);

            switch (status)
            {
                case StatusType.None:
                    {
                        StatusValue = "";
                        StatusBackground = (Application.Current.TryFindResource("StatusNone") as SolidColorBrush).Color.ToString();
                        break;
                    }
                case StatusType.InProgress:
                    {
                        StatusValue = "⏳";
                        StatusBackground = (Application.Current.TryFindResource("StatusInProgress") as SolidColorBrush).Color.ToString();
                        break;
                    }
                case StatusType.Success:
                    {
                        StatusValue = "Ok";
                        StatusBackground = (Application.Current.TryFindResource("StatusSuccess") as SolidColorBrush).Color.ToString();
                        break;
                    }
                case StatusType.Failed:
                    {
                        StatusValue = "No";
                        StatusBackground = (Application.Current.TryFindResource("StatusFailed") as SolidColorBrush).Color.ToString();
                        break;
                    }

                default:
                    {
                        StatusValue = status.ToString();
                        StatusBackground = (Application.Current.TryFindResource("StatusNone") as SolidColorBrush).Color.ToString();
                        break;
                    }
            }
        }
    }
}
