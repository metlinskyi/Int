using System.Drawing;
using System.Windows.Forms;

internal sealed class WindowsGameForm : Form
{
    private readonly GameEngine _engine;
    private readonly SimpleRobotAi _ai;
    private readonly System.Windows.Forms.Timer _turnTimer;
    private readonly Image _wallSprite;
    private readonly Image _robot1Sprite;
    private readonly Image _robot2Sprite;

    private const float TileWidth = 48f;
    private const float TileHeight = 24f;

    public WindowsGameForm(GameEngine engine, string spriteDirectory)
    {
        _engine = engine;
        _ai = new SimpleRobotAi();
        Text = "Post-apocalypse Robot Battle";
        ClientSize = new Size(800, 600);
        MinimumSize = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        _wallSprite = Image.FromFile(Path.Combine(spriteDirectory, "wall.png"));
        _robot1Sprite = Image.FromFile(Path.Combine(spriteDirectory, "robot1.png"));
        _robot2Sprite = Image.FromFile(Path.Combine(spriteDirectory, "robot2.png"));

        _turnTimer = new System.Windows.Forms.Timer { Interval = 250 };
        _turnTimer.Tick += OnTurnTick;
        _turnTimer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        _turnTimer.Stop();
        _turnTimer.Dispose();
        _wallSprite.Dispose();
        _robot1Sprite.Dispose();
        _robot2Sprite.Dispose();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.Clear(Color.FromArgb(30, 30, 26));

        var battlefield = _engine.Battlefield;
        var originX = ClientSize.Width / 2f;
        var originY = 80f;

        for (var y = 0; y < battlefield.Height; y++)
        {
            for (var x = 0; x < battlefield.Width; x++)
            {
                var pos = new Position(x, y);
                var center = IsometricProjection.ToScreen(pos, TileWidth, TileHeight, originX, originY);
                var diamond = IsometricProjection.Diamond(center, TileWidth, TileHeight);

                using var floorBrush = new SolidBrush(Color.FromArgb(52, 60, 54));
                using var floorPen = new Pen(Color.FromArgb(70, 86, 74));
                g.FillPolygon(floorBrush, diamond);
                g.DrawPolygon(floorPen, diamond);

                if (battlefield.IsWall(pos))
                {
                    DrawSprite(g, _wallSprite, center.X - 18, center.Y - 44, 36, 44);
                    continue;
                }

                var robot = battlefield.Robots.FirstOrDefault(r => r.IsAlive && r.Position == pos);
                if (robot is not null)
                {
                    var sprite = robot.Symbol == '1' ? _robot1Sprite : _robot2Sprite;
                    DrawSprite(g, sprite, center.X - 18, center.Y - 44, 36, 44);
                }
            }
        }

        DrawHud(g);
    }

    private void OnTurnTick(object? sender, EventArgs e)
    {
        if (_engine.IsGameOver)
        {
            _turnTimer.Stop();
            return;
        }

        var action = _ai.DecideNextAction(_engine);
        _engine.ExecuteTurn(action);
        Invalidate();
    }

    private void DrawSprite(Graphics g, Image image, float x, float y, float w, float h)
    {
        g.DrawImage(image, x, y, w, h);
    }

    private void DrawHud(Graphics g)
    {
        var r1 = _engine.Battlefield.Robots[0];
        var r2 = _engine.Battlefield.Robots[1];

        using var font = new Font("Segoe UI", 10f, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.FromArgb(230, 220, 210));

        var current = _engine.GetCurrentRobot();
        var hud = $"Turn: {_engine.TurnNumber} | Current: {current.Name} | R1 HP: {r1.Health} | R2 HP: {r2.Health}";

        if (_engine.IsGameOver)
        {
            hud += _engine.Winner is null ? " | Draw" : $" | Winner: {_engine.Winner.Name}";
        }
        else
        {
            hud += " | AI battle in progress";
        }

        g.DrawString(hud, font, textBrush, 12, 12);
    }
}
