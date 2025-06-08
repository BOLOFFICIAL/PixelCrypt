using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using System.Drawing;
using System.Windows;

namespace PixelCrypt2025.Model
{
    internal class Cryptography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();

        public async Task EncryptAction(string password)
        {
            var hashPassword = ProgramHelper.GetHash32(password);

            OutputImage.Clear();

            try
            {
                foreach (var file in InputImage)
                {
                    OutputImage.Add(file, await CryptoService.EncryptPhoto(file.Path, hashPassword));
                }
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
            }
        }

        public async Task DecryptAction(string password)
        {
            var hashPassword = ProgramHelper.GetHash32(password);

            OutputImage.Clear();

            try
            {
                foreach (var file in InputImage)
                {
                    OutputImage.Add(file, await CryptoService.DecryptPhoto(file.Path, hashPassword));
                }
            }
            catch
            {
                OutputImage.Clear();
            }
        }

        public bool SaveData()
        {
            var title = "Сохранение изображений";
            try
            {
                var res = ProgramHelper.SaveBitmapToFolder(OutputImage);
                if (res.Result)
                {
                    MessageBox.Show($"Картинки сохранены в папке {res.FileName}", title);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
