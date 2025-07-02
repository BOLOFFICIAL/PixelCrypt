using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;
using System.Drawing;

namespace PixelCrypt2025.Model
{
    internal class Cryptography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();
        public Action<bool> UpdateList { get; set; }
        public Action<Image> ShowImage { get; set; }

        public ActionResult SaveData()
        {
            return FileHelper.SaveBitmapToFolder(OutputImage);
        }

        internal async Task<ActionResult> Decrypt(string password)
        {
            var res = await Encryption(password, CryptoService.DecryptPhoto);
            res.ResultTitle = "Расшифрование";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно расшифрованы";

            return res;
        }

        internal async Task<ActionResult> Encrypt(string password)
        {
            var res = await Encryption(password, CryptoService.EncryptPhoto);
            res.ResultTitle = "Шифрование";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно зашифрованы";

            return res;
        }

        private async Task<ActionResult> Encryption(string password, Func<string, string, Task<Bitmap>> action)
        {
            var result = new ActionResult();

            var hashPassword = ProgramHelper.GetHash32(password);

            OutputImage.Clear();
            UpdateList.Invoke(true);

            try
            {
                foreach (var file in InputImage)
                {
                    OutputImage.Add(file, await action(file.Path, hashPassword));

                    var fullUpdate = Context.MainWindowViewModel.CurrentPage.GetType() == typeof(CryptographyPage) && Context.MainWindow.IsActive;

                    if (fullUpdate) ShowImage(file);

                    UpdateList.Invoke(fullUpdate);
                }

                result.IsSuccessResult = true;

                return result;
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                    ResultTitle = "",
                };
            }
        }
    }
}
