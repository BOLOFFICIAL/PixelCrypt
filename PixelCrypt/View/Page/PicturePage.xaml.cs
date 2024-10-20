using PixelCrypt.ProgramData;
using PixelCrypt.ViewModel.Page;

namespace PixelCrypt.View.Page
{
    /// <summary>
    /// Логика взаимодействия для PicturePage.xaml
    /// </summary>
    public partial class PicturePage : System.Windows.Controls.Page
    {
        public PicturePage()
        {
            InitializeComponent();
            DataContext = new PicturePageViewModel();
            Context.ResultImageHeight = ResultImageHeight;
            Context.ResultImageWidth = ResultImageWidth;
        }
    }
}
