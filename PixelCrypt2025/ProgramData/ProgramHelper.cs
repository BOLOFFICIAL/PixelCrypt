using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PixelCrypt2025.Model;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt2025.ProgramData
{
    internal static class ProgramHelper
    {
        public static string GetHash32(string input)
        {
            string output;
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return output = Convert.ToHexString(hash);
        }

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

        public static List<string> SplitStringIntoParts(string str, List<int> partsCount)
        {
            var result = new List<string>(partsCount.Count);
            int start = 0;

            foreach (int length in partsCount)
            {
                if (start + length > str.Length) return null;

                result.Add(str.Substring(start, length));
                start += length;
            }

            return result;
        }

        public static List<int> DistributeData(List<int> capacities, int data)
        {
            var result = new List<int>(new int[capacities.Count]);

            while (data > 0)
            {
                bool anyFilled = false;

                for (int i = 0; i < capacities.Count; i++)
                {
                    if (result[i] < capacities[i])
                    {
                        result[i]++;
                        data--;
                        anyFilled = true;

                        if (data == 0)
                            return result;
                    }
                }

                if (!anyFilled)
                    return null;
            }

            return result;
        }
    }
}
