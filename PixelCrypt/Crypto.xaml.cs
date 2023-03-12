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
        int i = 0;

        public Crypto(string name)
        {
            InitializeComponent();
        }

        private void Button_to_menu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Menu());
        }

        private void Button_Crypto(object sender, RoutedEventArgs e)
        {
            if (i == 1)
            {
                i1.Visibility = Visibility.Visible;
                i2.Visibility = Visibility.Hidden;
            }
            if (i == 2)
            {
                i2.Visibility = Visibility.Visible;
                i1.Visibility = Visibility.Hidden;
            }
            if (i == 3)
            {
                i1.Visibility = Visibility.Hidden;
                i2.Visibility = Visibility.Hidden;
                i = 0;
            }
            i++;
        }
    }
}
