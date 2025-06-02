using PixelCrypt2025.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelCrypt2025.Model
{
    class Cryptography : IImagePage
    {
        public List<Model.Image> ContextImage { get; } = new List<Model.Image>();

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
