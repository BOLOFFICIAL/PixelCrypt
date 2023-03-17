using System;
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
            BitmapImage originalImage = new BitmapImage(new Uri(imagepath));

            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            int width = originalImage.PixelWidth;
            int height = originalImage.PixelHeight;

            int stride = width * 4;
            byte[] pixelData = new byte[stride * height];
            byte[] tmp = new byte[stride * height];
            originalImage.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 0] = EncryptPixel(pixelData[i + 0], keybyte[index % keybyte.Length]);
                pixelData[i + 1] = EncryptPixel(pixelData[i + 1], keybyte[index % keybyte.Length]);
                pixelData[i + 2] = EncryptPixel(pixelData[i + 2], keybyte[index % keybyte.Length]);
                index++;
            }

            pixelData = ShiftByteArray(pixelData, key.Length);

            BitmapSource invertedBitmap = BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, pixelData, stride);

            BitmapImage invertedImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(invertedBitmap));
                encoder.Save(memory);
                memory.Position = 0;
                invertedImage.BeginInit();
                invertedImage.StreamSource = memory;
                invertedImage.CacheOption = BitmapCacheOption.OnLoad;
                invertedImage.EndInit();
            }

            return invertedImage;
        }

        public static BitmapImage DecryptPhoto(string imagepath, string key)
        {
            BitmapImage originalImage = new BitmapImage(new Uri(imagepath));

            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            int width = originalImage.PixelWidth;
            int height = originalImage.PixelHeight;

            int stride = width * 4; // 4 bytes per pixel
            byte[] pixelData = new byte[stride * height];
            
            originalImage.CopyPixels(pixelData, stride, 0);

            pixelData = ShiftByteArray(pixelData, key.Length);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 0] = DecryptPixel(pixelData[i + 0], keybyte[index % keybyte.Length]);
                pixelData[i + 1] = DecryptPixel(pixelData[i + 1], keybyte[index % keybyte.Length]);
                pixelData[i + 2] = DecryptPixel(pixelData[i + 2], keybyte[index % keybyte.Length]);
                index++;
            }

            BitmapSource invertedBitmap = BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, pixelData, stride);

            BitmapImage invertedImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(invertedBitmap));
                encoder.Save(memory);
                memory.Position = 0;
                invertedImage.BeginInit();
                invertedImage.StreamSource = memory;
                invertedImage.CacheOption = BitmapCacheOption.OnLoad;
                invertedImage.EndInit();
            }

            return invertedImage;
        }

        public static byte[] ShiftByteArray(byte[] byteArray, int shift)
        {
            byte[] shiftedArray = new byte[byteArray.Length];

            for (int i = 0; i < byteArray.Length; i++)
            {
                int newIndex = i + shift;
                if (newIndex >= byteArray.Length)
                {
                    newIndex -= byteArray.Length;
                }
                shiftedArray[newIndex] = byteArray[i];
            }

            return shiftedArray;
        }

        public static string GetUnicodeBinary(string str)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in str)
            {
                result.Append(Convert.ToString(c, 2).PadLeft(16, '0'));
            }
            return result.ToString();
        }

        private static byte EncryptPixel(byte color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
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

        private static byte DecryptPixel(byte color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
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
    }
}
