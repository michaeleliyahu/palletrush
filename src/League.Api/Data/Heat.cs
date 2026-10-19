namespace League.Api.Data;

public class Heat
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
}
