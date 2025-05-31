using PixelCrypt.ProgramData;
using PixelCrypt.View.Page;

namespace PixelCrypt.ViewModel.Window
{
    internal class MainWindowViewModel : Base.ViewModel
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
