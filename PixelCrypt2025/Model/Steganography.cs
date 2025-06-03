using PixelCrypt2025.Interfaces;

namespace PixelCrypt2025.Model
{
    internal class Steganography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Model.ResultImage> OutputImage { get; } = new Dictionary<Model.Image, Model.ResultImage>();
        public Model.File InputFile { get; } = new Model.File();

        public void ImportAction()
        {

        }

        public void ExportAction()
        {

        }
    }
}
