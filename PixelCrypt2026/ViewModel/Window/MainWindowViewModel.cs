using PixelCrypt2026.View.Page;
using PixelCrypt2026.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelCrypt2026.ViewModel.Window
{
    class MainWindowViewModel : BaseViewModel
    {
        private System.Windows.Controls.Page _currentPage = new MainPage();

        public MainWindowViewModel()
        {
        }

        public System.Windows.Controls.Page CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }
    }
}
