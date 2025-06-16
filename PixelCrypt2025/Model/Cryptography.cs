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
        public Func<Task> UpdateList { get; set; }
        public Action<Image> ShowImage { get; set; }

        public bool SaveData()
        {
            var title = "Сохранение изображений";
            try
            {
                var res = ProgramHelper.SaveBitmapToFolder(OutputImage);

                if (res.Result)
                    MessageBox.Show($"Картинки сохранены в папке {res.FileName}", title);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> Decrypt(string password)
        {
            return await Encryption(password, CryptoService.DecryptPhoto);
        }

        internal async Task<bool> Encrypt(string password)
        {
            return await Encryption(password, CryptoService.EncryptPhoto);
        }

        private async Task<bool> Encryption(string password, Func<string, string, Task<Bitmap>> action)
        {
            var hashPassword = ProgramHelper.GetHash32(password);

            OutputImage.Clear();
            await UpdateList.Invoke();

            try
            {
                foreach (var file in InputImage)
                {
                    OutputImage.Add(file, await action(file.Path, hashPassword));
                    ShowImage(file);
                    await UpdateList.Invoke();
                }
                return true;
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
                return false;
            }
        }
    }
}
