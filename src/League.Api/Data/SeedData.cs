using Microsoft.EntityFrameworkCore;

namespace League.Api.Data;

public static class SeedData
{
    private static readonly string[] TeamNames =
    [
        "Northside Logistics", "Harbor Freight Co.", "Summit Distribution", "Riverbend Storage",
        "Ironclad Pallets", "Cascade Supply", "Bluewater Depot", "Redline Warehousing",
        "Granite Fulfillment", "Lakeshore Cargo", "Prairie Wholesale", "Eastgate Freight",
    ];

    private static readonly string[] FirstNames =
    [
        "Adrian", "Alan", "Amanda", "Brandon", "Carlos", "Diana", "Elena", "Felix", "Grace", "Hugo",
        "Ivana", "Juliana", "Kofi", "Mariana", "Noah", "Oksana", "Priya", "Susana", "Tatiana", "Yusuf",
    ];

    private static readonly string[] LastNames =
        ["Becker", "Costa", "Dubois", "Ito", "Kowalski", "Mensah", "Novak", "Okafor", "Reyes", "Weber"];

    private static readonly int[] RegisteredPerHeat = [6, 5, 4, 8, 3, 5, 7, 2, 6, 4];

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken ct)
    {
        var db = (LeagueDbContext)context;
        if (await db.Teams.AnyAsync(ct))
            return;

        var teams = TeamNames.Select(name => new Team { Name = name, Active = true }).ToList();
        db.Teams.AddRange(teams);

        var drivers = FirstNames
            .SelectMany(first => LastNames.Select(last => $"{first} {last}"))
            .Select((name, i) => new Driver { Team = teams[i % teams.Count], Name = name, LicenseNo = $"FL-{10001 + i}" })
            .ToList();
        db.Drivers.AddRange(drivers);

        var firstHeatStartsAt = new DateTimeOffset(2026, 11, 7, 9, 0, 0, TimeSpan.Zero);
        var driversPerHeat = drivers.Count / RegisteredPerHeat.Length;
        var random = new Random(2026);

        for (var h = 0; h < RegisteredPerHeat.Length; h++)
        {
            var heat = new Heat
            {
                Name = $"Heat {h + 1}",
                StartsAt = firstHeatStartsAt.AddHours(h),
                Capacity = 8,
                RegisteredCount = RegisteredPerHeat[h],
            };
            db.Heats.Add(heat);

            var registered = drivers.Skip(h * driversPerHeat).Take(heat.RegisteredCount).ToList();
            db.Registrations.AddRange(registered.Select((driver, k) => new Registration
            {
                Heat = heat,
                Driver = driver,
                CreatedAt = heat.StartsAt.AddDays(-7).AddMinutes(90 * k),
            }));

            var finishedRuns = h % 2 == 0 ? 3 : 2;
            for (var k = 0; k < finishedRuns; k++)
            {
                var startedAt = heat.StartsAt.AddMinutes(5 + 4 * k);
                var rawTimeMs = random.Next(60_000, 120_001);
                db.Runs.Add(new Run
                {
                    Id = new Guid($"00000000-0000-0000-0000-{h + 1:D6}{k + 1:D6}"),
                    Heat = heat,
                    Driver = registered[^(k + 1)],
                    StartedAt = startedAt,
                    FinishedAt = startedAt.AddMilliseconds(rawTimeMs),
                    RawTimeMs = rawTimeMs,
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
