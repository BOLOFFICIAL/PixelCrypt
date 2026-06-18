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

                (var rows, var cols) = Matrix.Info(height, width, interference);
                int totalBlocks = (height / rows) * (width / cols);
                var sequence = Enumerable.Range(0, totalBlocks).ToList();
                var customOrder = action(sequence, key);
                var reorderBlocks = Matrix.ReorderBlocksCustom(pixels, cols, rows, customOrder);
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

    class Matrix
    {
        public static T[][] ToJaggedArray<T>(T[,] twoDimensionalArray)
        {
            int rows = twoDimensionalArray.GetLength(0);
            int cols = twoDimensionalArray.GetLength(1);

            T[][] jaggedArray = new T[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new T[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }

            return jaggedArray;
        }

        public static T[,] To2DArray<T>(T[][] jaggedArray)
        {
            if (jaggedArray == null || jaggedArray.Length == 0 || jaggedArray[0] == null)
                return new T[0, 0];

            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;

            T[,] twoDArray = new T[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                if (jaggedArray[i] == null)
                    throw new ArgumentException($"Строка {i} равна null");

                for (int j = 0; j < cols; j++)
                {
                    twoDArray[i, j] = jaggedArray[i][j];
                }
            }

            return twoDArray;
        }

        public static (int Height, int Width) Info(int imageHeight, int imageWidth, int splitPercentage = 100)
        {
            var allSplitOptions = GetAllSplitOptions(imageHeight, imageWidth)
                .OrderBy(e => e.BlockSize)
                .ThenBy(e => e.Item1.Width)
                .ThenBy(e => e.Item1.Height).ToList();

            if (allSplitOptions.Count == 0) return (1, 1);

            var optimalOptionsByCount = new Dictionary<int, (int Height, int Width)>();

            var groupsByBlockCount = allSplitOptions.GroupBy(option => option.BlockSize);

            foreach (var group in groupsByBlockCount)
            {
                (var bestHeight, var bestWidth) = group.First().Item1;

                var minAspectDiff = Math.Abs(bestHeight - bestWidth);

                foreach (var option in group)
                {
                    var currentAspectDiff = Math.Abs(option.Item1.Width - option.Item1.Height);

                    if (currentAspectDiff < minAspectDiff)
                    {
                        bestWidth = option.Item1.Width;
                        bestHeight = option.Item1.Height;
                        minAspectDiff = currentAspectDiff;
                    }
                }

                optimalOptionsByCount.Add(group.Key, (bestHeight, bestWidth));
            }

            var targetOptionIndex = Math.Ceiling(splitPercentage * optimalOptionsByCount.Count() / 100.0);

            targetOptionIndex = targetOptionIndex == 0 ? 1 : targetOptionIndex;

            var selectedEntry = optimalOptionsByCount.ElementAt((int)targetOptionIndex - 1);

            var totalBlocks = selectedEntry.Key;
            var blockHeight = selectedEntry.Value.Height;
            var blockWidth = selectedEntry.Value.Width;

            return (blockHeight, blockWidth);
        }

        static List<((int Height, int Width), int BlockSize)> GetAllSplitOptions(int height = 1, int width = 1)
        {
            var heightDivisors = GetNumberDivisors(height);
            var widthDivisors = GetNumberDivisors(width);

            var splitOptions = new List<((int Height, int Width), int BlockCount)>();

            foreach (var blockHeight in heightDivisors)
            {
                var blocksPerColumn = height / blockHeight;

                foreach (var blockWidth in widthDivisors)
                {
                    splitOptions.Add(((blockHeight, blockWidth), blocksPerColumn * width / blockWidth));
                }
            }

            return splitOptions;
        }

        static List<int> GetNumberDivisors(int number)
        {
            var divisors = new List<int>();
            for (int i = 1; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    divisors.Add(i);
                    if (i != number / i)
                    {
                        divisors.Add(number / i);
                    }
                }
            }

            return divisors;
        }

        public static T[,] ReorderBlocksCustom<T>(T[,] matrix, int blockRows, int blockCols, List<int> customOrder)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (customOrder == null)
                throw new ArgumentNullException(nameof(customOrder));
            if (blockRows <= 0 || blockCols <= 0)
                throw new ArgumentException("Block dimensions must be positive");

            int totalRows = matrix.GetLength(0);
            int totalCols = matrix.GetLength(1);

            if (totalRows % blockRows != 0 || totalCols % blockCols != 0)
                throw new ArgumentException("Matrix dimensions must be divisible by block dimensions");

            int blocksInRow = totalCols / blockCols;
            int blocksInCol = totalRows / blockRows;
            int totalBlocks = blocksInRow * blocksInCol;

            T[][,] blocks = new T[totalBlocks][,];

            for (int i = 0; i < totalBlocks; i++)
            {
                int blockRow = i / blocksInRow;
                int blockCol = i % blocksInRow;

                blocks[i] = new T[blockRows, blockCols];

                int startRow = blockRow * blockRows;
                int startCol = blockCol * blockCols;

                for (int r = 0; r < blockRows; r++)
                {
                    for (int c = 0; c < blockCols; c++)
                    {
                        blocks[i][r, c] = matrix[startRow + r, startCol + c];
                    }
                }
            }

            T[,] result = new T[totalRows, totalCols];

            int resultBlockIndex = 0;
            foreach (int sourceBlockIndex in customOrder)
            {
                if (sourceBlockIndex < 0 || sourceBlockIndex >= totalBlocks)
                    throw new ArgumentException($"Invalid block index {sourceBlockIndex} in customOrder");

                int resultBlockRow = resultBlockIndex / blocksInRow;
                int resultBlockCol = resultBlockIndex % blocksInRow;

                int startRow = resultBlockRow * blockRows;
                int startCol = resultBlockCol * blockCols;

                T[,] sourceBlock = blocks[sourceBlockIndex];

                for (int r = 0; r < blockRows; r++)
                {
                    for (int c = 0; c < blockCols; c++)
                    {
                        result[startRow + r, startCol + c] = sourceBlock[r, c];
                    }
                }

                resultBlockIndex++;
            }

            return result;
        }
    }
}
