using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PixelCrypt2026.Program
{
    internal static class ImageHelper
    {
        private const int Channels = 3;
        private const int BytesPerPixel = 4;

        private static readonly int[] ChannelOffsets = { 2, 1, 0 };

        public static async Task<Bitmap> ImportDataToImage(string data, string filePath)
        {
            var importDataImage = await Task.Run(() =>
            {
                var (pixels, imageWidth, imageHeight) = ReadPixels(filePath);
                int totalPixels = imageWidth * imageHeight;
                int binaryLength = Converter.ConvertIntToBinaryString(totalPixels).Length;

                var splitData = ProgramHelper.SplitString(data, Channels);
                var binaryDataList = splitData.Select(el => Converter.ConvertIntToBinaryString(el.Length).PadLeft(binaryLength, '0') + el).ToList();

                for (int i = 0; i < pixels.Length; i += BytesPerPixel)
                {
                    pixels[i] = NormalizeColorByte(pixels[i]);
                    pixels[i + 1] = NormalizeColorByte(pixels[i + 1]);
                    pixels[i + 2] = NormalizeColorByte(pixels[i + 2]);
                    pixels[i + 3] = 255;
                }

                for (int pixelIndex = 0; pixelIndex < totalPixels; pixelIndex++)
                {
                    int offset = pixelIndex * BytesPerPixel;

                    for (int channel = 0; channel < Channels; channel++)
                    {
                        var binaryData = binaryDataList[channel];

                        if (pixelIndex < binaryData.Length)
                        {
                            int channelOffset = offset + ChannelOffsets[channel];
                            pixels[channelOffset] = (byte)(pixels[channelOffset] - (binaryData[pixelIndex] - '0'));
                        }
                    }
                }

                return WriteBitmap(pixels, imageWidth, imageHeight);
            });

            return importDataImage;
        }

        public static async Task<string> ExportDataFromImage(string path)
        {
            var exportDataImage = await Task.Run(() =>
            {
                var (pixels, width, height) = ReadPixels(path);
                int totalPixels = width * height;
                var binaryDataBuilders = Enumerable.Range(0, Channels).Select(_ => new StringBuilder()).ToList();
                int binaryLength = Converter.ConvertIntToBinaryString(totalPixels).Length;

                for (int i = 0; i < binaryLength; i++)
                {
                    for (int channel = 0; channel < Channels; channel++)
                    {
                        binaryDataBuilders[channel].Append(GetBinaryColorIndicator(pixels, i, channel));
                    }
                }

                for (int channel = 0; channel < Channels; channel++)
                {
                    var binaryData = binaryDataBuilders[channel];
                    int dataSize = Converter.ConvertBinaryStringToInt(binaryData.ToString());
                    binaryData.Clear();

                    for (int i = binaryLength; i < binaryLength + dataSize; i++)
                    {
                        binaryData.Append(GetBinaryColorIndicator(pixels, i, channel));
                    }
                }

                return string.Concat(binaryDataBuilders);
            });

            return exportDataImage;
        }

        public static (byte[] Pixels, int Width, int Height) ReadPixels(string imagePath)
        {
            using var source = new Bitmap(imagePath);

            if (source.PixelFormat == PixelFormat.Format32bppArgb)
                return ReadLocked(source);

            using var converted = (Bitmap)source.Clone(new Rectangle(0, 0, source.Width, source.Height), PixelFormat.Format32bppArgb);
            return ReadLocked(converted);
        }

        public static Bitmap WriteBitmap(byte[] pixels, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                int stride = data.Stride;
                int rowBytes = width * BytesPerPixel;

                if (stride == rowBytes)
                {
                    Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
                }
                else
                {
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(pixels, y * rowBytes, IntPtr.Add(data.Scan0, y * stride), rowBytes);
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
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

        private static (byte[] Pixels, int Width, int Height) ReadLocked(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            var rect = new Rectangle(0, 0, width, height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int stride = data.Stride;
                int rowBytes = width * BytesPerPixel;
                byte[] pixels = new byte[rowBytes * height];

                if (stride == rowBytes)
                {
                    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        int sourceOffset = stride < 0 ? (height - 1 - y) * stride : y * stride;
                        Marshal.Copy(IntPtr.Add(data.Scan0, sourceOffset), pixels, y * rowBytes, rowBytes);
                    }
                }

                return (pixels, width, height);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        private static string GetBinaryColorIndicator(byte[] pixels, int pixelIndex, int channel)
            => (pixels[pixelIndex * BytesPerPixel + ChannelOffsets[channel]] % 2 == 0) ? "1" : "0";

        private static byte NormalizeColorByte(byte value)
        {
            if (value % 2 != 0) return value;
            return (byte)((value == 0) ? 1 : value - 1);
        }
    }
}
