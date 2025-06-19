using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page.MainWindow;
using System.Drawing;

namespace PixelCrypt2025.Model
{
    internal class Cryptography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();
        public Func<Task> UpdateList { get; set; }
        public Action<Image> ShowImage { get; set; }

        public ActionResult SaveData()
        {
            return ProgramHelper.SaveBitmapToFolder(OutputImage);
        }

        internal async Task<ActionResult> Decrypt(string password)
        {
            var res = await Encryption(password, CryptoService.DecryptPhoto);
            res.ResultTitle = "Расшифровывание данных";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно расшифрованы";

            return res;
        }

        internal async Task<ActionResult> Encrypt(string password)
        {
            var res = await Encryption(password, CryptoService.EncryptPhoto);
            res.ResultTitle = "Зашифровывание данных";

            if (res.IsSuccessResult)
                res.ResultMessage = "Данные успешно зашифрованы";

            return res;
        }

        private async Task<ActionResult> Encryption(string password, Func<string, string, Task<Bitmap>> action)
        {
            var result = new ActionResult();

            var hashPassword = ProgramHelper.GetHash32(password);

            OutputImage.Clear();
            await UpdateList.Invoke();

            try
            {
                foreach (var file in InputImage)
                {
                    OutputImage.Add(file, await action(file.Path, hashPassword));

                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(CryptographyPage) && Context.MainWindow.IsActive)
                    {
                        ShowImage(file);
                        await UpdateList.Invoke();
                    }
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
                    ResultMessage = $"Ошибка: {ex.Message}",
                    ResultTitle = "",
                };
            }
        }
    }
}
