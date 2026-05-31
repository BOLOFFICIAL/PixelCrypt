using PixelCrypt2026.Model;
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

        public ImageFile Image
        {
            get => (ImageFile)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(ImageFile),
                typeof(ImageChip));
    }
}