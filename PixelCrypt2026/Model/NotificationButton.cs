using System.Windows.Input;

namespace PixelCrypt2026.Model
{
    internal class NotificationButton
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
    }
}
