using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PixelCrypt
{
    /// <summary>
    /// Логика взаимодействия для CryptoImage.xaml
    /// </summary>
    public partial class CryptoImage : Page
    {
        private string image_path = "";
        private Button selected_button = new Button();

        public CryptoImage()
        {
            InitializeComponent();
            Initialization();
        }

        private void Initialization()
        {
            Button_Encrypt.Background = new SolidColorBrush(Colors.White);
            Button_Encrypt.BorderBrush = new SolidColorBrush(Colors.White);

            MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
            GridImage.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Star);
            GridImage.RowDefinitions[1].Height = new GridLength(0);
            GridImage.RowDefinitions[2].Height = new GridLength(0);

            Button_Image.Background = new SolidColorBrush(Colors.White);
            Button_Image.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 32, 32));

            selected_button = Button_Encrypt;
        }

        private void Button_Menu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Menu());
        }

        private void Button_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            Button_Encrypt.Background = new SolidColorBrush(Colors.White);
            Button_Encrypt.BorderBrush = new SolidColorBrush(Colors.White);
            Button_Decrypt.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            Button_Decrypt.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            selected_button = Button_Encrypt;
            Button_Crypto.Content = selected_button.Content;
        }

        private void Button_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            Button_Decrypt.Background = new SolidColorBrush(Colors.White);
            Button_Decrypt.BorderBrush = new SolidColorBrush(Colors.White);
            Button_Encrypt.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            Button_Encrypt.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            selected_button = Button_Decrypt;
            Button_Crypto.Content = selected_button.Content;
        }

        private void Button_Crypto_Click(object sender, RoutedEventArgs e)
        {
            if (CheckCrypto())
            {
                Image_Imagу_new.Source = Image_Image.Source;
                MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                if (selected_button == Button_Encrypt)
                {
                    Image_Imagу_new.Source = ImageCrypto.EncryptPhoto(image_path, TextBox_Key.Text);
                }
                else
                {
                    Image_Imagу_new.Source = ImageCrypto.DecryptPhoto(image_path, TextBox_Key.Text);
                }
            }
        }

        private bool CheckCrypto()
        {
            if (Image_Image.Source == null)
            {
                MessageBox.Show("Выберите картику");
                return false;
            }
            if (TextBox_Key.Text.Length < 1)
            {
                MessageBox.Show("Введите ключ");
                return false;
            }
            using (var image = Bitmap.FromFile(image_path))
            {
                if (Path.GetFileNameWithoutExtension(image_path) == TextBox_Image_Name.Text)
                {
                    MessageBox.Show("Имена не должны совпадать");
                    return false;
                }
            }
            return true;
        }

        private void Button_Image_Save_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSave())
            {
                BitmapSource bitmapSource = (BitmapSource)Image_Imagу_new.Source;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new FileStream(RenameFile(image_path, TextBox_Image_Save_Name.Text), FileMode.Create))
                {
                    encoder.Save(stream);
                }

                MessageBox.Show("Картинка успешно сохранена");
            }
        }

        private bool CheckSave()
        {
            if (TextBox_Image_Save_Name.Text.Length < 1)
            {
                MessageBox.Show("Введите имя для новой картинки");
                return false;
            }
            if (System.IO.Path.GetFileName(image_path) == TextBox_Image_Save_Name.Text+".jpg")
            {
                MessageBox.Show("Имена не должны совпадать");
                return false;
            }
            return true;
        }

        public static string RenameFile(string filePath, string name)
        {
            string directory = System.IO.Path.GetDirectoryName(filePath);
            string extension = System.IO.Path.GetExtension(filePath);
            string newFilePath = System.IO.Path.Combine(directory, name + extension);
            return newFilePath;
        }

        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image(*.jpg;*.png;*.jpeg;)|*.jpg;*.png;*.jpeg;";
            if (openFileDialog.ShowDialog() == true)
            {
                image_path = openFileDialog.FileName;
                Image_Image.Source = new BitmapImage(new Uri(image_path));

                TextBox_Image_Name.Text = System.IO.Path.GetFileName(image_path);

                GridImage.RowDefinitions[0].Height = new GridLength(30);
                GridImage.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
                GridImage.RowDefinitions[2].Height = new GridLength(30);

                Button_Image.Foreground = new SolidColorBrush(Colors.White);
                Button_Image.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 32, 32));

                Button_Image.MaxWidth = Double.PositiveInfinity;
                Button_Image.MaxHeight = Double.PositiveInfinity;

                MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
            }
        }
    }
}
