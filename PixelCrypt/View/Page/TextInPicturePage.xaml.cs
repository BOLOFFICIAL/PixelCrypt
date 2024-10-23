using PixelCrypt.ProgramData;
using PixelCrypt.ViewModel.Page;

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
