using Microsoft.Extensions.Logging;

internal class Application
{
    private readonly IGameService _gameService;
    private readonly ILogger<Application> _logger;

    public Application(IGameService gameService, ILogger<Application> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("Starting robot battle game");
        await _gameService.RunAsync();
    }
}