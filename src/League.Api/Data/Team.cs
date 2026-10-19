namespace League.Api.Data;

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool Active { get; set; }

    public List<Driver> Drivers { get; set; } = [];
}
