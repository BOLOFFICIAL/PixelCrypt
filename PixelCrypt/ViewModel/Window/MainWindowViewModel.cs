using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;

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
