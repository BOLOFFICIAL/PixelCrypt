using PixelCrypt2026.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ProgressPanelViewModel : BaseViewModel
    {
        private string _progressTime = "00:00:00";
        private string _progressPercent = "0%";
        private double _progressValue;

        public string ProgressTime
        {
            get => _progressTime;
            set => Set(ref _progressTime, value);
        }

        public string ProgressPercent
        {
            get => _progressPercent;
            private set => Set(ref _progressPercent, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (Set(ref _progressValue, value))
                {
                    ProgressPercent = $"{value:F1}%";
                }
            }
        }

        public ProgressPanelViewModel()
        {
            ProgressValue = 0;
            ProgressTime = "--:--:--";
        }
    }
}
