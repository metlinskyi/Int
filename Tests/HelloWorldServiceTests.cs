namespace Tests;

public class GameEngineTests
{
    [Test]
    public void CreateRandom_GeneratesGridWithinExpectedSizeAndTwoRobots()
    {
        var engine = GameEngine.CreateRandom(new Random(1234));

        Assert.That(engine.Battlefield.Width, Is.InRange(15, 30));
        Assert.That(engine.Battlefield.Height, Is.InRange(15, 30));
        Assert.That(engine.Battlefield.Robots.Length, Is.EqualTo(2));
        Assert.That(engine.Battlefield.Robots.All(r => r.Health == Robot.MaxHealth), Is.True);
    }

    [Test]
    public void CreateRandom_PlacesRobotsAsFarAsPossible()
    {
        var engine = GameEngine.CreateRandom(new Random(42));
        var battlefield = engine.Battlefield;

        var freeCells = new List<Position>();
        for (var y = 0; y < battlefield.Height; y++)
        {
            for (var x = 0; x < battlefield.Width; x++)
            {
                var position = new Position(x, y);
                if (!battlefield.IsWall(position))
                {
                    freeCells.Add(position);
                }
            }
        }

        var maxDistance = -1;
        for (var i = 0; i < freeCells.Count - 1; i++)
        {
            for (var j = i + 1; j < freeCells.Count; j++)
            {
                var distance = Math.Abs(freeCells[i].X - freeCells[j].X) + Math.Abs(freeCells[i].Y - freeCells[j].Y);
                maxDistance = Math.Max(maxDistance, distance);
            }
        }

        var first = battlefield.Robots[0].Position;
        var second = battlefield.Robots[1].Position;
        var robotsDistance = Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);

        Assert.That(robotsDistance, Is.EqualTo(maxDistance));
    }

    [Test]
    public void ExecuteTurn_BlockedMove_DoesNotAdvanceTurn()
    {
        var battlefield = new Battlefield(15, 15);
        var robotA = new Robot("Robot 1", '1', new Position(1, 1));
        var robotB = new Robot("Robot 2", '2', new Position(14, 14));

        battlefield.SetRobots(new[] { robotA, robotB });
        battlefield.SetWall(new Position(1, 0), true);

        var engine = new GameEngine(battlefield, startRobotIndex: 0);
        var result = engine.ExecuteTurn(GameAction.MoveUp);

        Assert.That(result.Success, Is.False);
        Assert.That(engine.TurnNumber, Is.EqualTo(1));
        Assert.That(engine.GetCurrentRobot().Name, Is.EqualTo("Robot 1"));
    }

    [Test]
    public void ExecuteTurn_ShootingDamageAndKill_EndsGame()
    {
        var battlefield = new Battlefield(15, 15);
        var robotA = new Robot("Robot 1", '1', new Position(0, 0));
        var robotB = new Robot("Robot 2", '2', new Position(0, 3));

        battlefield.SetRobots(new[] { robotA, robotB });

        var engine = new GameEngine(battlefield, startRobotIndex: 0);

        var r1 = engine.ExecuteTurn(GameAction.ShootDown);
        var r2 = engine.ExecuteTurn(GameAction.ShootUp);
        var r3 = engine.ExecuteTurn(GameAction.ShootDown);
        var r4 = engine.ExecuteTurn(GameAction.ShootUp);
        var final = engine.ExecuteTurn(GameAction.ShootDown);

        Assert.Multiple(() =>
        {
            Assert.That(r1.Success, Is.True);
            Assert.That(r2.Success, Is.True);
            Assert.That(r3.Success, Is.True);
            Assert.That(r4.Success, Is.True);
            Assert.That(final.Success, Is.True);
            Assert.That(engine.IsGameOver, Is.True);
            Assert.That(engine.Winner?.Name, Is.EqualTo("Robot 1"));
            Assert.That(robotB.IsAlive, Is.False);
            Assert.That(robotB.Health, Is.EqualTo(0));
        });

    }

    [Test]
    public void SimpleRobotAi_WhenTargetInClearLine_ChoosesShoot()
    {
        var battlefield = new Battlefield(15, 15);
        var robotA = new Robot("Robot 1", '1', new Position(4, 3));
        var robotB = new Robot("Robot 2", '2', new Position(4, 10));
        battlefield.SetRobots(new[] { robotA, robotB });

        var engine = new GameEngine(battlefield, startRobotIndex: 0);
        var ai = new SimpleRobotAi();

        var action = ai.DecideNextAction(engine);

        Assert.That(action, Is.EqualTo(GameAction.ShootDown));
    }

    [Test]
    public void SimpleRobotAi_WhenNoLineOfSight_ChoosesMoveTowardsEnemy()
    {
        var battlefield = new Battlefield(15, 15);
        var robotA = new Robot("Robot 1", '1', new Position(1, 1));
        var robotB = new Robot("Robot 2", '2', new Position(1, 4));
        battlefield.SetRobots(new[] { robotA, robotB });

        // Block direct vertical line so pathfinding must route to the right corridor.
        battlefield.SetWall(new Position(1, 2), true);
        battlefield.SetWall(new Position(0, 1), true);
        battlefield.SetWall(new Position(2, 2), true);
        battlefield.SetWall(new Position(0, 2), true);

        var engine = new GameEngine(battlefield, startRobotIndex: 0);
        var ai = new SimpleRobotAi();

        var action = ai.DecideNextAction(engine);

        Assert.That(action, Is.EqualTo(GameAction.MoveRight));
    }
}
