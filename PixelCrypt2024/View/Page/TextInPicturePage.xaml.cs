using PixelCrypt.ProgramData;

namespace PixelCrypt.View.Page
{
    /// <summary>
    /// Логика взаимодействия для TextInPicturePage.xaml
    /// </summary>
    public partial class TextInPicturePage : System.Windows.Controls.Page
    {
        public TextInPicturePage()
        {
            InitializeComponent();
            DataContext = Context.TextInPicturePageViewModel;
        }
    }
}
