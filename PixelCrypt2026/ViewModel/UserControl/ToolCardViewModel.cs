using PixelCrypt2026.ViewModel.Base;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    public class ToolCardViewModel : BaseViewModel
    {
        private string _status;

        public string Title { get; set; }
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
        public string Description { get; set; }
        public ICommand ToolCardCommand { get; set; }
    }

    class ToolCardViewModel<T> : ToolCardViewModel where T : BasePageLayoutViewModel
    {
        public Type ToolCardParameter => typeof(T);
    }
}
