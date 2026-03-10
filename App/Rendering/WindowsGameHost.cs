using System.Windows.Forms;

internal static class WindowsGameHost
{
    public static Task RunAsync(GameEngine engine)
    {
        var spriteDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Sprites");
        SpriteGenerator.EnsureSprites(spriteDirectory);

        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        using var form = new WindowsGameForm(engine, spriteDirectory);
        System.Windows.Forms.Application.Run(form);
        return Task.CompletedTask;
    }
}
