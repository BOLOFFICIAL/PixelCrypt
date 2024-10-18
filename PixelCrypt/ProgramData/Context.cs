using PixelCrypt.View.Window;
using PixelCrypt.ViewModel.Window;
using System.Windows.Controls;

namespace PixelCrypt.ProgramData
{
    internal class Context
    {
        public static MainWindowViewModel MainWindowViewModel { get; internal set; }
        public static Image ResultImage { get; internal set; }
        public static NotificationWindow NotificationWindow { get; internal set; }
        public static NotificationResult NotificationResult { get; internal set; }
    }
}
