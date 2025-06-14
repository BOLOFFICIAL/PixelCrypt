using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using System.Drawing;
using System.Windows;

namespace PixelCrypt2025.Model
{
    internal class Steganography : IImagePage
    {
        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();
        public Model.File DataFile { get; } = new Model.File();

        public bool SaveImport()
        {
            var res = ProgramHelper.SaveBitmapToFolder(OutputImage);
            if (res.Result)
            {
                MessageBox.Show($"Картинки сохранены в папке {res.FileName}", "Импорт");
            }
            return true;
        }

        public bool SaveExport()
        {
            if (DataFile.Content.Length == 0)
            {
                MessageBox.Show("Нет данных для сохранения", "Экспорт");
            }
            else
            {
                var res = ProgramHelper.SaveDataToFile($"PixelCrypt_{DateTime.Now:yyyyMMddHHmmss}", $"Файлы (*.txt)|*.txt", DataFile.Content);
                if (res.Result)
                {
                    MessageBox.Show($"Файл {res.FileName} сохранен", "Экспорт");
                }
            }

            return true;
        }
    }
}
