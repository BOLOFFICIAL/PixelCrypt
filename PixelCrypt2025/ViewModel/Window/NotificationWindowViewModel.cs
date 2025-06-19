using PixelCrypt2025.ProgramData;

namespace PixelCrypt2025.ViewModel.Window
{
    internal class NotificationWindowViewModel : Base.BaseViewModel
    {
        private System.Windows.Controls.Page _currentPage;

        public NotificationWindowViewModel()
        {
            Context.NotificationWindowViewModel = this;
        }

        public System.Windows.Controls.Page CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }
    }
}
