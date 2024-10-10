using System.Drawing;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
    }
}