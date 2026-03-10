internal sealed class GameEngine
{
    private int _currentRobotIndex;

    public GameEngine(Battlefield battlefield, int startRobotIndex)
    {
        if (battlefield.Robots.Length != 2)
        {
            throw new ArgumentException("Game requires exactly two robots.", nameof(battlefield));
        }

        Battlefield = battlefield;
        _currentRobotIndex = startRobotIndex is 0 or 1
            ? startRobotIndex
            : throw new ArgumentOutOfRangeException(nameof(startRobotIndex));
        TurnNumber = 1;
    }

    public Battlefield Battlefield { get; }
    public int TurnNumber { get; private set; }
    public bool IsGameOver => Battlefield.Robots.Count(r => r.IsAlive) <= 1;
    public Robot? Winner => Battlefield.Robots.SingleOrDefault(r => r.IsAlive);

    public static GameEngine CreateRandom(Random random)
    {
        var battlefield = Battlefield.CreateRandom(random);
        var startRobot = random.Next(2);
        return new GameEngine(battlefield, startRobot);
    }

    public Robot GetCurrentRobot()
    {
        var robot = Battlefield.Robots[_currentRobotIndex];
        if (robot.IsAlive)
        {
            return robot;
        }

        _currentRobotIndex = (_currentRobotIndex + 1) % Battlefield.Robots.Length;
        return Battlefield.Robots[_currentRobotIndex];
    }

    public TurnResult ExecuteTurn(GameAction action)
    {
        if (IsGameOver)
        {
            return new TurnResult(false, "Game is already finished.");
        }

        var actor = GetCurrentRobot();
        var outcome = action switch
        {
            GameAction.MoveUp => TryMove(actor, Direction.Up),
            GameAction.MoveDown => TryMove(actor, Direction.Down),
            GameAction.MoveLeft => TryMove(actor, Direction.Left),
            GameAction.MoveRight => TryMove(actor, Direction.Right),
            GameAction.ShootUp => Shoot(actor, Direction.Up),
            GameAction.ShootDown => Shoot(actor, Direction.Down),
            GameAction.ShootLeft => Shoot(actor, Direction.Left),
            GameAction.ShootRight => Shoot(actor, Direction.Right),
            _ => new TurnResult(false, "Unknown action.")
        };

        if (!outcome.Success)
        {
            return outcome;
        }

        if (!IsGameOver)
        {
            AdvanceTurn();
        }

        return outcome;
    }

    private TurnResult TryMove(Robot actor, Direction direction)
    {
        if (!Battlefield.TryMove(actor, direction))
        {
            return new TurnResult(false, "Move blocked.");
        }

        return new TurnResult(true, $"{actor.Name} moved {direction}.");
    }

    private TurnResult Shoot(Robot actor, Direction direction)
    {
        var target = Battlefield.Shoot(actor, direction);
        if (target is null)
        {
            return new TurnResult(true, $"{actor.Name} fired {direction} and hit nothing.");
        }

        if (!target.IsAlive)
        {
            return new TurnResult(true, $"{actor.Name} destroyed {target.Name}.");
        }

        return new TurnResult(true, $"{actor.Name} hit {target.Name}. Remaining HP: {target.Health}.");
    }

    private void AdvanceTurn()
    {
        _currentRobotIndex = (_currentRobotIndex + 1) % Battlefield.Robots.Length;
        TurnNumber++;

        if (!Battlefield.Robots[_currentRobotIndex].IsAlive)
        {
            _currentRobotIndex = (_currentRobotIndex + 1) % Battlefield.Robots.Length;
        }
    }
}
