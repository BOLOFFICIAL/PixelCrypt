namespace PixelCrypt2026.Program
{
    class MatrixBlockUtils
    {
        public static (int Height, int Width) CalculateBlockDimensions(int imageHeight, int imageWidth, int splitPercentage = 100)
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

        public static T[,] ReorderBlocks<T>(T[,] matrix, int blockRows, int blockCols, List<int> customOrder)
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

        private static List<((int Height, int Width), int BlockSize)> GetAllSplitOptions(int height = 1, int width = 1)
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

        private static List<int> GetNumberDivisors(int number)
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
    }
}
