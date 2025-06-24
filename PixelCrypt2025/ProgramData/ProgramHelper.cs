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

        public static List<string> SplitStringIntoParts(string str, List<int> partsCount)
        {
            var result = new List<string>(partsCount.Count);
            int start = 0;

            foreach (int length in partsCount)
            {
                if (start + length > str.Length) return null;

                result.Add(str.Substring(start, length));
                start += length;
            }

            return result;
        }

        public static List<int> DistributeData(List<int> capacities, int data)
        {
            var result = new List<int>(new int[capacities.Count]);

            while (data > 0)
            {
                bool anyFilled = false;

                for (int i = 0; i < capacities.Count; i++)
                {
                    if (result[i] < capacities[i])
                    {
                        result[i]++;
                        data--;
                        anyFilled = true;

                        if (data == 0)
                            return result;
                    }
                }

                if (!anyFilled)
                    return null;
            }

            return result;
        }

        public static ActionResult SaveDataToFile(string fileName, string filter, string data)
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

                    System.IO.File.WriteAllText(selectedFilePath, data);

                    return new ActionResult()
                    {
                        IsSuccessResult = true,
                        ResultMessage = $"Данные сохранены в файле {Path.GetFileName(selectedFilePath)}",
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
                    ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                    ResultTitle = title,
                };
            }
        }

        public static SaveDataResult SaveDataToFile(string fileName, string filter, byte[] data)
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

                    System.IO.File.WriteAllBytes(selectedFilePath, data);

                    return new SaveDataResult()
                    {
                        Result = new ActionResult()
                        {
                            IsSuccessResult = true,
                            ResultMessage = $"Фаил {Path.GetFileName(selectedFilePath)} успешно сохранен",
                            ResultTitle = title,
                        },
                        FilePath = selectedFilePath
                    };
                }

                return new SaveDataResult()
                {
                    Result = new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = "Данные не сохранены",
                        ResultTitle = title,
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
                        ResultMessage = $"Неизвестная ошибка: {ex.Message}",
                        ResultTitle = title,
                    },
                };
            }

        }

        public static ActionResult SaveBitmapToFolder(Dictionary<Model.Image, Bitmap> images)
        {
            var title = "Сохранение данных";
            Model.Image currentElement = null;

            try
            {
                if (images.Count == 0)
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
                        currentElement = el.Key;
                        var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(currentElement.Path) + $"_PixelCrypt_{now:yyyyMMddHHmmss}.png");
                        el.Value.Save(name, ImageFormat.Png);
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
                    ResultMessage = $"В процессе сохранения данных для\n\n{currentElement?.Name}\n\nвозникла неизвестная ошибка: {ex.Message}",
                    ResultTitle = title,
                };
            }
        }
    }
}
