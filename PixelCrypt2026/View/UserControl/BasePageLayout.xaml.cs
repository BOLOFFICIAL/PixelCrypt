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

        public object PageContent
        {
            get => GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public static readonly DependencyProperty PageContentProperty =
            DependencyProperty.Register(
                nameof(PageContent),
                typeof(object),
                typeof(BasePageLayout),
                new PropertyMetadata(null));
    }
}
