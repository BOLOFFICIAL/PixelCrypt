using System.IO;

namespace PixelCrypt2026.Model
{
    public class ImageFile
    {
        public string ImagePath { get; }

        public string FileName { get; }

        public ImageFile(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            ImagePath = filePath;
        }
    }
}
