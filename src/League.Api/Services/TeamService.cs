using League.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace League.Api.Services;

public record TeamDriver(int Id, string Name);

public record TeamSummary(int Id, string Name, List<TeamDriver> Drivers);

public class TeamService(LeagueDbContext db)
{
    public Task<List<TeamSummary>> GetAllAsync(CancellationToken ct) =>
        db.Teams
            .OrderBy(t => t.Name)
            .Select(t => new TeamSummary(
                t.Id,
                t.Name,
                t.Drivers.OrderBy(d => d.Name).Select(d => new TeamDriver(d.Id, d.Name)).ToList()))
            .ToListAsync(ct);
}
