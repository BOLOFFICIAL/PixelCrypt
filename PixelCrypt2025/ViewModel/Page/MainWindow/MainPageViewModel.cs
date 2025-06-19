using PixelCrypt2025.Commands.Base;
using PixelCrypt2025.ProgramData;
using System.Windows.Input;

namespace PixelCrypt2025.ViewModel.Page.MainWindow
{
    class MainPageViewModel : Base.BaseViewModel
    {
        public ICommand OpenImageProcessingPageCommand { get; }

        public MainPageViewModel()
        {
            OpenImageProcessingPageCommand = new LambdaCommand(OnOpenImageProcessingPageCommandExecuted);
        }

        private void OnOpenImageProcessingPageCommandExecuted(object p = null)
        {
            if (p is not string parametr) return;

            Context.MainWindowViewModel.CurrentPage = ImagePageFactory.CreatePage(parametr);
        }
    }
}
