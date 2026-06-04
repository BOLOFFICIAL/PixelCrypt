using PixelCrypt2026.Model;
using PixelCrypt2026.Program.Enum;
using PixelCrypt2026.ViewModel.Base;
using System.Windows;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ImageChipViewModel : BaseViewModel
    {
        private string _background = "#FFFFFF";
        private string _borderBrush = "#000000";
        private string _foreground = "#000000";
        private string _statusValue;
        private GridLength _statusWidth;
        private Status _status;

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
            StatusWidth = new GridLength(0, GridUnitType.Star);
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
                        break;
                    }
                case Status.InProgress:
                    {
                        StatusValue = "⏳";
                        break;
                    }
                case Status.Success:
                    {
                        StatusValue = "Ok";
                        break;
                    }
                case Status.Failed:
                    {
                        StatusValue = "No";
                        break;
                    }

                default:
                    {
                        StatusValue = status.ToString();
                        break;
                    }
            }
        }
    }
}
