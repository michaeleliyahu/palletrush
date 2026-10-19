namespace League.Api.Data;

public class Run
{
    public Guid Id { get; set; }
    public int HeatId { get; set; }
    public int DriverId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public int? RawTimeMs { get; set; }

    public Heat Heat { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
