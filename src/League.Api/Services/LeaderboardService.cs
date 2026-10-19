using League.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace League.Api.Services;

public record LeaderboardEntry(Guid RunId, int DriverId, string DriverName, string TeamName, int RawTimeMs, int Rank);

public class LeaderboardService(LeagueDbContext db)
{
    public async Task<List<LeaderboardEntry>> GetAsync(int heatId, CancellationToken ct)
    {
        var runs = await db.Runs
            .Where(r => r.HeatId == heatId && r.RawTimeMs != null)
            .OrderBy(r => r.RawTimeMs)
            .ThenBy(r => r.FinishedAt)
            .Select(r => new
            {
                r.Id,
                r.DriverId,
                DriverName = r.Driver.Name,
                TeamName = r.Driver.Team.Name,
                RawTimeMs = r.RawTimeMs!.Value,
            })
            .ToListAsync(ct);

        return runs
            .Select((r, i) => new LeaderboardEntry(r.Id, r.DriverId, r.DriverName, r.TeamName, r.RawTimeMs, i + 1))
            .ToList();
    }
}
