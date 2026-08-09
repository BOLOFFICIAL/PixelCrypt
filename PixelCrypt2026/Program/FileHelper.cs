using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt2026.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PixelCrypt2026.Program
{
    internal static class FileHelper
    {
        public static SaveDataResult SaveDataToFile(string fileName, string filter, string data) => SaveDataToFile(fileName, filter, data, System.IO.File.WriteAllText);

        public static SaveDataResult SaveDataToFile(string fileName, string filter, byte[] data) => SaveDataToFile(fileName, filter, data, System.IO.File.WriteAllBytes);

        public static ActionResult SaveImageToFolder(List<ImageFile> imageFiles)
        {
            var title = "Saving data";
            var currentImage = imageFiles.FirstOrDefault();

            try
            {
                if (imageFiles.Count == 0)
                {
                    return new ActionResult()
                    {
                        IsSuccessResult = false,
                        ResultMessage = "No data to save",
                        ResultTitle = title,
                    };
                }

                var folderPicker = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Choose folder to save images"
                };

                if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var now = DateTime.Now;
                    var newFolderName = $"PixelCrypt_{now:yyyyMMddHHmmss}";

                    var targetFolder = Path.Combine(folderPicker.FileName, newFolderName);

                    Directory.CreateDirectory(targetFolder);

                    var errorList = new List<string>();

                    foreach (var el in imageFiles)
                    {
                        currentImage = el;
                        var baseName = Path.GetFileNameWithoutExtension(currentImage.FilePath);
                        var name = Path.Combine(targetFolder, baseName + ".png");

                        try
                        {
                            int counter = 1;
                            while (File.Exists(name))
                            {
                                name = Path.Combine(targetFolder, $"{baseName}_({counter}).png");
                                counter++;
                            }

                            CopyFileAndDeleteOriginal(el.ResultImage, name, true);
                        }
                        catch (Exception ex)
                        {
                            errorList.Add($"Error in saving {baseName}: {ex.Message}");
                        }
                    }

                    if (errorList.Any())
                    {
                        if (errorList.Count == imageFiles.Count)
                        {
                            return new ActionResult()
                            {
                                IsSuccessResult = false,
                                ResultMessage = $"Errors occurred when saving all the images:\n\n{string.Join("\n", errorList)}",
                                ResultTitle = title,
                            };
                        }
                        else
                        {
                            return new ActionResult()
                            {
                                IsSuccessResult = true,
                                ResultMessage = $"Some images were not saved due to errors:\n\n{string.Join("\n", errorList)}",
                                ResultTitle = title,
                            };
                        }
                    }
                    else
                    {
                        return new ActionResult()
                        {
                            IsSuccessResult = true,
                            ResultMessage = $"Successfully saved to folder {newFolderName}",
                            ResultTitle = title,
                        };
                    }
                }

                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = "Canceling data saving",
                    ResultTitle = title,
                };
            }
            catch (Exception ex)
            {
                return new ActionResult()
                {
                    IsSuccessResult = false,
                    ResultMessage = $"Error saving: \n\n{currentImage?.FileName}\n\n {ex.Message}",
                    ResultTitle = title,
                };
            }
        }

        public static void CopyFileAndDeleteOriginal(string sourceFilePath, string destinationFilePath, bool toDelete = false)
        {
            try
            {
                string? destDir = Path.GetDirectoryName(destinationFilePath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                if (toDelete)
                    File.Delete(sourceFilePath);
            }
            catch { }
        }

        public static List<string> SaveBitmapToFolder(string path = null, params Bitmap[] imageFiles)
        {
            if (string.IsNullOrEmpty(path))
                path = Path.Combine(Path.GetTempPath(), "PixelCrypt");

            if (imageFiles == null || imageFiles.Count() == 0)
                return null;

            var res = new List<string>();

            try
            {
                Directory.CreateDirectory(path);

                for (int i = 0; i < imageFiles.Count(); i++)
                {
                    Bitmap bmp = imageFiles[i];
                    if (bmp == null)
                        return null;

                    string fileName = Path.Combine(path, $"{Guid.NewGuid()}.png");
                    bmp.Save(fileName, ImageFormat.Png);
                    res.Add(fileName);
                }

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static SaveDataResult SaveDataToFile<T>(string fileName, string filter, T data, Action<string, T> action)
        {
            var title = "Saving data";

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Select a file to save data",
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
                            ResultMessage = $"Successfully saved to {Path.GetFileName(selectedFilePath)}",
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
                        ResultMessage = "Canceling data saving",
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
                        ResultMessage = $"An unexpected error occurred: {ex.Message}",
                        ResultTitle = title,
                    },
                    FilePath = "",
                };
            }
        }
    }
}
