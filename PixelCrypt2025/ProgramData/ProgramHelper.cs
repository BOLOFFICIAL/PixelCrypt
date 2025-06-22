using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt2025.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt2025.ProgramData
{
    internal static class ProgramHelper
    {
        public static string GetHash32(string input)
        {
            string output;
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return output = Convert.ToHexString(hash);
        }

        public static List<string> SplitStringIntoParts(string str, int partsCount)
        {
            List<string> result = new List<string>();

            int partLength = str.Length / partsCount;
            int remainder = str.Length % partsCount;

            int currentPosition = 0;

            for (int i = 0; i < partsCount; i++)
            {
                int currentPartLength = partLength + (remainder > 0 ? 1 : 0);
                remainder--;

                result.Add(str.Substring(currentPosition, currentPartLength));
                currentPosition += currentPartLength;
            }

            return result;
        }

        public static ActionResult SaveDataToFile(string fileName, string filter, string data)
        {
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

                    System.IO.File.WriteAllText(selectedFilePath, data);

                    return new ActionResult()
                    {
                        IsSuccessResult = true,
                        ResultMessage = $"Данные сохранены в папке {Path.GetFileName(selectedFilePath)}",
                        ResultTitle = "Сохранение данных",
                    };
                }

                return new ActionResult()
                {
                    IsSuccessResult = true,
                    ResultMessage = $"Данные не сохранены",
                    ResultTitle = "Сохранение данных",
                };
            }
            catch (Exception ex)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"{ex.Message}",
                    ResultTitle = "Сохранение данных",
                };
            }
        }

        public static SaveDataResult SaveDataToFile(string fileName, string filter, byte[] data)
        {
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

                    System.IO.File.WriteAllBytes(selectedFilePath, data);

                    return new SaveDataResult()
                    {
                        Result = new ActionResult()
                        {
                            IsSuccessResult = true,
                            ResultMessage = $"Фаил {Path.GetFileName(selectedFilePath)} успешно сохранен",
                            ResultTitle = "Сохранение данных",
                        },
                        FilePath = selectedFilePath
                    };
                }

                return new SaveDataResult()
                {
                    Result = new ActionResult()
                    {
                        IsSuccessResult = true,
                    }
                };
            }
            catch (Exception ex)
            {
                return new SaveDataResult()
                {
                    Result = new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = $"{ex.Message}",
                        ResultTitle = "Сохранение данных",
                    },
                };
            }

        }

        public static ActionResult SaveBitmapToFolder(Dictionary<Model.Image, Bitmap> images)
        {
            try
            {
                if (images.Count == 0)
                {
                    return new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = "Нет данных для сохранения",
                        ResultTitle = "Сохраение данных",
                    };
                }

                CommonOpenFileDialog folderPicker = new CommonOpenFileDialog();

                folderPicker.IsFolderPicker = true;
                folderPicker.Title = "Выбор папки для хранения данных";
                var now = DateTime.Now;
                folderPicker.DefaultFileName = $"PixelCrypt_{now:yyyyMMddHHmmss}";
                folderPicker.InitialDirectory = Path.GetDirectoryName(images.First().Key.Path);

                CommonFileDialogResult dialogResult = folderPicker.ShowDialog();

                if (dialogResult == CommonFileDialogResult.Ok)
                {
                    if (!Directory.Exists(folderPicker.FileName))
                    {
                        Directory.CreateDirectory(folderPicker.FileName);
                    }

                    foreach (var el in images)
                    {
                        var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(el.Key.Path) + $"_PixelCrypt_{now:yyyyMMddHHmmss}.png");
                        el.Value.Save(name, ImageFormat.Png);
                    }

                    return new ActionResult()
                    {
                        IsSuccessResult = true,
                        ResultMessage = $"Данные сохранены в папке {Path.GetFileName(folderPicker.FileName)}",
                        ResultTitle = "Сохраение данных",
                    };
                }

                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Данные не сохранены",
                    ResultTitle = "Сохраение данных",
                };
            }
            catch (Exception ex)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"{ex.Message}",
                    ResultTitle = "Сохранение данных",
                };
            }
        }
    }
}
