using System.IO;

namespace PixelCrypt2025.Model
{
    internal class Image
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public long Length { get; private set; }
        public string Extension { get; private set; }
        public string Permission => $"{Width} x {Height}";
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Image(string filepath)
        {
            Path = filepath;
            GetImageFileInfo(filepath);
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (obj is not Image image)
                return false;
            return image.Path == Path;
        }

        public void GetImageFileInfo(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath)) return;

            FileInfo fileInfo = new FileInfo(imagePath);

            Name = fileInfo.Name;
            Length = fileInfo.Length / 1024;
            Extension = fileInfo.Extension;

            try
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(imagePath))
                {
                    Width = img.Width;
                    Height = img.Height;
                }
            }
            catch { }
            finally { }
        }
    }
}
