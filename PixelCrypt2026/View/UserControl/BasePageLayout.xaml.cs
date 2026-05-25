using System.Windows;

namespace PixelCrypt2026.View.UserControl
{
    /// <summary>
    /// Логика взаимодействия для BasePageLayout.xaml
    /// </summary>
    public partial class BasePageLayout : System.Windows.Controls.UserControl
    {
        public BasePageLayout()
        {
            InitializeComponent();
        }

        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(BasePageLayout),
                new PropertyMetadata(null));
    }
}
