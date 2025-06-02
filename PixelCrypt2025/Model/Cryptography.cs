using PixelCrypt2025.Interfaces;

namespace PixelCrypt2025.Model
{
    class Cryptography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();

        public void AddElement(string path)
        {
            foreach (var image in InputImage)
            {
                if (image.Path == path) return;
            }

            InputImage.Add(new Image(path));
        }

        public void RemoveElement(Image image)
        {
            InputImage.Remove(image);
        }

        public void EncryptAction()
        {

        }

        public void DecryptAction()
        {

        }
    }
}
