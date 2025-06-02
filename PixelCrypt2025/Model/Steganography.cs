using PixelCrypt2025.Interfaces;

namespace PixelCrypt2025.Model
{
    internal class Steganography<T> : IImagePage<T>
    {
        public List<Model.Image> ContextImage { get; } = new List<Model.Image>();

        public T InputData { get; }

        public Steganography(T inputData)
        {
            InputData = inputData;
        }

        public void AddElement(string path)
        {
            foreach (var image in ContextImage)
            {
                if (image.Path == path) return;
            }

            ContextImage.Add(new Image(path));
        }

        public void RemoveElement(Image image)
        {
            ContextImage.Remove(image);
        }

        public void EncryptAction()
        {

        }

        public void DecryptAction()
        {

        }
    }
}
