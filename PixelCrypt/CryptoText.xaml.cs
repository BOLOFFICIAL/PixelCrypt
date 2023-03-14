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
    /// Логика взаимодействия для CryptoText.xaml
    /// </summary>
    public partial class CryptoText : Page
    {
        private Button selected_button = new Button();
        private string image_path = "";
        private string file_path = "";

        public CryptoText()
        {
            InitializeComponent();
            Initialization();
        }

        private void Initialization()
        {
            Button_Encrypt.Background = new SolidColorBrush(Colors.White);
            Button_Encrypt.BorderBrush = new SolidColorBrush(Colors.White);
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
            TextBox_Name.IsReadOnly = false;
            if (image_path.Length > 0)
            {
                TextBox_Name.Text = "";
            }
            if (!(file_path.Length > 0))
            {
                TextBox_Text.IsReadOnly = false;
                Button_File.Content = "Выбрать фаил для чтения";
            }
            else
            {
                TextBox_Text.IsReadOnly = true;
                TextBox_Text.Text = File.ReadAllText(file_path);
                Button_File.Content = "Фаил для чтения " + Path.GetFileName(file_path);
            }
        }

        private void Button_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Text = "";
            if (image_path.Length > 0)
            {
                TextBox_Name.Text = Path.GetFileName(image_path);
            }
            else
            {
                Button_File.Content = "Выбрать фаил для записи";
            }
            if (!(file_path.Length > 0))
            {
                Button_File.Content = "Выбрать фаил для записи";
            }
            else
            {
                Button_File.Content = "Фаил для записи " + Path.GetFileName(file_path);
            }
            Button_Decrypt.Background = new SolidColorBrush(Colors.White);
            Button_Decrypt.BorderBrush = new SolidColorBrush(Colors.White);
            Button_Encrypt.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            Button_Encrypt.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            selected_button = Button_Decrypt;
            TextBox_Name.IsReadOnly = true;
            Button_Crypto.Content = selected_button.Content;
            TextBox_Text.IsReadOnly = true;
            TextBox_Text.Text = "";
        }

        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image(*.jpg;*.png;*.jpeg;)|*.jpg;*.png;*.jpeg;";
            if (openFileDialog.ShowDialog() == true)
            {
                image_path = openFileDialog.FileName;
                Image_Image.Source = new BitmapImage(new Uri(image_path));
                if (selected_button == Button_Decrypt) 
                {
                    TextBox_Name.Text = Path.GetFileName(image_path);
                }
                //TextBox_Name.Text = DateTime.Now.ToString("HHmmssfff");
            }
        }

        private void Button_Crypto_Click(object sender, RoutedEventArgs e)
        {
            if (CheckCrypto())
            {
                if (selected_button.Content == Button_Encrypt.Content)
                {
                    TextCrypto.EncryptPhoto(TextBox_Text.Text, image_path, TextBox_Key.Text, TextBox_Name.Text);
                }
                if (selected_button.Content == Button_Decrypt.Content)
                {
                    TextBox_Text.Text = TextCrypto.DecryptPhoto(image_path, TextBox_Key.Text).Result;
                    if (file_path.Length > 0)
                    {
                        File.WriteAllText(file_path, TextBox_Text.Text);
                    }
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
            if (TextBox_Name.Text.Length < 1)
            {
                MessageBox.Show("Введите имя для картинки");
                return false;
            }
            if (TextBox_Key.Text.Length < 1)
            {
                MessageBox.Show("Введите ключ");
                return false;
            }
            if (TextBox_Name.Text.Length < 1) 
            {
                MessageBox.Show("Укажите имя для зашифованной картинки");
                return false;
            }
            var image = Bitmap.FromFile(image_path);
            if ((image.Width * image.Height) < (TextBox_Text.Text.Length * 16 + 16))
            {
                MessageBox.Show("Картинка мала для этого текста");
                return false;
            }
            return true;
        }

        private void Button_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                file_path = openFileDialog.FileName;
                if (selected_button.Content == Button_Encrypt.Content)
                {
                    Button_File.Content = "Фаил для чтения " + Path.GetFileName(file_path);
                }
                else
                {
                    Button_File.Content = "Фаил для записи " + Path.GetFileName(file_path);
                }

                if (selected_button.Content == Button_Encrypt.Content)
                {
                    TextBox_Text.Text = File.ReadAllText(file_path);
                }
                TextBox_Text.IsReadOnly = true;
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            file_path = "";
            if (selected_button.Content == Button_Encrypt.Content)
            {
                Button_File.Content = "Выбрать фаил для чтения";
                TextBox_Text.IsReadOnly = false;
            }
            else
            {
                Button_File.Content = "Выбрать фаил для записи";
            }
        }
    }
}
