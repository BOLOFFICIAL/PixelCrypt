using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PixelCrypt.ViewModel.Page
{
    internal class PicturePageViewModel : Base.ViewModel
    {
        private GridLength _resultImageWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _actionWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength _saveButtonWidth = new GridLength(0, GridUnitType.Pixel);

        public PicturePageViewModel()
        {
            ChoseImageCommand = new LambdaCommand(OnChoseImageCommandExecuted, CanChoseImageCommandExecute);
            DoActionCommand = new LambdaCommand(OnDoActionCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);
        }

        public GridLength ResultImageWidth
        {
            get => _resultImageWidth;
            set => Set(ref _resultImageWidth, value);
        }

        public GridLength ActionWidth
        {
            get => _actionWidth;
            set => Set(ref _actionWidth, value);
        }

        public GridLength SaveButtonWidth
        {
            get => _saveButtonWidth;
            set => Set(ref _saveButtonWidth, value);
        }

        private bool CanChoseImageCommandExecute(object arg)
        {
            return CanChoseImage();
        }

        public void OnChoseImageCommandExecuted(object p = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
            };

            openFileDialog.Title = "Выбор изображения";

            if (openFileDialog.ShowDialog() ?? false)
            {
                ImageData = openFileDialog.FileNames[0];

                FilePathImage = ImageData;

                ActionWidth = new GridLength(1, GridUnitType.Auto);
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                ResultImageWidth = SaveButtonWidth;
            }
        }

        public void OnDoActionCommandExecuted(object p = null)
        {
            try
            {
                if (p is not string action) return;

                _isSuccessAction = false;
                SaveButtonWidth = new GridLength(0, GridUnitType.Pixel);
                ResultImageWidth = SaveButtonWidth;

                switch (action)
                {
                    case "Encrypt": Encrypt(); break;
                    case "Decrypt": Decrypt(); break;
                }

                if (_isSuccessAction)
                {
                    SaveButtonWidth = new GridLength(1, GridUnitType.Auto);
                    ResultImageWidth = new GridLength(1, GridUnitType.Star);
                }
            }
            catch
            {
                MessageBox.Show("Не удалось выполнить действие");
            }
        }

        private void OnSaveCommandExecuted(object p = null)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();

                var dir = Path.GetDirectoryName(_filePathImage);
                var name = "PixelCrypt_" + (Path.GetFileNameWithoutExtension(_filePathImage) + "_" + DateTime.Now).Replace(":", "").Replace(" ", "").Replace(".", "");
                var format = ImageFormat.Png;

                format = ImageFormat.Png;
                saveFileDialog.Filter = "PNG Image|*.png";

                saveFileDialog.Title = "Сохранение изображения";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = dir;
                saveFileDialog.FileName = name;

                if (saveFileDialog.ShowDialog() ?? false)
                {
                    Program.ConvertToBitmap(Context.ResultImage).Save(saveFileDialog.FileName, format);
                    MessageBox.Show("Картинка сохранена", "Сохранение изображения");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Возникла ошибка при сохранении", "Сохранение");
            }
        }

        private void Decrypt()
        {
            var hashPassword = Program.Hash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            try
            {
                Context.ResultImage.Source = DecryptPhoto(_filePathImage, hashPassword);
                MessageBox.Show("Картинка успешно расшифрована");
                _isSuccessAction = true;
            }
            catch
            {
                MessageBox.Show("Не удалось расшифровать картинку");
                _isSuccessAction = false;
            }
        }

        private void Encrypt()
        {
            var hashPassword = Program.Hash32(Password?.Length > 0 ? Password : "PyxelCrypt");

            try
            {
                Context.ResultImage.Source = EncryptPhoto(_filePathImage, hashPassword);
                MessageBox.Show("Картинка успешно зашифрована");
                _isSuccessAction = true;
            }
            catch
            {
                MessageBox.Show("Не удалось зашифровать картинку");
                _isSuccessAction = false;
            }

        }

        private bool CanChoseImage()
        {
            var res = true;

            if (FilePathImage.Length > 0)
            {
                ChoseImageVisibility = Visibility.Collapsed;
                ChoseImageWidth = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                ChoseImageVisibility = Visibility.Visible;
                ChoseImageWidth = new GridLength(0, GridUnitType.Pixel);
            }

            return res;
        }

        public static BitmapImage EncryptPhoto(string imagepath, string key)
        {
            var pixels = Program.GetPixelsFromImageArray(imagepath);

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            var listPixels = Program.GetPixelsFromImageList(pixels);

            var encrypt = EncryptSequence(listPixels, key);

            return ConvertToBitmapImage(Program.CreateBitmapFromPixelsList(encrypt, width, height));
        }

        public static BitmapImage DecryptPhoto(string imagepath, string key)
        {
            var pixels = Program.GetPixelsFromImageArray(imagepath);

            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            var listPixels = Program.GetPixelsFromImageList(pixels);

            var decrypt = DecryptSequence(listPixels, key);

            return ConvertToBitmapImage(Program.CreateBitmapFromPixelsList(decrypt, width, height));
        }

        public static BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private static List<int> CreatePermutation(string password, int length)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                int hashValue = BitConverter.ToInt32(hash, 0);

                List<int> perm = Enumerable.Range(0, length).ToList();
                Random random = new Random(hashValue);
                perm = perm.OrderBy(x => random.Next()).ToList();

                return perm;
            }
        }

        private static List<T> EncryptSequence<T>(List<T> collection, string password)
        {
            int length = collection.Count;
            List<int> perm = CreatePermutation(password, length);
            List<T> encryptedCollection = new List<T>(new T[length]);
            for (int i = 0; i < length; i++)
            {
                encryptedCollection[i] = collection[perm[i]];
            }
            return encryptedCollection;
        }
        
        private static List<T> DecryptSequence<T>(List<T> encryptedCollection, string password)
        {
            int length = encryptedCollection.Count;
            List<int> perm = CreatePermutation(password, length);
            List<int> reversePerm = new List<int>(new int[length]);
            for (int i = 0; i < length; i++)
            {
                reversePerm[perm[i]] = i;
            }
            List<T> decryptedCollection = new List<T>(new T[length]);
            for (int i = 0; i < length; i++)
            {
                decryptedCollection[i] = encryptedCollection[reversePerm[i]];
            }
            return decryptedCollection;
        }
    }
}
