using System.Windows;

namespace PixelCrypt2026.View.UserControl.Mini
{
    /// <summary>
    /// Логика взаимодействия для ImageToolTip.xaml
    /// </summary>
    public partial class ImageToolTip : System.Windows.Controls.UserControl
    {
        public ImageToolTip()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ImageToolTip));

        public string FileSizeFormatted
        {
            get => (string)GetValue(FileSizeFormattedProperty);
            set => SetValue(FileSizeFormattedProperty, value);
        }

        public static readonly DependencyProperty FileSizeFormattedProperty =
            DependencyProperty.Register(nameof(FileSizeFormatted), typeof(string), typeof(ImageToolTip));

        public string ImageResolution
        {
            get => (string)GetValue(ImageResolutionProperty);
            set => SetValue(ImageResolutionProperty, value);
        }

        public static readonly DependencyProperty ImageResolutionProperty =
            DependencyProperty.Register(nameof(ImageResolution), typeof(string), typeof(ImageToolTip));

        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(ImageToolTip));

        public string Foreground
        {
            get => (string)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(string), typeof(ImageToolTip));
    }
}
