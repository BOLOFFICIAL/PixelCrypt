using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;

namespace PixelCrypt2025.ViewModel.Window
{
    internal class MainWindowViewModel : Base.BaseViewModel
    {
        private System.Windows.Controls.Page _currentPage = new MainPage();

        public MainWindowViewModel()
        {
            Context.MainWindowViewModel = this;
        }

        public System.Windows.Controls.Page CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }
    }
}
