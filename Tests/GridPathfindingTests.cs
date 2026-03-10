namespace Tests;

public class GridPathfindingTests
{
    [Test]
    public void FindPath_ReturnsPath_AroundWall()
    {
        var grid = new Grid();
        grid.Cells[1, 0].IsWall = true;

        var path = grid.FindPath(0, 0, 2, 0);

        Assert.That(path, Is.Not.Empty);
        Assert.That(path[0], Is.EqualTo((0, 0)));
        Assert.That(path[^1], Is.EqualTo((2, 0)));
        Assert.That(path, Does.Not.Contain((1, 0)));
    }

    [Test]
    public void FindPath_ReturnsEmpty_WhenGoalUnreachable()
    {
        var grid = new Grid();
        grid.Cells[1, 0].IsWall = true;
        grid.Cells[0, 1].IsWall = true;

        var path = grid.FindPath(0, 0, 2, 0);

        Assert.That(path, Is.Empty);
    }
}
