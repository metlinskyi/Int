using Microsoft.Extensions.Logging;

internal class Application
{
    private readonly IHelloWorldService _helloWorldService;
    private readonly ILogger<Application> _logger;

    public Application(IHelloWorldService helloWorldService, ILogger<Application> logger)
    {
        _helloWorldService = helloWorldService;
        _logger = logger;
    }

    public async Task Run()
    {
        var message = _helloWorldService.GetMessage();
        Console.WriteLine(message);
    }
}