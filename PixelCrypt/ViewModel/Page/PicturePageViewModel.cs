using Microsoft.Win32;
using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            using (var image = System.Drawing.Image.FromFile(imagepath))
            using (var bitmap = new Bitmap(image))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        bitmap.SetPixel(i, j, EncryptPixel(bitmap.GetPixel(i, j), keybyte[index % keybyte.Length]));
                        index++;
                    }
                }
                return ConvertToBitmapImage(bitmap);
            }
        }

        public static BitmapImage DecryptPhoto(string imagepath, string key)
        {
            int index = 0;
            byte[] keybyte = Encoding.UTF8.GetBytes(key);
            using (var image = System.Drawing.Image.FromFile(imagepath))
            using (var bitmap = new Bitmap(image))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        bitmap.SetPixel(i, j, DecryptPixel(bitmap.GetPixel(i, j), keybyte[index % keybyte.Length]));
                        index++;
                    }
                }
                return ConvertToBitmapImage(bitmap);
            }
        }

        private static Color EncryptPixel(Color color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
            return Color.FromArgb(EncryptColor(color.R, key), EncryptColor(color.G, key), EncryptColor(color.B, key));
        }

        private static byte EncryptColor(byte color, byte key)
        {
            if (color % 2 != 0)
            {
                color = (byte)(254 - (color ^ key));
            }
            else
            {
                color = (byte)((color ^ key));
            }
            return color;
        }

        private static Color DecryptPixel(Color color, byte key)
        {
            if (key % 2 == 0)
            {
                key++;
            }
            return Color.FromArgb(DecryptColor(color.R, key), DecryptColor(color.G, key), DecryptColor(color.B, key));
        }

        private static byte DecryptColor(byte color, byte key)
        {
            if (color % 2 == 0)
            {
                color = (byte)((254 - color) ^ key);
            }
            else
            {
                color = (byte)((color ^ key));
            }
            return color;
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
    }
}
