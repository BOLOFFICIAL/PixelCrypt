using PixelCrypt2025.View.Page;
using System.Windows.Controls;

namespace PixelCrypt2025.ProgramData
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
