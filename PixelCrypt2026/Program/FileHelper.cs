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
                        var name = Path.Combine(targetFolder, baseName);

                        try
                        {
                            int counter = 1;
                            while (File.Exists(name))
                            {
                                name = Path.Combine(targetFolder, $"{baseName}_({counter}).png");
                                counter++;
                            }

                            el.ResultImage.Save(name, ImageFormat.Png);
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
