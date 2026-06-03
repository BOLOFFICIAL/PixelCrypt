using PixelCrypt2026.ViewModel.UserControl;
using System.Windows;

namespace PixelCrypt2026.View.UserControl
{
    /// <summary>
    /// Логика взаимодействия для ImageChip.xaml
    /// </summary>
    public partial class ImageChip : System.Windows.Controls.UserControl
    {
        public ImageChip()
        {
            InitializeComponent();
        }

        public ImageChipViewModel Image
        {
            get => (ImageChipViewModel)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(ImageChipViewModel),
                typeof(ImageChip));
    }
}