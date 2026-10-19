using System.Text.Json;

namespace Shared.Contracts;

public static class CourseEvents
{
    public const string Stream = "course-events";

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}

public interface ICourseEvent
{
    Guid EventId { get; }
}

public record CourseEventEnvelope(string Type, JsonElement Payload);

public record RunStarted(Guid EventId, Guid RunId, int HeatId, int DriverId, DateTimeOffset SensorTs, string Firmware) : ICourseEvent;

public record ConeHit(Guid EventId, Guid RunId, string ConeId, DateTimeOffset SensorTs, string Firmware) : ICourseEvent;

public record RunFinished(Guid EventId, Guid RunId, DateTimeOffset SensorTs, string Firmware) : ICourseEvent;
