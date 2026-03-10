internal static class Pathfinding
{
    private static readonly Direction[] NeighborOrder =
    [
        Direction.Right,
        Direction.Down,
        Direction.Left,
        Direction.Up
    ];

    public static Position? FindNextStep(Battlefield battlefield, Position start, Position goal)
    {
        if (start == goal)
        {
            return null;
        }

        var openSet = new PriorityQueue<Position, int>();
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int> { [start] = 0 };
        var closed = new HashSet<Position>();

        openSet.Enqueue(start, Heuristic(start, goal));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            if (closed.Contains(current))
            {
                continue;
            }

            if (current == goal)
            {
                return ReconstructFirstStep(cameFrom, start, goal);
            }

            closed.Add(current);
            var currentCost = gScore[current];

            foreach (var direction in NeighborOrder)
            {
                var next = current.Offset(direction);
                if (!battlefield.IsInside(next) || battlefield.IsWall(next))
                {
                    continue;
                }

                // Goal is allowed even if occupied by the enemy robot.
                if (next != goal && battlefield.IsOccupied(next))
                {
                    continue;
                }

                var tentativeG = currentCost + 1;
                if (gScore.TryGetValue(next, out var existing) && tentativeG >= existing)
                {
                    continue;
                }

                cameFrom[next] = current;
                gScore[next] = tentativeG;
                var f = tentativeG + Heuristic(next, goal);
                openSet.Enqueue(next, f);
            }
        }

        return null;
    }

    private static int Heuristic(Position a, Position b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static Position ReconstructFirstStep(Dictionary<Position, Position> cameFrom, Position start, Position goal)
    {
        var step = goal;
        while (cameFrom.TryGetValue(step, out var parent) && parent != start)
        {
            step = parent;
        }

        return step;
    }
}
