using League.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace League.Api.Services;

public record HeatSummary(int Id, string Name, DateTimeOffset StartsAt, int Capacity, int RegisteredCount);

public class HeatService(LeagueDbContext db)
{
    public Task<List<HeatSummary>> GetAllAsync(CancellationToken ct) =>
        db.Heats
            .OrderBy(h => h.Id)
            .Select(h => new HeatSummary(h.Id, h.Name, h.StartsAt, h.Capacity, h.RegisteredCount))
            .ToListAsync(ct);

    public Task<HeatSummary?> GetAsync(int id, CancellationToken ct) =>
        db.Heats
            .Where(h => h.Id == id)
            .Select(h => new HeatSummary(h.Id, h.Name, h.StartsAt, h.Capacity, h.RegisteredCount))
            .SingleOrDefaultAsync(ct);
}
