using PixelCrypt2025.View.Page;
using PixelCrypt2025.View.Window;
using PixelCrypt2025.ViewModel.Window;

namespace PixelCrypt2025.ProgramData
{
    class Context
    {
        public static MainWindowViewModel MainWindowViewModel { get; internal set; }
        public static MainWindow MainWindow { get; internal set; }
        public static CryptographyPage? CryptographyPage { get; internal set; }
        public static SteganographyPage? SteganographyPage { get; internal set; }
    }
}
