using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Collections.ObjectModel;

namespace PixelCrypt2026.ViewModel.Page
{
    class MainPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly Dictionary<Type, ToolCardViewModel> _tools;

        public ObservableCollection<ToolCardViewModel> Tools { get; }

        public MainPageViewModel(NavigationService navigation)
        {
            _navigation = navigation;

            _tools = new Dictionary<Type, ToolCardViewModel>()
            {
                {
                    typeof(CryptographyPageViewModel),
                    new ToolCardViewModel<CryptographyPageViewModel>()
                    {
                        Title = "Cryptography",
                        Description = "Protect the image with a password by turning it into visual noise. No one will see the original without the key.",
                        ToolCardCommand = new LambdaCommand(OnNavigate)
                    }
                },
                {
                    typeof(SteganographyPageViewModel),
                    new ToolCardViewModel<SteganographyPageViewModel>()
                    {
                        Title = "Steganography",
                        Description = "Hide the text or file inside the image unnoticeably. Externally, the picture remains the same.",
                        ToolCardCommand = new LambdaCommand(OnNavigate)
                    }
                }
            };

            Tools = new ObservableCollection<ToolCardViewModel>(_tools.Values);

            OperationStatusService.Instance.StatusChanged += OnSetStatus;
        }

        private void OnSetStatus(Type type, string status)
        {
            _tools[type].Status = status;
        }

        private void OnNavigate(object parameter)
        {
            if (parameter is Type type)
                _navigation.NavigateTo(type);
        }
    }
}
