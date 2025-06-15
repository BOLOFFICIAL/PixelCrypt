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
        public Func<Task> UpdateList { get; set; }
        public Action<Model.Image> ShowImage { get; set; }

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

        internal async Task<bool> Import(string password)
        {
            try
            {
                _saveAction = SaveImport;

                var hashPassword = ProgramHelper.GetHash32(password);

                var inportData = DataFile.Content;

                if (inportData.Length == 0)
                {
                    return false;
                }

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
                await UpdateList.Invoke();

                var lines = ProgramHelper.SplitStringIntoParts(binary, InputImage.Count);

                for (int i = 0; i < InputImage.Count; i++)
                {
                    var bitmapResult = await ImageHelper.ImportDataToImage(lines[i], InputImage[i].Path);
                    OutputImage.Add(InputImage[i], bitmapResult);
                    ShowImage(InputImage[i]);
                    await UpdateList.Invoke();
                }

                return true;
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
                UpdateList.Invoke();
                return false;
            }
        }

        internal async Task<bool> Export(string password)
        {
            //InputData = "";
            //InputFilePath = "";
            _saveAction = SaveExport;

            var hashPassword = ProgramHelper.GetHash32(password);

            var bynaryData = new List<string>();
            OutputImage.Clear();
            await UpdateList.Invoke();

            try
            {
                foreach (var filePathImage in InputImage)
                {
                    bynaryData.Add(await ImageHelper.ExportDataFromImage(filePathImage.Path));
                    OutputImage.Add(filePathImage, null);
                    ShowImage(filePathImage);
                    await UpdateList.Invoke();
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

                    if (MessageBox.Show("Экспортированные данные являются файлом.\nСформировать файл?", "Экспорт данных", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var res = ProgramHelper.SaveDataToFile(exportFileData[0], $"Файлы (*{exportFileData[1]})|*{exportFileData[1]}", Convert.FromBase64String(exportFileData[2]));
                        if (res.Result)
                        {
                            MessageBox.Show($"Фаил {res.FileName} сохранен", "Экспорт данных");

                            var fileData = System.IO.File.ReadAllText(res.FilePath);

                            if (fileData.Length > 10000)
                            {
                                fileData = new string(fileData.Take(10000).ToArray());
                            }

                            DataFile.Content = fileData;
                            DataFile.Path = res.FilePath;
                            DataFile.Name = System.IO.Path.GetFileName(res.FilePath);
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

                return true;
            }
            catch
            {
                bynaryData.Clear();
                OutputImage.Clear();
                await UpdateList.Invoke();
                return false;
            }
        }

        public bool SaveData()
        {
            return _saveAction.Invoke();
        }
    }
}
