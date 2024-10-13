using System.Drawing;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Imaging;

namespace PixelCrypt.ProgramData
{
    public class Program
    {
        public static Color[,] GetPixelsFromImage(string imagePath)
        {
            if (!File.Exists(imagePath)) return new Color[0, 0];

            try
            {
                using (var bitmap = new Bitmap(imagePath))
                {
                    var width = bitmap.Width;
                    var height = bitmap.Height;
                    var pixels = new Color[width, height];

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            pixels[x, y] = bitmap.GetPixel(x, y);
                        }
                    }

                    return pixels;
                }
            }
            catch (Exception ex)
            {
                return new Color[0, 0];
            }
        }

        public static Bitmap CreateImageFromPixels(Color[,] pixels)
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

        public static string TextToBinary(string input)
        {
            var text = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            StringBuilder binary = new StringBuilder();
            foreach (char c in text)
            {
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return binary.ToString();
        }

        public static string BinaryToText(string binary)
        { 
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string byteString = binary.Substring(i, 8);
                text.Append((char)Convert.ToByte(byteString, 2));
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(text.ToString()));
        }

        public static string ToBinary(int number)
        {
            return Convert.ToString(number, 2);
        }

        public static int FromBinary(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }

        public static List<string> SplitStringIntoParts(string str, int partsCount)
        {
            List<string> result = new List<string>();

            int partLength = str.Length / partsCount;
            int remainder = str.Length % partsCount;

            int currentPosition = 0;

            for (int i = 0; i < partsCount; i++)
            {
                int currentPartLength = partLength + (remainder > 0 ? 1 : 0);
                remainder--;

                result.Add(str.Substring(currentPosition, currentPartLength));
                currentPosition += currentPartLength;
            }

            return result;
        }

        public static string Hash32(string input)
        {
            string output; 
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input); 
            byte[] hash = MD5Hash.ComputeHash(inputBytes); 
            return output = Convert.ToHexString(hash); 
        }

        public static string Encrypt(string plainText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        public static Bitmap ConvertToBitmap(System.Windows.Controls.Image wpfImage)
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
    }
}