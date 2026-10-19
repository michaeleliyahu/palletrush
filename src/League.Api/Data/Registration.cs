namespace League.Api.Data;

public class Registration
{
    public int Id { get; set; }
    public int HeatId { get; set; }
    public int DriverId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Heat Heat { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
