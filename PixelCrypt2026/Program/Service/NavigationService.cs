using PixelCrypt2026.ViewModel.Base;
using System.Windows.Controls;

namespace PixelCrypt2026.Program.Service
{
    internal class NavigationService
    {
        private readonly Frame _frame;
        private readonly Dictionary<Type, Page> _pageCache = new();

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Register<TViewModel>(Page page) where TViewModel : BaseViewModel
        {
            var vmType = typeof(TViewModel);
            if (!_pageCache.ContainsKey(vmType))
                _pageCache[vmType] = page;
        }

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            NavigateTo(typeof(TViewModel));
        }

        public void NavigateTo(Type viewModelType)
        {
            if (_pageCache.TryGetValue(viewModelType, out var page))
                _frame.Content = page;
        }
    }
}
