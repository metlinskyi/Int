using System.Drawing;

internal static class IsometricProjection
{
    public static PointF ToScreen(Position position, float tileWidth, float tileHeight, float originX, float originY)
    {
        var screenX = (position.X - position.Y) * (tileWidth / 2f) + originX;
        var screenY = (position.X + position.Y) * (tileHeight / 2f) + originY;
        return new PointF(screenX, screenY);
    }

    public static PointF[] Diamond(PointF center, float tileWidth, float tileHeight)
    {
        var halfW = tileWidth / 2f;
        var halfH = tileHeight / 2f;

        return
        [
            new PointF(center.X, center.Y - halfH),
            new PointF(center.X + halfW, center.Y),
            new PointF(center.X, center.Y + halfH),
            new PointF(center.X - halfW, center.Y)
        ];
    }
}
