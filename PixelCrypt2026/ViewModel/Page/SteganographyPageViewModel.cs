using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.UserControl;

namespace PixelCrypt2026.ViewModel.Page
{
    class SteganographyPageViewModel : BasePageLayoutViewModel
    {
        public SteganographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Стеганография создано в {DateTime.Now}";
        }
    }
}
