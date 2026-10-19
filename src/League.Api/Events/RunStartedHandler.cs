using League.Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace League.Api.Events;

public class RunStartedHandler(LeagueDbContext db) : ICourseEventHandler<RunStarted>
{
    public Task HandleAsync(RunStarted courseEvent, CancellationToken ct) =>
        db.Database.ExecuteSqlAsync($"""
            INSERT INTO runs (id, heat_id, driver_id, started_at)
            VALUES ({courseEvent.RunId}, {courseEvent.HeatId}, {courseEvent.DriverId}, {courseEvent.SensorTs.ToUniversalTime()})
            ON CONFLICT (id) DO NOTHING
            """, ct);
}
