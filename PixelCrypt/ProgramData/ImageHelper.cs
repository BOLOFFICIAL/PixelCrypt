using System.Drawing;
using System.Drawing.Imaging;
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
                    throw new Exception($"Недостаточно места для импорта данных в изображение '{Path.GetFileName(filepath)}'.\nЗамените изображение на другое или используйте несколько изображений и повторите попытку.");
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
                            color = Color.FromArgb(
                                (elementIndex < dataA.Length) ? (byte)(color.A - byte.Parse(dataA[elementIndex].ToString())) : color.A,
                                (elementIndex < dataR.Length) ? (byte)(color.R - byte.Parse(dataR[elementIndex].ToString())) : color.R,
                                (elementIndex < dataG.Length) ? (byte)(color.G - byte.Parse(dataG[elementIndex].ToString())) : color.G,
                                (elementIndex < dataB.Length) ? (byte)(color.B - byte.Parse(dataB[elementIndex].ToString())) : color.B);

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
                    A.Append((listPixels[index].A % 2 == 0) ? "1" : "0");
                    R.Append((listPixels[index].R % 2 == 0) ? "1" : "0");
                    G.Append((listPixels[index].G % 2 == 0) ? "1" : "0");
                    B.Append((listPixels[index].B % 2 == 0) ? "1" : "0");
                }

                var SizeA = Converter.ConvertBinaryStringToInt(A.ToString()); A.Clear();
                var SizeR = Converter.ConvertBinaryStringToInt(R.ToString()); R.Clear();
                var SizeG = Converter.ConvertBinaryStringToInt(G.ToString()); G.Clear();
                var SizeB = Converter.ConvertBinaryStringToInt(B.ToString()); B.Clear();

                for (int i = index; i < index + (slash * SizeA - (slash - 1)); i += slash)
                {
                    A.Append((listPixels[i].A % 2 == 0) ? "1" : "0");
                }

                for (int i = index; i < index + (slash * SizeR - (slash - 1)); i += slash)
                {
                    R.Append((listPixels[i].R % 2 == 0) ? "1" : "0");
                }

                for (int i = index; i < index + (slash * SizeG - (slash - 1)); i += slash)
                {
                    G.Append((listPixels[i].G % 2 == 0) ? "1" : "0");
                }

                for (int i = index; i < index + (slash * SizeB - (slash - 1)); i += 8)
                {
                    B.Append((listPixels[i].B % 2 == 0) ? "1" : "0");
                }

                return A.Append(R.Append(G.Append(B))).ToString();
            });

            return exportDataImage;
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
    }
}
