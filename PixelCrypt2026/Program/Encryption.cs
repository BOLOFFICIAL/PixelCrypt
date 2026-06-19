using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt2026.Program
{
    class Encryption
    {
        public static string EncryptText(string text, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(text);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string DecryptText(string text, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(text)))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        public static Task<Bitmap> EncryptPhoto(string imagePath, string key, int interference) => EncryptionSequence(imagePath, key, interference, EncryptSequence);

        public static Task<Bitmap> DecryptPhoto(string imagePath, string key, int interference) => EncryptionSequence(imagePath, key, interference, DecryptSequence);

        private static Task<Bitmap> EncryptionSequence(string imagepath, string key, int interference, Func<List<int>, string, List<int>> action)
        {
            return Task.Run(() =>
            {
                var pixels = ImageHelper.GetArrayPixelsFromImage(imagepath);
                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);

                (var rows, var cols) = MatrixBlockUtils.CalculateBlockDimensions(height, width, interference);

                int totalBlocks = (height / rows) * (width / cols);
                var sequence = Enumerable.Range(0, totalBlocks).ToList();
                var customOrder = action(sequence, key);
                var reorderBlocks = MatrixBlockUtils.ReorderBlocks(pixels, cols, rows, customOrder);
                var encrypt = ImageHelper.GetListPixelsFromArrayPixels(reorderBlocks);
                return Converter.ConvertPixelsToBitmap(encrypt, width, height);
            });
        }

        private static List<T> EncryptSequence<T>(List<T> collection, string password)
        {
            int length = collection.Count;
            List<int> perm = CreatePermutation(password, length);
            List<T> encryptedCollection = new List<T>(new T[length]);
            for (int i = 0; i < length; i++)
            {
                encryptedCollection[i] = collection[perm[i]];
            }
            return encryptedCollection;
        }

        private static List<T> DecryptSequence<T>(List<T> encryptedCollection, string password)
        {
            int length = encryptedCollection.Count;
            List<int> perm = CreatePermutation(password, length);
            List<int> reversePerm = new List<int>(new int[length]);
            for (int i = 0; i < length; i++)
            {
                reversePerm[perm[i]] = i;
            }
            List<T> decryptedCollection = new List<T>(new T[length]);
            for (int i = 0; i < length; i++)
            {
                decryptedCollection[i] = encryptedCollection[reversePerm[i]];
            }
            return decryptedCollection;
        }

        private static List<int> CreatePermutation(string password, int length)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                int hashValue = BitConverter.ToInt32(hash, 0);

                List<int> perm = Enumerable.Range(0, length).ToList();
                Random random = new Random(hashValue);
                perm = perm.OrderBy(x => random.Next()).ToList();

                return perm;
            }
        }
    }
}
