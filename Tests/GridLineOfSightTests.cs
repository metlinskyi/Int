namespace Tests;

public class GridLineOfSightTests
{
    [Test]
    public void HasClearLineOfSight_ReturnsFalse_WhenWallBetweenRobots()
    {
        var grid = new Grid();
        grid.Cells[1, 0].IsWall = true;

        bool canShoot = grid.HasClearLineOfSight(0, 0, 2, 0);

        Assert.That(canShoot, Is.False);
    }

    [Test]
    public void HasClearLineOfSight_ReturnsTrue_WhenNoWallBetweenRobots()
    {
        var grid = new Grid();

        bool canShoot = grid.HasClearLineOfSight(0, 0, 2, 0);

        Assert.That(canShoot, Is.True);
    }
}
