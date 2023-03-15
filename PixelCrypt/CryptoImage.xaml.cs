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

            MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
            GridImage.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Star);
            GridImage.RowDefinitions[1].Height = new GridLength(0);
            GridImage.RowDefinitions[2].Height = new GridLength(0);

            Button_Image.Background = new SolidColorBrush(Colors.White);
            Button_Image.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 32, 32));
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
            MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {
            GridImage.RowDefinitions[0].Height = new GridLength(30);
            GridImage.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            GridImage.RowDefinitions[2].Height = new GridLength(30);

            Button_Image.Foreground = new SolidColorBrush(Colors.White);
            Button_Image.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 32, 32));
        }
    }
}
