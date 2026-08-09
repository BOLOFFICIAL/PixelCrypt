using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace PixelCrypt2026.Program
{
    class ProgramHelper
    {
        public static string GetHash32(string input)
        {
            string output;
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return output = Convert.ToHexString(hash);
        }

        public static string GetSha256(Image image)
        {
            if (image == null) return null;

            int width = image.Width;
            int height = image.Height;
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            return GetSha256(bitmap);
        }

        public static string GetSha256(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = 4;
            int rowSize = checked(width * bytesPerPixel);

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var converted = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(bitmap, new Rectangle(0, 0, width, height));
                }

                return GetSha256(converted);
            }

            var rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                byte[] data = new byte[checked(8 + rowSize * height)];

                Buffer.BlockCopy(BitConverter.GetBytes(width), 0, data, 0, 4);

                Buffer.BlockCopy(BitConverter.GetBytes(height), 0, data, 4, 4);

                for (int y = 0; y < height; y++)
                {
                    IntPtr rowAddress = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                    Marshal.Copy(rowAddress, data, 8 + y * rowSize, rowSize);
                }

                byte[] hash = SHA256.HashData(data);
                return Convert.ToHexString(hash);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public static List<string> SplitString(string str, int partsCount)
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

        public static List<string> SplitString(string str, List<int> partsCount)
        {
            var result = new List<string>(partsCount.Count);
            int start = 0;

            foreach (int length in partsCount)
            {
                if (start + length > str.Length) return null;

                result.Add(str.Substring(start, length));
                start += length;
            }

            return result;
        }

        public static List<int> DistributeData(List<int> capacities, int data)
        {
            var result = new List<int>(new int[capacities.Count]);

            while (data > 0)
            {
                bool anyFilled = false;

                for (int i = 0; i < capacities.Count; i++)
                {
                    if (result[i] < capacities[i])
                    {
                        result[i]++;
                        data--;
                        anyFilled = true;

                        if (data == 0) return result;
                    }
                }

                if (!anyFilled) return null;
            }

            return result;
        }

        public static void CopyText(string text)
        {
            Clipboard.SetText(text ?? string.Empty);
        }

        public static void CopyFileToClipboard(List<string> filePaths)
        {
            if (filePaths == null || filePaths.Count == 0) return;

            var pathsCollection = new System.Collections.Specialized.StringCollection();
            pathsCollection.AddRange(filePaths.ToArray());
            Clipboard.SetFileDropList(pathsCollection);
        }

        public static void CleanupTempFiles()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "PixelCrypt");

            if (!Directory.Exists(tempDir)) return;

            try
            {
                var files = Directory.GetFiles(tempDir, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try { File.Delete(file); }
                    catch (Exception) { continue; }
                }
                Directory.Delete(tempDir, true);
            }
            catch (Exception ex) { }
        }

    }
}
