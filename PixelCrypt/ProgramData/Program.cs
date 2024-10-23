using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt.ProgramData
{
    public class Program
    {
        public static List<string> SplitStringIntoParts(string str, int partsCount)
        {
            List<string> result = new List<string>();

            int partLength = str.Length / partsCount;
            int remainder = str.Length % partsCount;

            int currentPosition = 0;

            for (int i = 0; i < partsCount; i++)
            {
                int currentPartLength = partLength + (remainder > 0 ? 1 : 0);
                remainder--;

                result.Add(str.Substring(currentPosition, currentPartLength));
                currentPosition += currentPartLength;
            }

            return result;
        }

        public static string GetHash32(string input)
        {
            string output;
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return output = Convert.ToHexString(hash);
        }

        public static bool SaveDataToFile(string filePath, string data)
        {
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, data);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}