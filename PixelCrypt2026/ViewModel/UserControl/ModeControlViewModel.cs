using PixelCrypt2026.Commands.Base;
using PixelCrypt2026.Model;
using PixelCrypt2026.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ModeControlViewModel : BaseViewModel
    {
        public ObservableCollection<UIButton> Buttons { get; set; } = new ObservableCollection<UIButton>();

        private int _selectedMode;

        public int SelectedMode
        {
            get => _selectedMode;
            private set
            {
                _selectedMode = value;
                UpdateButtonsState();
            }
        }
        
        public ModeControlViewModel(List<string> modes) 
        {
            var mds = new List<(string title, Action action)>();

            foreach (var mode in modes) 
            {
                mds.Add((mode, null));
            }

            InitializeModes(mds);
        }

        public ModeControlViewModel(List<(string title, Action action)> modes)
        {
            InitializeModes(modes);
        }

        private void InitializeModes(List<(string title, Action action)> modes) 
        {
            int index = 0;

            foreach (var mode in modes)
            {
                int currentIndex = index;
                Buttons.Add(new UIButton()
                {
                    Text = mode.title,
                    Command = new LambdaCommand((object obj) =>
                    {
                        SelectedMode = currentIndex;
                        mode.action?.Invoke();
                    }),
                    Foreground = (Application.Current.TryFindResource("Foreground") as SolidColorBrush)?.Color.ToString(),
                    Background = "#00000000",
                });

                index++;
            }

            if (Buttons.Count > 0)
                SelectedMode = 0;
        }

        private void UpdateButtonsState()
        {
            string activeForeground = (Application.Current.TryFindResource("Foreground") as SolidColorBrush)?.Color.ToString();
            string activeBackground = "#00000000";
            string selectedForeground = (Application.Current.TryFindResource("ButtonForeground") as SolidColorBrush)?.Color.ToString();
            string selectedBackground = (Application.Current.TryFindResource("Accent") as SolidColorBrush)?.Color.ToString();

            for (int i = 0; i < Buttons.Count; i++)
            {
                if (i == SelectedMode)
                {
                    Buttons[i].Foreground = selectedForeground;
                    Buttons[i].Background = selectedBackground;
                }
                else
                {
                    Buttons[i].Foreground = activeForeground;
                    Buttons[i].Background = activeBackground;
                }
            }
        }
    }
}