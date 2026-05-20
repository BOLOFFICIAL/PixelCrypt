using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.Page;

namespace PixelCrypt2026.ViewModel.Window
{
    class MainWindowViewModel : BaseViewModel
    {
        private System.Windows.Controls.Page _currentPage;

        public MainWindowViewModel(NavigationService navigation)
        {
            navigation.NavigateTo<MainPageViewModel>();
        }

        public System.Windows.Controls.Page CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }
    }
}
