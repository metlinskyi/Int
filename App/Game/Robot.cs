internal sealed class Robot
{
    public const int MaxHealth = 3;
    public const int Damage = 1;

    public Robot(string name, char symbol, Position startPosition)
    {
        Name = name;
        Symbol = symbol;
        Position = startPosition;
        Health = MaxHealth;
    }

    public string Name { get; }
    public char Symbol { get; }
    public int Health { get; private set; }
    public Position Position { get; set; }
    public bool IsAlive => Health > 0;

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }
}
