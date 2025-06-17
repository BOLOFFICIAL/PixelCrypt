using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace PixelCrypt2025.ProgramData
{
    internal static class ImageHelper
    {
        private const int rgb = 3;

        public static async Task<Bitmap> ImportDataToImage(string data, string filepath)
        {
            var importDataImage = await Task.Run(() =>
            {
                var pixelIndex = 0;
                var imagePixels = GetArrayPixelsFromImage(filepath);
                var imageWidth = imagePixels.GetLength(0);
                var imageHeight = imagePixels.GetLength(1);
                var modifiedPixels = new Color[imageWidth, imageHeight];
                var totalPixels = imageWidth * imageHeight;
                var binaryLength = Converter.ConvertIntToBinaryString(totalPixels).Length;
                var splitData = ProgramHelper.SplitStringIntoParts(data, rgb);
                var binaryDataList = splitData.Select(el => Converter.ConvertIntToBinaryString(el.Length).PadLeft(binaryLength, '0') + el).ToList();
                var averageDataLength = binaryDataList.Average(el => el.Length);

                if (totalPixels < averageDataLength)
                {
                    throw new Exception($"Недостаточно места для импорта данных в изображение '{Path.GetFileName(filepath)}'.\nЛимит изображения превышен в {(averageDataLength / totalPixels).ToString("F2")}.\nЗамените изображение на другое или используйте несколько изображений и повторите попытку.");
                }

                for (int x = 0; x < imageWidth; x++)
                {
                    for (int y = 0; y < imageHeight; y++, pixelIndex++)
                    {
                        var color = Color.FromArgb(NormalizeColorByte(imagePixels[x, y].R), NormalizeColorByte(imagePixels[x, y].G), NormalizeColorByte(imagePixels[x, y].B));

                        modifiedPixels[x, y] = Color.FromArgb(
                            (pixelIndex < binaryDataList[0].Length) ? (byte)(color.R - byte.Parse(binaryDataList[0][pixelIndex].ToString())) : color.R,
                            (pixelIndex < binaryDataList[1].Length) ? (byte)(color.G - byte.Parse(binaryDataList[1][pixelIndex].ToString())) : color.G,
                            (pixelIndex < binaryDataList[2].Length) ? (byte)(color.B - byte.Parse(binaryDataList[2][pixelIndex].ToString())) : color.B);
                    }
                }

                return modifiedPixels;
            });

            return Converter.ConvertPixelsToBitmap(importDataImage);
        }

        public static async Task<string> ExportDataFromImage(string path)
        {
            var exportDataImage = await Task.Run(() =>
            {
                var imagePixels = GetArrayPixelsFromImage(path);
                var pixelList = GetListPixelsFromArrayPixels(imagePixels);
                var binaryDataBuilders = Enumerable.Range(0, rgb).Select(_ => new StringBuilder()).ToList();
                var binaryLength = Converter.ConvertIntToBinaryString(pixelList.Count).Length;

                for (var i = 0; i < binaryLength; i++)
                {
                    foreach (var data in binaryDataBuilders)
                    {
                        data.Append(GetBinaryColorIndicator(pixelList[i], binaryDataBuilders.IndexOf(data)));
                    }
                }

                foreach (var binaryData in binaryDataBuilders)
                {
                    var dataSize = Converter.ConvertBinaryStringToInt(binaryData.ToString());
                    binaryData.Clear();
                    for (int i = binaryLength; i < binaryLength + dataSize; i++)
                    {
                        binaryData.Append(GetBinaryColorIndicator(pixelList[i], binaryDataBuilders.IndexOf(binaryData)));
                    }
                }

                return string.Concat(binaryDataBuilders);
            });

            return exportDataImage;
        }

        public static Color[,] GetArrayPixelsFromImage(string imagePath)
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

        public static List<Color> GetListPixelsFromArrayPixels(Color[,] pixels)
        {
            var listPixels = new List<Color>();

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    listPixels.Add(Color.FromArgb(pixels[x, y].A, pixels[x, y].R, pixels[x, y].G, pixels[x, y].B));
                }
            }

            return listPixels;
        }

        public static ImageFormat GetImageFormat(string path)
        {
            return Path.GetExtension(path)?.ToLower() switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".tiff" => ImageFormat.Tiff,
                ".ico" => ImageFormat.Icon,
                _ => ImageFormat.Png
            };
        }


        private static byte GetColorByIndex(Color color, int index)
        {
            return (byte)(index == 0 ? color.R : index == 1 ? color.G : index == 2 ? color.B : 0);
        }

        private static string GetBinaryColorIndicator(Color color, int index)
        {
            return (GetColorByIndex(color, index) % 2 == 0) ? "1" : "0";
        }

        private static byte NormalizeColorByte(byte value)
        {
            if (value % 2 != 0) return value;
            return (byte)((value == 0) ? 1 : value - 1);
        }
    }
}
