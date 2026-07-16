using System.Windows;

namespace PixelCrypt2026.View.UserControl.Mini
{
    /// <summary>
    /// Логика взаимодействия для ImageToolTip.xaml
    /// </summary>
    public partial class InfoToolTip : System.Windows.Controls.UserControl
    {
        public InfoToolTip()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(InfoToolTip));

        public string Info
        {
            get => (string)GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }

        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register(nameof(Info), typeof(string), typeof(InfoToolTip));
    }
}
