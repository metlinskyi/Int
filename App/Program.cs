using Microsoft.Extensions.DependencyInjection; 

await new ServiceCollection()
    .AddLogging()
    .AddScoped<IHelloWorldService, HelloWorldService>()
    .AddScoped<Application>()
    .BuildServiceProvider()
    .CreateApplicationScope();

