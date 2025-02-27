namespace Cashflow.API.Entities;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Icon { get; set; }

    public Player(string name)
    {
        Name = name;
    }
}