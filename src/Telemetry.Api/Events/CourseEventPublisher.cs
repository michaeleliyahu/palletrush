using System.Text.Json;
using Shared.Contracts;
using StackExchange.Redis;
using Telemetry.Api.Data;

namespace Telemetry.Api.Events;

public class CourseEventPublisher(TelemetryDbContext db, IConnectionMultiplexer redis)
{
    public async Task PublishAsync(ICourseEvent courseEvent)
    {
        var type = courseEvent.GetType().Name;
        var payload = JsonSerializer.Serialize(courseEvent, courseEvent.GetType(), CourseEvents.JsonOptions);

        db.Readings.Add(new Reading
        {
            EventId = courseEvent.EventId,
            Type = type,
            Payload = payload,
            ReceivedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync();

        await PublishRawAsync(type, payload);
    }

    public Task PublishRawAsync(string type, string payload) =>
        redis.GetDatabase().StreamAddAsync(CourseEvents.Stream,
        [
            new NameValueEntry("type", type),
            new NameValueEntry("payload", payload),
        ]);
}
