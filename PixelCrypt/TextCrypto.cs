using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace PixelCrypt
{
    public class TextCrypto
    {
        public static async void EncryptPhoto(string message, string imagePath, string key,string name)
        {
            int binaryIndex = 0;
            var binaryMessage = TextToBinary(message);
            binaryMessage = EncryptBinaryText(binaryMessage, key);
            binaryMessage += "0000000000000000";
            using (var image = Image.FromFile(imagePath))
            using (var bitmap = new Bitmap(image))
            {
                Color pixel = new Color();
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        pixel = bitmap.GetPixel(i, j);
                        bitmap.SetPixel(i, j, EncryptPixel(pixel, binaryMessage[binaryIndex]));
                        binaryIndex++;
                        if (binaryIndex == binaryMessage.Length)
                        {
                            string newpath = RenameFile(imagePath, name);
                            bitmap.Save(newpath);
                            MessageBox.Show($"Зашифрованная картинка\n{newpath}\nСохранена");
                            return;
                        }
                    }
                }
            }
            await Task.Delay(1);
        }

        private static string EncryptBinaryText(string binaryText, string key)
        {
            byte[] bytes = SplitBinaryString(binaryText).Select(BinaryToByte).ToArray();
            StringBuilder result = new StringBuilder(bytes.Length * 8);
            for (int i = 0; i < bytes.Length; i++)
            {
                int charCode = bytes[i] ^ (int)key[i % key.Length];
                result.Append(ByteToBinary((byte)charCode));
            }
            return result.ToString();
        }

        public static async Task<string> DecryptPhoto(string imagePath, string key)
        {
            StringBuilder result = new StringBuilder();
            using (var image = Image.FromFile(imagePath))
            using (var bitmap = new Bitmap(image))
            {
                Color pixel = new Color();
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        pixel = bitmap.GetPixel(i, j);
                        result.Append(DecryptPixel(pixel));
                        if (result.Length % 16 == 0 && CheckEnd(result))
                        {
                            var output = result.ToString();
                            output = output.Remove(result.Length - 16);
                            return BinaryToText(EncryptBinaryText(output, key));
                        }
                    }
                }
            }
            await Task.Delay(1);
            return " ";
        }



        private static string TextToBinary(string text)
        {
            return string.Concat(text.Select(c => Convert.ToString(c, 2).PadLeft(16, '0')));
        }

        private static string ByteToBinary(byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        private static byte BinaryToByte(string binary)
        {
            return Convert.ToByte(binary, 2);
        }

        private static string BinaryToText(string binary)
        {
            return string.Concat(Enumerable.Range(0, binary.Length / 16)
                .Select(i => (char)Convert.ToInt32(binary.Substring(i * 16, 16), 2)));
        }


        private static Color EncryptPixel(Color pixel, char binaryValue)
        {
            int red = pixel.R;
            int green = pixel.G;
            if (red == 255)
            {
                red--;
            }
            if (green == 255)
            {
                green--;
            }
            if ((red + green) % 2 == 0)
            {
                green++;
            }
            green -= int.Parse(binaryValue.ToString());
            return Color.FromArgb(red, green, pixel.B);
        }

        private static string DecryptPixel(Color colorPixel)
        {
            return ((colorPixel.R + colorPixel.G) % 2 == 0) ? ("1") : ("0");
        }


        private static bool CheckEnd(StringBuilder stringBuilder)
        {
            if (stringBuilder.Length >= 16 && stringBuilder.Length % 16 == 0)
            {
                return stringBuilder.ToString().Substring(stringBuilder.ToString().Length - 16).Equals("0000000000000000");
            }
            return false;
        }

        private static string[] SplitBinaryString(string binaryString)
        {
            int binaryStringLength = binaryString.Length;
            int arrayLength = (binaryStringLength + 7) / 8;
            string[] result = new string[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                int startIndex = i * 8;
                int substringLength = Math.Min(8, binaryStringLength - startIndex);
                result[i] = binaryString.Substring(startIndex, substringLength).PadLeft(8, '0');
            }
            return result;
        }

        public static string RenameFile(string filePath,string name)
        {
            string directory = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);
            string newFilePath = Path.Combine(directory, name + extension);
            return newFilePath;
        }
    }
}
