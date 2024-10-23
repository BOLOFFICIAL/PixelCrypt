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
            var vm = Context.PicturePageViewModel;
            DataContext = vm;
            vm.ResultHeightImage = ResultImageHeight;
            vm.ResultWidthImage = ResultImageWidth;
            vm.InitializeImage();
        }
    }
}
