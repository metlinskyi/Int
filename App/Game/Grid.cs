public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsWall { get; set; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        IsOccupied = false;
        IsWall = false;
    }
}

public class Grid
{
    public const int BattlefieldSize = 10;
    public int Width { get; }
    public int Height { get; }
    public Cell[,] Cells { get; }
    public List<Wall> Walls { get; }

    public Grid()
    {
        Width = BattlefieldSize;
        Height = BattlefieldSize;
        Cells = new Cell[Width, Height];
        Walls = new List<Wall>();
        InitializeCells();
    }

    private void InitializeCells()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cells[x, y] = new Cell(x, y);
            }
        }
    }

    public void LoadWallsFromAscii(string mapPath, char wallChar = '#')
    {
        if (!File.Exists(mapPath))
        {
            throw new FileNotFoundException("ASCII map file not found.", mapPath);
        }

        Walls.Clear();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Cells[x, y].IsWall = false;
                Cells[x, y].IsOccupied = false;
            }
        }

        string[] lines = File.ReadAllLines(mapPath);
        int mapHeight = Math.Min(lines.Length, Height);

        for (int y = 0; y < mapHeight; y++)
        {
            string line = lines[y];
            int mapWidth = Math.Min(line.Length, Width);

            for (int x = 0; x < mapWidth; x++)
            {
                if (line[x] != wallChar)
                {
                    continue;
                }

                Cells[x, y].IsWall = true;
                Walls.Add(new Wall(x, y));
            }
        }
    }

    public bool TryPlaceRobotRandomly(Robot robot, Random random)
    {
        const int maxAttempts = 5000;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = random.Next(0, Width);
            int y = random.Next(0, Height);

            if (!IsCellAvailable(x, y))
            {
                continue;
            }

            robot.X = x;
            robot.Y = y;
            Cells[x, y].IsOccupied = true;
            return true;
        }

        return false;
    }

    public void ReleaseCell(int x, int y)
    {
        if (!IsInBounds(x, y))
        {
            return;
        }

        Cells[x, y].IsOccupied = false;
    }

    public bool TryMoveRobot(Robot robot, int targetX, int targetY)
    {
        if (!IsInBounds(targetX, targetY))
        {
            return false;
        }

        int distance = Math.Abs(targetX - robot.X) + Math.Abs(targetY - robot.Y);
        if (distance != 1)
        {
            return false;
        }

        if (!IsCellAvailable(targetX, targetY))
        {
            return false;
        }

        Cells[robot.X, robot.Y].IsOccupied = false;
        robot.X = targetX;
        robot.Y = targetY;
        Cells[targetX, targetY].IsOccupied = true;
        return true;
    }

    public bool IsWallAt(int x, int y)
    {
        return IsInBounds(x, y) && Cells[x, y].IsWall;
    }

    public bool HasClearLineOfSight(int fromX, int fromY, int toX, int toY)
    {
        if (!IsInBounds(fromX, fromY) || !IsInBounds(toX, toY))
        {
            return false;
        }

        if (fromX == toX)
        {
            int step = fromY < toY ? 1 : -1;
            for (int y = fromY + step; y != toY; y += step)
            {
                if (IsWallAt(fromX, y))
                {
                    return false;
                }
            }

            return true;
        }

        if (fromY == toY)
        {
            int step = fromX < toX ? 1 : -1;
            for (int x = fromX + step; x != toX; x += step)
            {
                if (IsWallAt(x, fromY))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public void Render(IReadOnlyCollection<Robot> robots)
    {
        char[,] canvas = new char[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                canvas[x, y] = Cells[x, y].IsWall ? '#' : '.';
            }
        }

        foreach (var robot in robots)
        {
            if (IsInBounds(robot.X, robot.Y))
            {
                canvas[robot.X, robot.Y] = robot.Symbol;
            }
        }

        for (int y = 0; y < Height; y++)
        {
            var row = new char[Width];
            for (int x = 0; x < Width; x++)
            {
                row[x] = canvas[x, y];
            }

            Console.WriteLine(new string(row));
        }
    }

    private bool IsCellAvailable(int x, int y)
    {
        return IsInBounds(x, y) && !Cells[x, y].IsWall && !Cells[x, y].IsOccupied;
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}