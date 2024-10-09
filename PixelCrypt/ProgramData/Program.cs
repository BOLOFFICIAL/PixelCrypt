using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}