using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PixelCrypt
{
    public class ImageCrypto
    {
        public static BitmapImage EncryptPhoto(string imagepath, string key)
        {
            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            using (var image = System.Drawing.Image.FromFile(imagepath))
            using (var bitmap = new Bitmap(image))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        bitmap.SetPixel(i, j,EncryptPixel(bitmap.GetPixel(i, j), keybyte[index % keybyte.Length]));
                        index++;
                    }
                }
                return ConvertToBitmapImage(bitmap);
            }
        }

        public static BitmapImage DecryptPhoto(string imagepath, string key)
        {
            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            using (var image = System.Drawing.Image.FromFile(imagepath))
            using (var bitmap = new Bitmap(image))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        bitmap.SetPixel(i, j,  DecryptPixel(bitmap.GetPixel(i, j), keybyte[index % keybyte.Length]));
                        index++;
                    }
                }
                return ConvertToBitmapImage(bitmap);
            }
        }
        
        private static Color EncryptPixel(Color color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
            return Color.FromArgb(EncryptColor(color.R, key), EncryptColor(color.G, key), EncryptColor(color.B, key));
        }

        private static byte EncryptColor(byte color, byte key)
        {
            if (color % 2 != 0)
            {
                color = (byte)(254 - (color ^ key));
            }
            else
            {
                color = (byte)((color ^ key));
            }
            return color;
        }

        private static Color DecryptPixel(Color color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
            return Color.FromArgb(DecryptColor(color.R, key), DecryptColor(color.G, key), DecryptColor(color.B, key));
        }

        private static byte DecryptColor(byte color, byte key)
        {
            if (color % 2 == 0)
            {
                color = (byte)((254 - color) ^ key);
            }
            else
            {
                color = (byte)((color ^ key));
            }
            return color;
        }

        public static BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
