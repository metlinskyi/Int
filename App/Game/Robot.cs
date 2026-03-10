public class Robot : GridItem
{
    public string Name { get; }
    public char Symbol { get; }
    public int Health { get; private set; }

    public Robot(string name, char symbol, int x = 0, int y = 0, int health = 3) : base(x, y)
    {
        Name = name;
        Symbol = symbol;
        Health = health;
    }

    public void Attack(Robot target)
    {
        target.ApplyDamage(1);
        Console.WriteLine($"{Name} attacks {target.Name} for 1 damage. {target.Name} HP: {Math.Max(0, target.Health)}");
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0 || Health <= 0)
        {
            return;
        }

        Health -= amount;
    }
}