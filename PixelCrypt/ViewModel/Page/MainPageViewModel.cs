using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View.Page;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Page
{
    internal class MainPageViewModel : Base.ViewModel
    {
        public ICommand OpenPageCommand { get; }

        public MainPageViewModel()
        {
            OpenPageCommand = new LambdaCommand(OnOpenPageCommandExecuted);
        }

        private void OnOpenPageCommandExecuted(object p = null)
        {
            if (p is not string pageName) return;

            switch (pageName)
            {
                case "TextInPicturePage": Context.MainWindowViewModel.CurrentPage = new TextInPicturePage(); break;
                case "PicturePage": Context.MainWindowViewModel.CurrentPage = new PicturePage(); break;
            }
        }
    }
}
