using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program;
using PixelCrypt2026.ViewModel.Page;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Base
{
    internal abstract class BasePageLayoutViewModel : BaseViewModel
    {
        protected readonly NavigationService _navigation;

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public ICommand BackCommand { get; }

        protected BasePageLayoutViewModel(NavigationService navigation)
        {
            _navigation = navigation;

            BackCommand = new LambdaCommand(OnBackCommand);
        }

        private void OnBackCommand(object obj)
        {
            _navigation.NavigateTo<MainPageViewModel>();
        }

        protected void SetStatus(string status = "")
        {
            OperationStatusService.Instance.UpdateStatus(GetType(), status);
        }
    }
}
