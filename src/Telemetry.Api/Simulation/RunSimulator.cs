using Shared.Contracts;
using Telemetry.Api.Events;

namespace Telemetry.Api.Simulation;

public class RunSimulator(CourseEventPublisher publisher)
{
    public async Task<Guid> SimulateAsync(int heatId, int driverId, int? seed)
    {
        var random = seed is { } s ? new Random(s) : new Random();
        var runId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;

        var durationMs = random.Next(60_000, 120_001);
        var early = random.NextDouble() < 0.10;
        var coneCount = early ? random.Next(1, 5) : random.Next(0, 5);

        var cones = Enumerable.Range(1, 12).Select(i => $"C{i}").ToArray();
        random.Shuffle(cones);

        var hits = new List<(ConeHit Hit, double Fault)>();
        foreach (var coneId in cones.Take(coneCount))
        {
            var sensorTs = startedAt.AddMilliseconds(random.Next(1, durationMs));
            hits.Add((new ConeHit(Guid.NewGuid(), runId, coneId, sensorTs, "v2"), random.NextDouble()));
        }

        List<ICourseEvent> events = [new RunStarted(Guid.NewGuid(), runId, heatId, driverId, startedAt, "v2")];
        foreach (var (hit, fault) in hits.OrderBy(h => h.Hit.SensorTs))
        {
            events.Add(hit);
            if (fault < 0.15) events.Add(hit);
            else if (fault < 0.30) events.Add(hit with { EventId = Guid.NewGuid(), Firmware = "v1" });
        }
        events.Add(new RunFinished(Guid.NewGuid(), runId, startedAt.AddMilliseconds(durationMs), "v2"));

        if (early)
            (events[0], events[1]) = (events[1], events[0]);

        foreach (var courseEvent in events)
            await publisher.PublishAsync(courseEvent);

        return runId;
    }
}
