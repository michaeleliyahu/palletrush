namespace Telemetry.Api.Data;

public class Reading
{
    public long Id { get; set; }
    public Guid EventId { get; set; }
    public required string Type { get; set; }
    public required string Payload { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
}
