using PixelCrypt2026.Commands.Base;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ImageChipViewModel
    {
        public string ImagePath { get; }

        public string FileName { get; }

        public ImageChipViewModel(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            ImagePath = filePath;
        }
    }
}
