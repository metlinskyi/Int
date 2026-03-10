using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

internal static class SpriteGenerator
{
    public static void EnsureSprites(string directory)
    {
        Directory.CreateDirectory(directory);

        var wallPath = Path.Combine(directory, "wall.png");
        if (!File.Exists(wallPath))
        {
            GenerateWallSprite(wallPath);
        }

        var robot1Path = Path.Combine(directory, "robot1.png");
        if (!File.Exists(robot1Path))
        {
            GenerateRobotSprite(robot1Path, Color.FromArgb(188, 77, 44));
        }

        var robot2Path = Path.Combine(directory, "robot2.png");
        if (!File.Exists(robot2Path))
        {
            GenerateRobotSprite(robot2Path, Color.FromArgb(60, 139, 128));
        }
    }

    private static void GenerateWallSprite(string path)
    {
        using var bitmap = new Bitmap(64, 64);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var baseBrush = new SolidBrush(Color.FromArgb(93, 90, 84));
        using var shadowBrush = new SolidBrush(Color.FromArgb(64, 50, 46, 42));
        using var linePen = new Pen(Color.FromArgb(140, 130, 122), 2f);

        var points = new[]
        {
            new PointF(32, 8),
            new PointF(58, 22),
            new PointF(32, 52),
            new PointF(6, 22)
        };

        graphics.FillPolygon(baseBrush, points);
        graphics.FillPolygon(shadowBrush, new[]
        {
            new PointF(32, 22),
            new PointF(58, 22),
            new PointF(32, 52),
            new PointF(6, 22)
        });

        graphics.DrawPolygon(linePen, points);
        bitmap.Save(path, ImageFormat.Png);
    }

    private static void GenerateRobotSprite(string path, Color body)
    {
        using var bitmap = new Bitmap(64, 64);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var legBrush = new SolidBrush(Color.FromArgb(60, 56, 52));
        using var bodyBrush = new SolidBrush(body);
        using var accentBrush = new SolidBrush(Color.FromArgb(218, 199, 116));
        using var outline = new Pen(Color.FromArgb(30, 24, 20), 2f);

        graphics.FillEllipse(legBrush, 18, 38, 10, 16);
        graphics.FillEllipse(legBrush, 36, 38, 10, 16);
        graphics.FillRoundedRectangle(bodyBrush, 14, 16, 36, 28, 6);
        graphics.FillEllipse(accentBrush, 24, 22, 16, 10);
        graphics.DrawRoundedRectangle(outline, 14, 16, 36, 28, 6);
        bitmap.Save(path, ImageFormat.Png);
    }

    private static void FillRoundedRectangle(this Graphics g, Brush brush, float x, float y, float w, float h, float radius)
    {
        using var path = RoundedRectPath(x, y, w, h, radius);
        g.FillPath(brush, path);
    }

    private static void DrawRoundedRectangle(this Graphics g, Pen pen, float x, float y, float w, float h, float radius)
    {
        using var path = RoundedRectPath(x, y, w, h, radius);
        g.DrawPath(pen, path);
    }

    private static GraphicsPath RoundedRectPath(float x, float y, float w, float h, float radius)
    {
        var diameter = radius * 2f;
        var path = new GraphicsPath();
        var arc = new RectangleF(x, y, diameter, diameter);

        path.AddArc(arc, 180, 90);
        arc.X = x + w - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = y + h - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = x;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}
