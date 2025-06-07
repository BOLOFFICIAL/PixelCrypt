using PixelCrypt2025.Interfaces;
using PixelCrypt2025.ProgramData;
using System.Drawing;
using System.IO;
using System.Text;

namespace PixelCrypt2025.Model
{
    internal class Steganography : IImagePage
    {
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
                }
                else
                {
                    DataFile.Content = CryptoService.DecryptText(exportData, hashPassword);
                }
            }
            catch
            {
                bynaryData.Clear();
            }
        }
    }
}
