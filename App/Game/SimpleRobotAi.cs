internal sealed class SimpleRobotAi
{
    private static readonly (Direction Direction, GameAction MoveAction, GameAction ShootAction)[] DirectionMap =
    [
        (Direction.Up, GameAction.MoveUp, GameAction.ShootUp),
        (Direction.Down, GameAction.MoveDown, GameAction.ShootDown),
        (Direction.Left, GameAction.MoveLeft, GameAction.ShootLeft),
        (Direction.Right, GameAction.MoveRight, GameAction.ShootRight)
    ];

    public GameAction DecideNextAction(GameEngine engine)
    {
        var actor = engine.GetCurrentRobot();
        var opponent = engine.Battlefield.Robots.Single(r => r != actor && r.IsAlive);

        var directShot = TryGetDirectShot(engine.Battlefield, actor, opponent);
        if (directShot is not null)
        {
            return directShot.Value;
        }

        var nextMove = TryGetPathMove(engine.Battlefield, actor, opponent);
        if (nextMove is not null)
        {
            return nextMove.Value;
        }

        // If no path exists, still fire so the turn progresses.
        return GameAction.ShootUp;
    }

    private static GameAction? TryGetDirectShot(Battlefield battlefield, Robot actor, Robot opponent)
    {
        if (actor.Position.X == opponent.Position.X)
        {
            var direction = actor.Position.Y < opponent.Position.Y ? Direction.Down : Direction.Up;
            if (HasClearLine(battlefield, actor.Position, opponent.Position, direction))
            {
                return DirectionMap.Single(d => d.Direction == direction).ShootAction;
            }
        }

        if (actor.Position.Y == opponent.Position.Y)
        {
            var direction = actor.Position.X < opponent.Position.X ? Direction.Right : Direction.Left;
            if (HasClearLine(battlefield, actor.Position, opponent.Position, direction))
            {
                return DirectionMap.Single(d => d.Direction == direction).ShootAction;
            }
        }

        return null;
    }

    private static bool HasClearLine(Battlefield battlefield, Position start, Position target, Direction direction)
    {
        var cursor = start.Offset(direction);
        while (battlefield.IsInside(cursor))
        {
            if (cursor == target)
            {
                return true;
            }

            if (battlefield.IsWall(cursor))
            {
                return false;
            }

            cursor = cursor.Offset(direction);
        }

        return false;
    }

    private static GameAction? TryGetPathMove(Battlefield battlefield, Robot actor, Robot opponent)
    {
        var step = Pathfinding.FindNextStep(battlefield, actor.Position, opponent.Position);
        if (step is null)
        {
            return null;
        }

        return ResolveMoveAction(actor.Position, step.Value);
    }

    private static GameAction ResolveMoveAction(Position from, Position to)
    {
        if (to.X == from.X)
        {
            return to.Y < from.Y ? GameAction.MoveUp : GameAction.MoveDown;
        }

        return to.X < from.X ? GameAction.MoveLeft : GameAction.MoveRight;
    }
}
