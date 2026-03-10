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
            currentTurn++;
            Console.WriteLine($"Turn {currentTurn}");

            for (int i = 0; i < _robots.Count; i++)
            {
                if (_robots[i].Health <= 0)
                {
                    Console.WriteLine($"{_robots[i].Name} has been destroyed!");
                    _grid.ReleaseCell(_robots[i].X, _robots[i].Y);
                    _robots.RemoveAt(i);
                    i--;
                    continue;
                }

                // Simple attack logic: each robot attacks the next one in the list
                int targetIndex = (i + 1) % _robots.Count;
                _robots[i].Attack(_robots[targetIndex]);
            }
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
}