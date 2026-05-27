using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.UserControl;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Page
{
    internal class CryptographyPageViewModel : BasePageLayoutViewModel
    {
        public ImageListViewModel ImageList { get; set; }

        public ICommand DoCommand { get; set; }

        public CryptographyPageViewModel(NavigationService navigation) : base(navigation)
        {
            Title = $"Шифрование";
            ImageList = new ImageListViewModel();

            DoCommand = new LambdaCommand(OnDoCommand);
        }

        private async void OnDoCommand(object obj)
        {
            foreach (var el in ImageList.Images)
            {
                ImageList.SelectedImage = el;

                await Task.Delay(100);
            }
        }
    }
}
