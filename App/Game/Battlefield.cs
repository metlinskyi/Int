internal sealed class Battlefield
{
    private readonly bool[,] _walls;

    public Battlefield(int width, int height)
    {
        Width = width;
        Height = height;
        _walls = new bool[width, height];
        Robots = Array.Empty<Robot>();
    }

    public int Width { get; }
    public int Height { get; }
    public Robot[] Robots { get; private set; }

    public static Battlefield CreateRandom(Random random)
    {
        const int minSize = 15;
        const int maxSize = 30;
        const double wallRatio = 0.30;

        for (var attempt = 0; attempt < 64; attempt++)
        {
            var width = random.Next(minSize, maxSize + 1);
            var height = random.Next(minSize, maxSize + 1);
            var battlefield = new Battlefield(width, height);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pos = new Position(x, y);
                    battlefield.SetWall(pos, random.NextDouble() < wallRatio);
                }
            }

            if (!battlefield.TryPlaceRobots(random))
            {
                continue;
            }

            return battlefield;
        }

        throw new InvalidOperationException("Unable to generate a valid battlefield with two robots.");
    }

    public bool IsInside(Position position)
    {
        return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
    }

    public bool IsWall(Position position)
    {
        return IsInside(position) && _walls[position.X, position.Y];
    }

    public bool IsOccupied(Position position)
    {
        return Robots.Any(r => r.IsAlive && r.Position == position);
    }

    public char GetTileSymbol(Position position)
    {
        var robot = Robots.FirstOrDefault(r => r.IsAlive && r.Position == position);
        if (robot is not null)
        {
            return robot.Symbol;
        }

        return IsWall(position) ? '#' : '.';
    }

    public bool TryMove(Robot robot, Direction direction)
    {
        var target = robot.Position.Offset(direction);
        if (!IsInside(target) || IsWall(target) || IsOccupied(target))
        {
            return false;
        }

        robot.Position = target;
        return true;
    }

    public Robot? Shoot(Robot attacker, Direction direction)
    {
        var cursor = attacker.Position.Offset(direction);
        while (IsInside(cursor))
        {
            if (IsWall(cursor))
            {
                return null;
            }

            var hit = Robots.FirstOrDefault(r => r.IsAlive && r.Position == cursor);
            if (hit is not null)
            {
                hit.TakeDamage(Robot.Damage);
                return hit;
            }

            cursor = cursor.Offset(direction);
        }

        return null;
    }

    internal void SetWall(Position position, bool value)
    {
        _walls[position.X, position.Y] = value;
    }

    internal void SetRobots(Robot[] robots)
    {
        Robots = robots;
    }

    private bool TryPlaceRobots(Random random)
    {
        var freeCells = new List<Position>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var pos = new Position(x, y);
                if (!IsWall(pos))
                {
                    freeCells.Add(pos);
                }
            }
        }

        if (freeCells.Count < 2)
        {
            return false;
        }

        var bestPairs = new List<(Position A, Position B)>();
        var maxDistance = -1;
        for (var i = 0; i < freeCells.Count - 1; i++)
        {
            for (var j = i + 1; j < freeCells.Count; j++)
            {
                var a = freeCells[i];
                var b = freeCells[j];
                var distance = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
                if (distance > maxDistance)
                {
                    bestPairs.Clear();
                    maxDistance = distance;
                    bestPairs.Add((a, b));
                }
                else if (distance == maxDistance)
                {
                    bestPairs.Add((a, b));
                }
            }
        }

        if (bestPairs.Count == 0)
        {
            return false;
        }

        var pair = bestPairs[random.Next(bestPairs.Count)];
        Robots = new[]
        {
            new Robot("Robot 1", '1', pair.A),
            new Robot("Robot 2", '2', pair.B)
        };

        return true;
    }
}
