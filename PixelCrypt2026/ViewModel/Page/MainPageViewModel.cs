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

        public ObservableCollection<ToolCardViewModel> Tools { get; set; }

        public MainPageViewModel(NavigationService navigation)
        {
            _navigation = navigation;

            Tools = new ObservableCollection<ToolCardViewModel>()
            {
                new ToolCardViewModel()
                {
                    Title = "Шифрование",
                    Description = "Защита изображений",
                    ToolCardParameter =  typeof(CryptographyPageViewModel),
                    ToolCardCommand = new LambdaCommand(OnNavigate)
                },
                new ToolCardViewModel()
                {
                    Title = "Стеганография",
                    Description = "Скрытие данных",
                    ToolCardParameter = typeof(SteganographyPageViewModel),
                    ToolCardCommand = new LambdaCommand(OnNavigate)
                }
            };
        }

        private void OnNavigate(object parameter)
        {
            if (parameter is Type type) _navigation.NavigateTo(type);
        }
    }
}
