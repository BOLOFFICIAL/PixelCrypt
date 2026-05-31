using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Collections.ObjectModel;

namespace PixelCrypt2026.ViewModel.Page
{
    class MainPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;

        public ObservableCollection<ToolCardViewModel> Tools { get; }

        public MainPageViewModel(NavigationService navigation)
        {
            _navigation = navigation;

            Tools = new ObservableCollection<ToolCardViewModel>()
            {
                new ToolCardViewModel<CryptographyPageViewModel>()
                {
                    Title = "Шифрование",
                    Description = "Защитите изображение паролем, превратив его в визуальный шум. Без ключа никто не увидит оригинал.",
                    ToolCardCommand = new LambdaCommand(OnNavigate)
                },
                new ToolCardViewModel<SteganographyPageViewModel>()
                {
                    Title = "Стеганография",
                    Description = "Спрячьте текст или файл внутри изображения незаметно. Внешне картинка остаётся прежней.",
                    ToolCardCommand = new LambdaCommand(OnNavigate)
                },
            };
        }

        private void OnNavigate(object parameter)
        {
            if (parameter is Type type)
                _navigation.NavigateTo(type);
        }
    }
}
