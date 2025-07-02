using PixelCrypt2025.Model;
using System.Drawing;

namespace PixelCrypt2025.Interfaces
{
    internal interface IImagePage
    {
        List<Model.Image> InputImage { get; }
        Dictionary<Model.Image, Bitmap> OutputImage { get; }
        ActionResult SaveData();
        Action<bool> UpdateList { get; set; }
        Action<Model.Image> ShowImage { get; set; }
    }
}
