using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelCrypt.ProgramData
{
    internal class Cryptography
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

        public static async Task<Bitmap> EncryptPhoto(string imagepath, string key)
        {
            var encryptedPixels = await Task.Run(() =>
            {
                var pixels = ImageHelper.GetArrayPixelsFromImage(imagepath);
                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);

                var listPixels = ImageHelper.GetListPixelsFromArrayPixels(pixels);
                var encrypt = EncryptSequence(listPixels, key);

                return Converter.ConvertPixelsToBitmap(encrypt, width, height);
            });

            return encryptedPixels;
        }

        public static async Task<Bitmap> DecryptPhoto(string imagepath, string key)
        {
            var decryptedPixels = await Task.Run(() =>
            {
                var pixels = ImageHelper.GetArrayPixelsFromImage(imagepath);
                int width = pixels.GetLength(0);
                int height = pixels.GetLength(1);

                var listPixels = ImageHelper.GetListPixelsFromArrayPixels(pixels);
                var decrypt = DecryptSequence(listPixels, key);

                return Converter.ConvertPixelsToBitmap(decrypt, width, height);
            });

            return decryptedPixels;
        }

        public static List<T> EncryptSequence<T>(List<T> collection, string password)
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

        public static List<T> DecryptSequence<T>(List<T> encryptedCollection, string password)
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
