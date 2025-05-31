using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Page
{
    class MainPageViewModel : Base.ViewModel
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
