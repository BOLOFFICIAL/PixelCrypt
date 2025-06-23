using PixelCrypt2025.Enum;
using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using PixelCrypt2025.View.Page;
using System.Drawing;
using System.IO;
using System.Text;

namespace PixelCrypt2025.Model
{
    internal class Steganography : IImagePage
    {
        private Func<ActionResult> _saveAction;

        public List<Model.Image> InputImage { get; } = new List<Model.Image>();
        public Dictionary<Model.Image, Bitmap> OutputImage { get; } = new Dictionary<Model.Image, Bitmap>();
        public Model.File DataFile { get; } = new Model.File();
        public Func<Task> UpdateList { get; set; }
        public Action<Model.Image> ShowImage { get; set; }

        internal async Task<ActionResult> Import(string password)
        {
            var title = "Импорт данных";
            try
            {
                _saveAction = SaveImport;

                var hashPassword = ProgramHelper.GetHash32(password);
                var inportData = DataFile.Content;

                if (inportData.Length == 0)
                {
                    return new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = "Нет данных для импорта",
                        ResultTitle = title,
                    };
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
                    if (Context.MainWindowViewModel.CurrentPage.GetType() == typeof(SteganographyPage) && Context.MainWindow.IsActive)
                    {
                        ShowImage(InputImage[i]);
                        await UpdateList.Invoke();
                    }
                }

                return new ActionResult()
                {
                    IsSuccessResult = true,
                    ResultMessage = "Данные успешно импортированы",
                    ResultTitle = title,
                };
            }
            catch (Exception ex)
            {
                OutputImage.Clear();
                UpdateList.Invoke();
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                    ResultTitle = title,
                };
            }
        }

        internal async Task<ActionResult> Export(string password)
        {
            var result = new ActionResult();

            var title = "Экспорт данных";

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

                foreach (var item in bynaryData) allData.Append(item);

                var exportData = Converter.ConvertBinaryStringToText(allData.ToString());

                var exportFileData = exportData.Split("[*]");

                if (exportFileData.Length > 1)
                {
                    exportFileData[2] = CryptoService.DecryptText(exportFileData[2], hashPassword);

                    var doFile = Notification.Show("Экспортированные данные являются файлом.\nСобрать файл?", "Экспорт данных", NotificationType.YesNo, NotificationStatus.Question).Result == NotificationResultType.Yes;

                    if (doFile)
                    {
                        var res = ProgramHelper.SaveDataToFile(exportFileData[0], $"Файлы (*{exportFileData[1]})|*{exportFileData[1]}", Convert.FromBase64String(exportFileData[2]));

                        result = res.Result;

                        if (result.IsSuccessResult)
                        {
                            string fileData = System.IO.File.ReadAllText(res.FilePath) ?? string.Empty;

                            DataFile.Content = fileData.Length > 10000 ? fileData.Substring(0, 10000) : fileData;
                            DataFile.Path = res.FilePath;
                            DataFile.Name = System.IO.Path.GetFileName(res.FilePath);
                        }
                        else
                        {
                            DataFile.Content = exportFileData[2];
                            DataFile.Path = "";
                            DataFile.Name = "";

                            result = new ActionResult()
                            {
                                IsSuccessResult = true,
                                ResultMessage = "Фаил экспортирован без сборки",
                                ResultTitle = title,
                            };
                        }
                    }
                    else
                    {
                        DataFile.Content = exportFileData[2];
                        DataFile.Path = "";
                        DataFile.Name = "";

                        result = new ActionResult()
                        {
                            IsSuccessResult = true,
                            ResultMessage = "Фаил экспортирован без формирования",
                            ResultTitle = title,
                        };
                    }
                }
                else
                {
                    DataFile.Content = CryptoService.DecryptText(exportData, hashPassword);

                    result = new ActionResult()
                    {
                        IsSuccessResult = true,
                        ResultMessage = "Данные успешно экспортрованы",
                        ResultTitle = title,
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                bynaryData.Clear();
                OutputImage.Clear();
                await UpdateList.Invoke();

                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                    ResultTitle = title,
                };
            }
        }

        public ActionResult SaveData()
        {
            return _saveAction.Invoke();
        }

        private ActionResult SaveImport()
        {
            return ProgramHelper.SaveBitmapToFolder(OutputImage);
        }

        private ActionResult SaveExport()
        {
            if (DataFile.Content.Length == 0)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = "Нет данных для сохранения",
                    ResultTitle = "Экспорт"
                };
            }

            return ProgramHelper.SaveDataToFile($"PixelCrypt_{DateTime.Now:yyyyMMddHHmmss}", $"Файлы (*.txt)|*.txt", DataFile.Content);
        }
    }
}
