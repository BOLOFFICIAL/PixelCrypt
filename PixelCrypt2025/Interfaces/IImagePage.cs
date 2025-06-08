using System.Drawing;

namespace PixelCrypt2025.Interfaces
{
    internal interface IImagePage
    {
        List<Model.Image> InputImage { get; }
        Dictionary<Model.Image, Bitmap> OutputImage { get; }
        bool SaveData();
    }
}
