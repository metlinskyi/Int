public class GameLoop
{
    private readonly Grid _grid;
    private readonly List<Robot> _robots;
    private readonly Random _random;
    private const int MaxTurns = 100;

    public GameLoop(string mapPath)
    {
        _grid = new Grid();
        _robots = new List<Robot>();
        _random = Random.Shared;

        _grid.LoadWallsFromAscii(mapPath);
        SpawnTwoRobots();
    }

    public void AddRobot(Robot robot)
    {
        bool placed = _grid.TryPlaceRobotRandomly(robot, _random);
        if (!placed)
        {
            throw new InvalidOperationException("Unable to place robot on the battlefield.");
        }

        _robots.Add(robot);
        Console.WriteLine($"{robot.Name} has entered the arena!");
    }

    private void SpawnTwoRobots()
    {
        AddRobot(new Robot("Robot-A", 'A'));
        AddRobot(new Robot("Robot-B", 'B'));
    }

    public void Start()
    {
        Console.WriteLine("Game started on a 50x50 battlefield!");
        _grid.Render(_robots);

        int currentTurn = 0;
        while (_robots.Count > 1 && currentTurn < MaxTurns)
        {
            WaitForEnterToRunNextTurn(currentTurn + 1);
            currentTurn++;
            Console.WriteLine($"Turn {currentTurn}");

            RemoveDestroyedRobots();
            if (_robots.Count <= 1)
            {
                break;
            }

            var pendingDamage = new Dictionary<Robot, int>();

            for (int i = 0; i < _robots.Count; i++)
            {
                int targetIndex = (i + 1) % _robots.Count;
                var actor = _robots[i];
                var target = _robots[targetIndex];

                if (CanShoot(actor, target))
                {
                    if (!pendingDamage.ContainsKey(target))
                    {
                        pendingDamage[target] = 0;
                    }

                    pendingDamage[target] += 1;
                    Console.WriteLine($"{actor.Name} prepares shot at {target.Name}.");
                    continue;
                }

                bool moved = TryMoveOneCellToward(actor, target);
                if (!moved)
                {
                    Console.WriteLine($"{actor.Name} cannot move this turn.");
                }
            }

            ResolveEndOfTurnDamage(pendingDamage);
            RemoveDestroyedRobots();

            Console.WriteLine($"Battlefield after turn {currentTurn}:");
            _grid.Render(_robots);
        }

        if (_robots.Count == 1)
        {
            Console.WriteLine($"{_robots[0].Name} is the winner!");
        }
        else
        {
            Console.WriteLine("It's a draw!");
        }
    }

    private static void WaitForEnterToRunNextTurn(int nextTurn)
    {
        Console.WriteLine($"Press Enter to run turn {nextTurn}...");
        Console.ReadLine();
    }

    private void ResolveEndOfTurnDamage(Dictionary<Robot, int> pendingDamage)
    {
        if (pendingDamage.Count == 0)
        {
            return;
        }

        Console.WriteLine("Resolving end-of-turn damage...");
        foreach (var (target, damage) in pendingDamage)
        {
            target.ApplyDamage(damage);
            Console.WriteLine($"{target.Name} takes {damage} damage. HP: {Math.Max(0, target.Health)}");
        }
    }

    private void RemoveDestroyedRobots()
    {
        for (int i = 0; i < _robots.Count; i++)
        {
            if (_robots[i].Health > 0)
            {
                continue;
            }

            Console.WriteLine($"{_robots[i].Name} has been destroyed!");
            _grid.ReleaseCell(_robots[i].X, _robots[i].Y);
            _robots.RemoveAt(i);
            i--;
        }
    }

    private bool CanShoot(Robot shooter, Robot target)
    {
        return _grid.HasClearLineOfSight(shooter.X, shooter.Y, target.X, target.Y);
    }

    private bool TryMoveOneCellToward(Robot mover, Robot target)
    {
        var candidates = new List<(int x, int y)>();

        if (target.X > mover.X)
        {
            candidates.Add((mover.X + 1, mover.Y));
        }
        else if (target.X < mover.X)
        {
            candidates.Add((mover.X - 1, mover.Y));
        }

        if (target.Y > mover.Y)
        {
            candidates.Add((mover.X, mover.Y + 1));
        }
        else if (target.Y < mover.Y)
        {
            candidates.Add((mover.X, mover.Y - 1));
        }

        foreach (var candidate in candidates)
        {
            if (_grid.TryMoveRobot(mover, candidate.x, candidate.y))
            {
                Console.WriteLine($"{mover.Name} moves to ({mover.X},{mover.Y}).");
                return true;
            }
        }

        return false;
    }
}