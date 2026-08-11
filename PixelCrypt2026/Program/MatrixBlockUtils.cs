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

        public static byte[] ReorderBlocks(byte[] pixels, int width, int height, int blockWidth, int blockHeight, List<int> customOrder)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (customOrder == null)
                throw new ArgumentNullException(nameof(customOrder));
            if (blockWidth <= 0 || blockHeight <= 0)
                throw new ArgumentException("Block dimensions must be positive");
            if (width % blockWidth != 0 || height % blockHeight != 0)
                throw new ArgumentException("Image dimensions must be divisible by block dimensions");

            const int bytesPerPixel = 4;
            int stride = width * bytesPerPixel;

            int blocksInRow = height / blockHeight;
            int blocksInCol = width / blockWidth;
            int totalBlocks = blocksInRow * blocksInCol;

            if (customOrder.Count != totalBlocks)
                throw new ArgumentException("customOrder length must match the number of blocks");

            byte[] result = new byte[pixels.Length];
            int blockBytes = blockWidth * bytesPerPixel;

            int resultBlockIndex = 0;
            foreach (int sourceBlockIndex in customOrder)
            {
                if (sourceBlockIndex < 0 || sourceBlockIndex >= totalBlocks)
                    throw new ArgumentException($"Invalid block index {sourceBlockIndex} in customOrder");

                int sourceBandX = sourceBlockIndex / blocksInRow;
                int sourceBandY = sourceBlockIndex % blocksInRow;

                int destinationBandX = resultBlockIndex / blocksInRow;
                int destinationBandY = resultBlockIndex % blocksInRow;

                int sourceStartX = sourceBandX * blockWidth;
                int sourceStartY = sourceBandY * blockHeight;
                int destinationStartX = destinationBandX * blockWidth;
                int destinationStartY = destinationBandY * blockHeight;

                for (int y = 0; y < blockHeight; y++)
                {
                    int sourceOffset = (sourceStartY + y) * stride + sourceStartX * bytesPerPixel;
                    int destinationOffset = (destinationStartY + y) * stride + destinationStartX * bytesPerPixel;
                    Buffer.BlockCopy(pixels, sourceOffset, result, destinationOffset, blockBytes);
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
