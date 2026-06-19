using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt2026.Model;
using System.Drawing.Imaging;
using System.IO;

namespace PixelCrypt2026.Program
{
    internal static class FileHelper
    {
        public static SaveDataResult SaveDataToFile(string fileName, string filter, string data) => SaveDataToFile(fileName, filter, data, System.IO.File.WriteAllText);

        public static SaveDataResult SaveDataToFile(string fileName, string filter, byte[] data) => SaveDataToFile(fileName, filter, data, System.IO.File.WriteAllBytes);

        public static ActionResult SaveBitmapToFolder(List<ImageFile> imageFiles)
        {
            var title = "Сохранение данных";
            var currentImage = imageFiles.FirstOrDefault();

            try
            {
                if (imageFiles.Count == 0)
                {
                    return new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = "Нет данных для сохранения",
                        ResultTitle = title,
                    };
                }

                CommonOpenFileDialog folderPicker = new CommonOpenFileDialog();

                folderPicker.IsFolderPicker = true;
                folderPicker.Title = "Выбор папки для хранения данных";
                var now = DateTime.Now;
                folderPicker.DefaultFileName = $"PixelCrypt_{now:yyyyMMddHHmmss}";
                folderPicker.InitialDirectory = Path.GetDirectoryName(currentImage.FilePath);

                CommonFileDialogResult dialogResult = folderPicker.ShowDialog();

                if (dialogResult == CommonFileDialogResult.Ok)
                {
                    if (!Directory.Exists(folderPicker.FileName))
                    {
                        Directory.CreateDirectory(folderPicker.FileName);
                    }

                    foreach (var el in imageFiles)
                    {
                        currentImage = el;
                        var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(currentImage.FilePath) + $"_PixelCrypt_{now:yyyyMMddHHmmss}.png");
                        el.ResultImage.Save(name, ImageFormat.Png);
                    }

                    return new ActionResult()
                    {
                        IsSuccessResult = true,
                        ResultMessage = $"Данные сохранены в папке {Path.GetFileName(folderPicker.FileName)}",
                        ResultTitle = title,
                    };
                }

                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Данные не сохранены",
                    ResultTitle = title,
                };
            }
            catch (Exception ex)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"В процессе сохранения данных для\n\n{currentImage?.FileName}\n\nвозникла неизвестная ошибка: {ex.Message}",
                    ResultTitle = title,
                };
            }
        }


        private static SaveDataResult SaveDataToFile<T>(string fileName, string filter, T data, Action<string, T> action)
        {
            var title = "Сохранение данных";

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Выбрать файл для сохранения данных",
                    FileName = fileName,
                    Filter = filter
                };

                if (saveFileDialog.ShowDialog() ?? false)
                {
                    var selectedFilePath = saveFileDialog.FileName;

                    if (!System.IO.File.Exists(selectedFilePath))
                    {
                        using (System.IO.File.Create(selectedFilePath)) { }
                    }

                    action(selectedFilePath, data);

                    return new SaveDataResult()
                    {
                        Result = new ActionResult()
                        {
                            IsSuccessResult = true,
                            ResultMessage = $"Данные сохранены в файле {Path.GetFileName(selectedFilePath)}",
                            ResultTitle = title,
                        },
                        FilePath = selectedFilePath,
                    };
                }

                return new SaveDataResult()
                {
                    Result = new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = $"Данные не сохранены",
                        ResultTitle = title,
                    },
                    FilePath = "",
                };
            }
            catch (Exception ex)
            {
                return new SaveDataResult()
                {
                    Result = new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                        ResultTitle = title,
                    },
                    FilePath = "",
                };
            }
        }
    }
}
