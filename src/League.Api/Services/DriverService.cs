using League.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace League.Api.Services;

public record DriverSummary(int Id, string Name, string TeamName, string LicenseNo);

public record OnCourseEntry(Guid RunId, int HeatId, int DriverId, string DriverName, string TeamName);

public class DriverService(LeagueDbContext db)
{
    public Task<List<DriverSummary>> SearchAsync(string search, CancellationToken ct) =>
        db.Drivers
            .Where(d => EF.Functions.ILike(d.Name, $"%{search}%"))
            .OrderBy(d => d.Name)
            .ThenBy(d => d.Id)
            .Take(20)
            .Select(d => new DriverSummary(d.Id, d.Name, d.Team.Name, d.LicenseNo))
            .ToListAsync(ct);

    public Task<List<OnCourseEntry>> GetOnCourseAsync(CancellationToken ct) =>
        db.Runs
            .Where(r => r.FinishedAt == null)
            .OrderBy(r => r.StartedAt)
            .Select(r => new OnCourseEntry(r.Id, r.HeatId, r.DriverId, r.Driver.Name, r.Driver.Team.Name))
            .ToListAsync(ct);
}
