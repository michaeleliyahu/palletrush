using League.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace League.Api.Services;

public record RegistrationEntry(int Id, int DriverId, string DriverName, string TeamName, DateTimeOffset CreatedAt);

public enum RegistrationResult
{
    Registered,
    HeatNotFound,
    HeatFull,
    DriverNotEligible,
}

public class RegistrationService(LeagueDbContext db)
{
    public Task<List<RegistrationEntry>> GetAsync(int heatId, CancellationToken ct) =>
        db.Registrations
            .Where(r => r.HeatId == heatId)
            .OrderBy(r => r.Id)
            .Select(r => new RegistrationEntry(r.Id, r.DriverId, r.Driver.Name, r.Driver.Team.Name, r.CreatedAt))
            .ToListAsync(ct);

    public async Task<RegistrationResult> RegisterAsync(int heatId, int driverId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // Claim a slot atomically: concurrent requests (on any replica) queue on the heat's row lock
        // and re-check the capacity condition against the committed count.
        var claimed = await db.Heats
            .Where(h => h.Id == heatId && h.RegisteredCount < h.Capacity)
            .ExecuteUpdateAsync(s => s.SetProperty(h => h.RegisteredCount, h => h.RegisteredCount + 1), ct);
        if (claimed == 0)
            return await db.Heats.AnyAsync(h => h.Id == heatId, ct)
                ? RegistrationResult.HeatFull
                : RegistrationResult.HeatNotFound;

        // Returning without commit rolls the claim back.
        var eligible = await db.Drivers.AnyAsync(d => d.Id == driverId && d.Team.Active, ct);
        if (!eligible)
            return RegistrationResult.DriverNotEligible;

        db.Registrations.Add(new Registration
        {
            HeatId = heatId,
            DriverId = driverId,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return RegistrationResult.Registered;
    }
}
