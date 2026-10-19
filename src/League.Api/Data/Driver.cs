namespace League.Api.Data;

public class Driver
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public required string Name { get; set; }
    public required string LicenseNo { get; set; }

    public Team Team { get; set; } = null!;
}
