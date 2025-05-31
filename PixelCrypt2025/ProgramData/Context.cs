using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;

namespace PixelCrypt.ProgramData
{
    class Context
    {
        public static MainWindowViewModel MainWindowViewModel { get; internal set; }
        public static MainWindow MainWindow { get; internal set; }
    }
}
