using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;

namespace PixelCrypt2025.Model
{
    internal class Steganography : IImagePage
    {
        private Func<bool> _saveAction;

        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();
        public Model.File DataFile { get; } = new Model.File();

        public async Task ImportAction(string password)
        {
            var hashPassword = ProgramHelper.GetHash32(password);

            try
            {
                var inportData = DataFile.Content;

                if (DataFile.Path.Length > 0)
                {
                    inportData = Convert.ToBase64String(System.IO.File.ReadAllBytes(DataFile.Path));
                    inportData = CryptoService.EncryptText(inportData, hashPassword);

                    var fileInfo = new FileInfo(DataFile.Path);
                    inportData = $"{fileInfo.Name}[*]{fileInfo.Extension}[*]" + inportData;
                }
                else
                {
                    inportData = CryptoService.EncryptText(inportData, hashPassword);
                }

                string binary = Converter.ConvertTextToBinaryString(inportData);

                OutputImage.Clear();

                var lines = ProgramHelper.SplitStringIntoParts(binary, InputImage.Count);

                for (int i = 0; i < InputImage.Count; i++)
                {
                    OutputImage.Add(InputImage[i], await ImageHelper.ImportDataToImage(lines[i], InputImage[i].Path));
                }

                _saveAction = SaveImport;
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
            }
        }

        public async Task ExportAction(string password)
        {
            var hashPassword = ProgramHelper.GetHash32(password);

            var bynaryData = new List<string>();

            try
            {
                foreach (var filePathImage in InputImage)
                {
                    bynaryData.Add(await ImageHelper.ExportDataFromImage(filePathImage.Path));
                }

                var allData = new StringBuilder();

                foreach (var item in bynaryData)
                {
                    allData.Append(item);
                }

                var exportData = Converter.ConvertBinaryStringToText(allData.ToString());

                var exportFileData = exportData.Split("[*]");

                if (exportFileData.Length > 1)
                {
                    exportFileData[2] = CryptoService.DecryptText(exportFileData[2], hashPassword);

                    if (MessageBox.Show("Экспортированные данные являются файлом.\nСформировать файл?", "Экспорт данных") == MessageBoxResult.OK)
                    {
                        var res = ProgramHelper.SaveDataToFile(exportFileData[0], $"Файлы (*{exportFileData[1]})|*{exportFileData[1]}", Convert.FromBase64String(exportFileData[2]));
                        if (res.Result)
                        {
                            MessageBox.Show($"Фаил {res.FileName} сохранен", "Экспорт данных");

                            DataFile.Content = System.IO.File.ReadAllText(res.FilePath);
                            DataFile.Path = res.FilePath;
                        }
                    }
                    else
                    {
                        DataFile.Content = exportFileData[2];
                    }
                }
                else
                {
                    DataFile.Content = CryptoService.DecryptText(exportData, hashPassword);
                }

                _saveAction = SaveExport;
            }
            catch
            {
                bynaryData.Clear();
            }
        }

        public bool SaveData()
        {
            return _saveAction.Invoke();
        }

        private bool SaveImport()
        {
            var res = ProgramHelper.SaveBitmapToFolder(OutputImage.Select(i => i.Key.Path).ToList(), OutputImage.Select(i => i.Value).ToList());
            if (res.Result)
            {
                MessageBox.Show($"Картинки сохранены в папке {res.FileName}", "Импорт");
            }
            return true;
        }

        private bool SaveExport()
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
