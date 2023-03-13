using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixelCrypt
{
    /// <summary>
    /// Логика взаимодействия для Crypto.xaml
    /// </summary>
    public partial class Crypto : Page
    {
        private Button selected_button;

        public Crypto(string name)
        {
            InitializeComponent();
            selected_button = Encrypt;
        }

        private void Button_to_menu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Menu());
        }

        private void Button_Crypto(object sender, RoutedEventArgs e)
        {
            if (selected_button.Name== "Зашифровать") 
            {

            }
            if (selected_button.Name == "Расшифровать")
            {

            }
            CryptoText.Visibility= Visibility.Collapsed;
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            Encrypt.Background = new SolidColorBrush(Colors.White);
            Encrypt.BorderBrush = new SolidColorBrush(Colors.White);
            Decrypt.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Decrypt.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            selected_button = (Button)sender;
            Button_crypto.Content = selected_button.Content;
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            Decrypt.Background = new SolidColorBrush(Colors.White);
            Decrypt.BorderBrush = new SolidColorBrush(Colors.White);
            Encrypt.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Encrypt.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            selected_button = (Button)sender;
            Button_crypto.Content = selected_button.Content;
        }

        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {
            Button_Image.Visibility = Visibility.Collapsed;
            CryptoImage.Visibility= Visibility.Visible;
        }
    }
}
