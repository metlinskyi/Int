using Microsoft.Extensions.DependencyInjection; 

await new ServiceCollection()
    .AddLogging()
    .AddScoped<IGameService, GameService>()
    .AddScoped<Application>()
    .BuildServiceProvider()
    .CreateApplicationScope();

