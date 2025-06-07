using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelCrypt2025.ProgramData
{
    internal static class Converter
    {
        public static Bitmap ConvertPixelsToBitmap(List<System.Drawing.Color> pixels, int width, int height)
        {
            var newPixels = new System.Drawing.Color[width, height];
            int index = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newPixels[x, y] = System.Drawing.Color.FromArgb(pixels[index].A, pixels[index].R, pixels[index].G, pixels[index].B);
                    index++;
                }
            }

            return ConvertPixelsToBitmap(newPixels);
        }

        public static Bitmap ConvertPixelsToBitmap(System.Drawing.Color[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bitmap.SetPixel(x, y, pixels[x, y]);
                }
            }

            return bitmap;
        }

        public static string ConvertTextToBinaryString(string input)
        {
            var text = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            StringBuilder binary = new StringBuilder();
            foreach (char c in text)
            {
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return binary.ToString();
        }

        public static string ConvertBinaryStringToText(string binary)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string byteString = binary.Substring(i, 8);
                text.Append((char)Convert.ToByte(byteString, 2));
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(text.ToString()));
        }

        public static string ConvertIntToBinaryString(int number)
        {
            return Convert.ToString(number, 2);
        }

        public static int ConvertBinaryStringToInt(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }

        public static Bitmap ConvertImageToBitmap(System.Windows.Controls.Image wpfImage)
        {
            var bitmapSource = wpfImage.Source as BitmapSource;

            using (var memoryStream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                encoder.Save(memoryStream);

                using (var bmp = new Bitmap(memoryStream))
                {
                    return new Bitmap(bmp);
                }
            }
        }

        public static ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
