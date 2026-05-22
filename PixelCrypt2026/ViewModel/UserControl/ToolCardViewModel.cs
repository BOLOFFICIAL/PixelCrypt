using PixelCrypt2026.ViewModel.Base;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ToolCardViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public ICommand ToolCardCommand { get; set; }
    }

    class ToolCardViewModel<T> : ToolCardViewModel where T : BasePageLayoutViewModel
    {
        public Type ToolCardParameter => typeof(T);
    }
}
