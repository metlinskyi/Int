public static class MapGenerator
{
    public static void GenerateRandomMap(string mapPath, int width, int height, double wallRatio = 0.30)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Map dimensions must be greater than zero.");
        }

        if (wallRatio < 0 || wallRatio > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(wallRatio), "Wall ratio must be between 0 and 1.");
        }

        string? directory = Path.GetDirectoryName(mapPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        int totalCells = width * height;
        int wallCount = (int)Math.Round(totalCells * wallRatio);
        wallCount = Math.Clamp(wallCount, 0, Math.Max(0, totalCells - 2));

        var allPositions = Enumerable.Range(0, totalCells).ToArray();
        Random random = Random.Shared;

        for (int i = 0; i < wallCount; i++)
        {
            int swapIndex = random.Next(i, allPositions.Length);
            (allPositions[i], allPositions[swapIndex]) = (allPositions[swapIndex], allPositions[i]);
        }

        var wallPositions = allPositions.Take(wallCount).ToHashSet();
        var lines = new string[height];

        for (int y = 0; y < height; y++)
        {
            var row = new char[width];
            for (int x = 0; x < width; x++)
            {
                int flatIndex = (y * width) + x;
                row[x] = wallPositions.Contains(flatIndex) ? '#' : '.';
            }

            lines[y] = new string(row);
        }

        File.WriteAllLines(mapPath, lines);
    }
}
