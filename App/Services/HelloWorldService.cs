internal class GameService : IGameService
{
    public Task RunAsync()
    {
        var engine = GameEngine.CreateRandom(new Random());
        return WindowsGameHost.RunAsync(engine);
    }
}