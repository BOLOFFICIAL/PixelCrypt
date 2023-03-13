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
    /// Логика взаимодействия для CryptoImage.xaml
    /// </summary>
    public partial class CryptoImage : Page
    {
        public CryptoImage()
        {
            InitializeComponent();
            Initialization();
        }

        private void Initialization()
        {
            Button_Encrypt.Background = new SolidColorBrush(Colors.White);
            Button_Encrypt.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void Button_Menu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Menu());
        }

        private void Button_Encrypt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Decrypt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Crypto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
