using PixelCrypt2025.Interfaces;

namespace PixelCrypt2025.Model
{
    internal class Cryptography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image,Model.ResultImage> OutputImage { get; } = new Dictionary<Model.Image, Model.ResultImage>();

        public void EncryptAction()
        {

        }

        public void DecryptAction()
        {

        }
    }
}
