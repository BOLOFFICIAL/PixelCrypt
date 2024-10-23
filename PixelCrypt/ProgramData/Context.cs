using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;
using System.Windows.Controls;

namespace PixelCrypt.ProgramData
{
    internal class Context
    {
        public static MainWindowViewModel MainWindowViewModel { get; internal set; }
        public static Image ResultImageHeight { get; internal set; }
        public static Image ResultImageWidth { get; internal set; }
    }
}
