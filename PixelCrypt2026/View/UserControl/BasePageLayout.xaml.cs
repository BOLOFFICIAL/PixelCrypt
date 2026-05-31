using System.Windows;
using System.Windows.Input;

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
                typeof(BasePageLayout));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(BasePageLayout));

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register(
                nameof(BackCommand),
                typeof(ICommand),
                typeof(BasePageLayout));
    }
}