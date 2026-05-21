using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.UserControl;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Шифрование создано в {DateTime.Now}";
        }
    }
}
