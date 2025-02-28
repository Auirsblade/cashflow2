namespace Cashflow.API.Entities;

public class Player(string name)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public int BoardSpaceId { get; set; } = 1;
}