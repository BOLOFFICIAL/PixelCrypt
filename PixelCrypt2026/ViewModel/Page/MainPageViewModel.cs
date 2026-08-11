using BackgroundUpdater;
using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Program.Notification;
using PixelCrypt2026.Program.Service;
using PixelCrypt2026.ViewModel.Base;
using PixelCrypt2026.ViewModel.UserControl;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.Page
{
    class MainPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly Dictionary<Type, ToolCardViewModel> _tools;
        private Updater _backgroundUpdater;
        private GridLength _updateWidth = new GridLength(0);
        private string _newVersion;

        public ICommand UpdateCommand { get; }

        public ObservableCollection<ToolCardViewModel> Tools { get; }

        public MainPageViewModel(NavigationService navigation)
        {
            _navigation = navigation;

            UpdateCommand = new LambdaCommand(OnUpdate);

            _backgroundUpdater = new Updater("BOLOFFICIAL", "PixelCrypt", Version);
            _backgroundUpdater.UpdateFound += (sender, e) => ShowUpdate(e);
            _backgroundUpdater.Start();

            _tools = new Dictionary<Type, ToolCardViewModel>()
            {
                {
                    typeof(CryptographyPageViewModel),
                    new ToolCardViewModel<CryptographyPageViewModel>()
                    {
                        Title = "Cryptography",
                        Icon = "Solid_Lock",
                        Description = "Protect the image with a password by turning it into visual noise. No one will see the original without the key.",
                        ToolCardCommand = new LambdaCommand(OnNavigate)
                    }
                },
                {
                    typeof(SteganographyPageViewModel),
                    new ToolCardViewModel<SteganographyPageViewModel>()
                    {
                        Title = "Steganography",
                        Icon = "Solid_Eye",
                        Description = "Hide the text or file inside the image unnoticeably. Externally, the picture remains the same.",
                        ToolCardCommand = new LambdaCommand(OnNavigate)
                    }
                }
            };

            Tools = new ObservableCollection<ToolCardViewModel>(_tools.Values);

            OperationStatusService.Instance.StatusChanged += OnSetStatus;
        }

        private void OnUpdate(object obj)
        {
            var res = Notification.Show(
                $"Update to version {_backgroundUpdater.LatestRelease?.Name} now?", "Update available",
                button: Program.Enum.NotificationButtonType.YesNo,
                icon: Program.Enum.NotificationIconType.Question);

            if (res.Result != Program.Enum.NotificationResultType.Yes)
                return;

            string exePath = Environment.ProcessPath
                ?? Process.GetCurrentProcess().MainModule?.FileName
                ?? throw new InvalidOperationException("Cannot determine executable path");

            _backgroundUpdater.StartUpdate(exePath);

            Application.Current.Shutdown();
        }

        private void ShowUpdate(GitHubRelease release)
        {
            UpdateWidth = new GridLength(1, GridUnitType.Auto);
            NewVersion = $"A new version is available: {release.TagName}";
        }

        public string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public string NewVersion
        {
            get => _newVersion;
            set => Set(ref _newVersion, value);
        }

        public GridLength UpdateWidth
        {
            get => _updateWidth;
            set => Set(ref _updateWidth, value);
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
