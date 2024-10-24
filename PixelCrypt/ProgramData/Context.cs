using PixelCrypt.ViewModel.Base;
using PixelCrypt.ViewModel.Page;
using PixelCrypt.ViewModel.Window;
using System.Windows.Controls;

namespace PixelCrypt.ProgramData
{
    internal class Context
    {
        public static MainWindowViewModel MainWindowViewModel { get; internal set; } = new MainWindowViewModel();
        public static MainPageViewModel MainPageViewModel { get; internal set; } = new MainPageViewModel();
        public static PicturePageViewModel PicturePageViewModel { get; internal set; } = new PicturePageViewModel();
        public static TextInPicturePageViewModel TextInPicturePageViewModel { get; internal set; } = new TextInPicturePageViewModel();
        public static MainWindow MainWindow { get; internal set; }
    }
}
