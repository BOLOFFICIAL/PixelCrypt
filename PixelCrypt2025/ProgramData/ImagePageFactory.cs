using PixelCrypt.View.Page;
using System.Windows.Controls;

namespace PixelCrypt.ProgramData
{
    internal static class ImagePageFactory
    {
        public static Page CreatePage(string key)
        {
            return key switch
            {
                "Сryptography" => new CryptographyPage(),
                "Steganography" => new SteganographyPage(),
                _ => new MainPage()
            };
        }
    }
}
