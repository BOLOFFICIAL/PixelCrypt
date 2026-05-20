using PixelCrypt2026.ViewModel.Base;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ToolCardViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Type ToolCardParameter { get; set; }
        public ICommand ToolCardCommand { get; set; }
    }
}
