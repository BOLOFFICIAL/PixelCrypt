using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace PixelCrypt2026.Model
{
    public class ImageFile
    {
        public string FilePath { get; }

        public string FileName { get; }

        public long FileSize { get; }

        public string FileSizeFormatted
        {
            get
            {
                if (FileSize < 1024)
                    return $"{FileSize} B";
                else if (FileSize < 1024 * 1024)
                    return $"{FileSize / 1024.0:F1} KB";
                else if (FileSize < 1024 * 1024 * 1024)
                    return $"{FileSize / (1024.0 * 1024.0):F1} MB";
                else
                    return $"{FileSize / (1024.0 * 1024.0 * 1024.0):F2} GB";
            }
        }

        public int ImageWidth { get; }
        public int ImageHeight { get; }

        public string ImageResolution => $"{ImageWidth} x {ImageHeight} px";

        public Bitmap ResultImage { get; internal set; }
        public ImageSource ResultImageSource { get; internal set; }

        public ImageFile(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            FilePath = filePath;

            var fileInfo = new FileInfo(filePath);
            FileSize = fileInfo.Length;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(
                        stream,
                        System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation,
                        System.Windows.Media.Imaging.BitmapCacheOption.None);

                    if (decoder.Frames.Count > 0)
                    {
                        var frame = decoder.Frames[0];
                        ImageWidth = frame.PixelWidth;
                        ImageHeight = frame.PixelHeight;
                    }
                }
            }
            catch
            {
                ImageWidth = 0;
                ImageHeight = 0;
            }
        }
    }
}
