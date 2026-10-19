using League.Api.Data;
using Shared.Contracts;

namespace League.Api.Events;

public class RunFinishedHandler(LeagueDbContext db, ILogger<RunFinishedHandler> logger) : ICourseEventHandler<RunFinished>
{
    public async Task HandleAsync(RunFinished courseEvent, CancellationToken ct)
    {
        var run = await db.Runs.FindAsync([courseEvent.RunId], ct);
        if (run is null)
        {
            logger.LogWarning("RunFinished for unknown run {RunId}", courseEvent.RunId);
            return;
        }

        run.FinishedAt = courseEvent.SensorTs.ToUniversalTime();
        run.RawTimeMs = (int)Math.Round((courseEvent.SensorTs - run.StartedAt).TotalMilliseconds);
        await db.SaveChangesAsync(ct);
    }
}
