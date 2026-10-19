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
        var heat = await db.Heats.FindAsync([heatId], ct);
        if (heat is null)
            return RegistrationResult.HeatNotFound;

        if (heat.RegisteredCount >= heat.Capacity)
            return RegistrationResult.HeatFull;

        var eligible = await db.Drivers.AnyAsync(d => d.Id == driverId && d.Team.Active, ct);
        if (!eligible)
            return RegistrationResult.DriverNotEligible;

        db.Registrations.Add(new Registration
        {
            HeatId = heatId,
            DriverId = driverId,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        heat.RegisteredCount++;
        await db.SaveChangesAsync(ct);

        return RegistrationResult.Registered;
    }
}
