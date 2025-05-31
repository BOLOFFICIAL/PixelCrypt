using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt.ProgramData
{
    public class Program
    {
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

        public static string GetHash32(string input)
        {
            string output;
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return output = Convert.ToHexString(hash);
        }

        public static SaveDataResult SaveDataToFile(string fileName, string filter, string data)
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

                    if (!File.Exists(selectedFilePath))
                    {
                        using (File.Create(selectedFilePath)) { }
                    }

                    File.WriteAllText(selectedFilePath, data);

                    return new SaveDataResult()
                    {
                        Result = true,
                        FileName = Path.GetFileName(selectedFilePath),
                        FilePath = selectedFilePath,
                    };
                }

                return new SaveDataResult();
            }
            catch
            {
                return new SaveDataResult();
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

                    if (!File.Exists(selectedFilePath))
                    {
                        using (File.Create(selectedFilePath)) { }
                    }

                    File.WriteAllBytes(selectedFilePath, data);

                    return new SaveDataResult()
                    {
                        Result = true,
                        FileName = Path.GetFileName(selectedFilePath),
                        FilePath = selectedFilePath,
                    };
                }

                return new SaveDataResult();
            }
            catch
            {
                return new SaveDataResult();
            }

        }

        public static SaveDataResult SaveBitmapToFolder(List<string> filePathImages, List<Bitmap> resultImages)
        {
            try
            {
                CommonOpenFileDialog folderPicker = new CommonOpenFileDialog();

                folderPicker.IsFolderPicker = true;
                folderPicker.Title = "Выбор папки для хранения данных";
                var now = DateTime.Now;
                folderPicker.DefaultFileName = $"PixelCrypt_{now:yyyyMMddHHmmss}";
                folderPicker.InitialDirectory = Path.GetDirectoryName(filePathImages[0]);

                CommonFileDialogResult dialogResult = folderPicker.ShowDialog();

                if (dialogResult == CommonFileDialogResult.Ok)
                {
                    if (!Directory.Exists(folderPicker.FileName))
                    {
                        Directory.CreateDirectory(folderPicker.FileName);
                    }

                    for (int i = 0; i < resultImages.Count; i++)
                    {
                        var name = Path.Combine(folderPicker.FileName, Path.GetFileNameWithoutExtension(filePathImages[i]) + $"_PixelCrypt_{now:yyyyMMddHHmmss}.png");
                        resultImages[i].Save(name, ImageFormat.Png);
                    }

                    return new SaveDataResult()
                    {
                        Result = true,
                        FileName = Path.GetFileName(folderPicker.FileName),
                        FilePath = folderPicker.FileName,
                    };
                }

                return new SaveDataResult();
            }
            catch
            {
                return new SaveDataResult();
            }
        }
    }
}