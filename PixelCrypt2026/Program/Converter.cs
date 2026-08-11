using System.Text;

namespace PixelCrypt2026.Program
{
    internal static class Converter
    {
        public static string ConvertTextToBinaryString(string input)
        {
            var text = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            StringBuilder binary = new StringBuilder();
            foreach (char c in text)
            {
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return binary.ToString();
        }

        public static string ConvertBinaryStringToText(string binary)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string byteString = binary.Substring(i, 8);
                text.Append((char)Convert.ToByte(byteString, 2));
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(text.ToString()));
        }

        public static string ConvertIntToBinaryString(int number) => Convert.ToString(number, 2);

        public static int ConvertBinaryStringToInt(string binary) => Convert.ToInt32(binary, 2);

    }
}
