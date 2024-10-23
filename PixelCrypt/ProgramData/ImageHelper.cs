using System.Drawing;
using System.IO;
using System.Text;

namespace PixelCrypt.ProgramData
{
    internal class ImageHelper
    {
        private static readonly int slash = 8;

        public static async Task<Bitmap> ImportDataToImage(string data, string filepath)
        {
            var importDataImage = await Task.Run(() =>
            {
                var pixels = GetArrayPixelsFromImage(filepath);

                var listPixels = new List<Color>();

                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var color = Color.FromArgb(
                            NormalizeColorByte(pixels[x, y].A),
                            NormalizeColorByte(pixels[x, y].R),
                            NormalizeColorByte(pixels[x, y].G),
                            NormalizeColorByte(pixels[x, y].B));

                        listPixels.Add(color);
                    }
                }

                var uniqBinaryLength = Converter.ConvertIntToBinaryString(listPixels.Count).Length;

                var spl = Program.SplitStringIntoParts(data, 4);

                var l0 = Converter.ConvertIntToBinaryString(spl[0].Length).PadLeft(uniqBinaryLength, '0');
                var l1 = Converter.ConvertIntToBinaryString(spl[1].Length).PadLeft(uniqBinaryLength, '0');
                var l2 = Converter.ConvertIntToBinaryString(spl[2].Length).PadLeft(uniqBinaryLength, '0');
                var l3 = Converter.ConvertIntToBinaryString(spl[3].Length).PadLeft(uniqBinaryLength, '0');

                var dataA = l0 + spl[0];
                var dataR = l1 + spl[1];
                var dataG = l2 + spl[2];
                var dataB = l3 + spl[3];

                var limit = listPixels.Count / slash;

                var avrage = (dataA.Length + dataR.Length + dataG.Length + dataB.Length) / 4;

                if (limit < avrage)
                {
                    throw new Exception($"A lot of data");
                }

                var newPixels = new Color[width, height];

                int index = 0;
                int elementIndex = 0;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var color = Color.FromArgb(listPixels[index].A, listPixels[index].R, listPixels[index].G, listPixels[index].B);

                        if (index % slash == 0)
                        {
                            var a = (elementIndex < dataA.Length) ? (byte)(color.A - byte.Parse(dataA[elementIndex].ToString())) : color.A;
                            var r = (elementIndex < dataR.Length) ? (byte)(color.R - byte.Parse(dataR[elementIndex].ToString())) : color.R;
                            var g = (elementIndex < dataG.Length) ? (byte)(color.G - byte.Parse(dataG[elementIndex].ToString())) : color.G;
                            var b = (elementIndex < dataB.Length) ? (byte)(color.B - byte.Parse(dataB[elementIndex].ToString())) : color.B;

                            color = Color.FromArgb(a, r, g, b);

                            elementIndex++;
                        }

                        newPixels[x, y] = color;
                        index++;
                    }
                }
                return newPixels;
            });

            return Converter.ConvertPixelsToBitmap(importDataImage);
        }

        public static async Task<string> ExportDataFromImage(string path)
        {
            try
            {
                var exportDataImage = await Task.Run(() =>
                {
                    var pixels = GetArrayPixelsFromImage(path);

                    var listPixels = GetListPixelsFromArrayPixels(pixels);

                    var uniqBinaryLength = Converter.ConvertIntToBinaryString(listPixels.Count).Length;

                    var A = new StringBuilder();
                    var R = new StringBuilder();
                    var G = new StringBuilder();
                    var B = new StringBuilder();

                    var index = 0;

                    for (index = 0; index < slash * uniqBinaryLength - (slash - 1); index += slash)
                    {
                        var currentPixel = listPixels[index];
                        A.Append((currentPixel.A % 2 == 0) ? "1" : "0");
                        R.Append((currentPixel.R % 2 == 0) ? "1" : "0");
                        G.Append((currentPixel.G % 2 == 0) ? "1" : "0");
                        B.Append((currentPixel.B % 2 == 0) ? "1" : "0");
                    }

                    var dataA = new string(Enumerable.Range(0, Converter.ConvertBinaryStringToInt(A.ToString())).Select(i => (listPixels[index + i * slash].A & 1) == 0 ? '1' : '0').ToArray());
                    var dataR = new string(Enumerable.Range(0, Converter.ConvertBinaryStringToInt(R.ToString())).Select(i => (listPixels[index + i * slash].R & 1) == 0 ? '1' : '0').ToArray());
                    var dataG = new string(Enumerable.Range(0, Converter.ConvertBinaryStringToInt(G.ToString())).Select(i => (listPixels[index + i * slash].G & 1) == 0 ? '1' : '0').ToArray());
                    var dataB = new string(Enumerable.Range(0, Converter.ConvertBinaryStringToInt(B.ToString())).Select(i => (listPixels[index + i * slash].B & 1) == 0 ? '1' : '0').ToArray());

                    return dataA + dataR + dataG + dataB;
                });

                return exportDataImage;
            }
            catch
            {
                return "";
            }
        }

        private static byte NormalizeColorByte(byte value)
        {
            var res = value;
            if (res % 2 != 0)
            {
                return res;
            }
            else
            {
                if (res == 0)
                {
                    return 1;
                }
                else
                {
                    return (byte)(res - 1);
                }
            }
        }

        public static Color[,] GetArrayPixelsFromImage(string imagePath)
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
    }
}
