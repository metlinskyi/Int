using Microsoft.Extensions.Logging;

internal class Application
{
    private readonly ILogger<Application> _logger;

    public Application(ILogger<Application> logger)
    {
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("Starting application...");

        string mapPath = Path.Combine(AppContext.BaseDirectory, "Maps", "battlefield.txt");
        MapGenerator.GenerateRandomMap(mapPath, Grid.BattlefieldSize, Grid.BattlefieldSize, 0.30);
        var gameLoop = new GameLoop(mapPath);


        // Start the game loop
        gameLoop.Start();

        _logger.LogInformation("Application finished.");
    }
}