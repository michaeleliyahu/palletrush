using System.Net;
using System.Net.Http.Json;
using League.Api.Data;
using League.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace League.Api.Tests;

public class RegistrationConcurrencyTests
{
    private const int HeatId = 7;
    private const int Requests = 20;

    [Test]
    public async Task Simultaneous_registrations_for_the_last_slot_admit_exactly_one_driver()
    {
        using var client = TestEnvironment.Factory.CreateClient();
        var heat = await client.GetFromJsonAsync<HeatSummary>($"/api/heats/{HeatId}");
        Assert.That(heat!.Capacity - heat.RegisteredCount, Is.EqualTo(1), "heat should have exactly one free slot");

        var driverIds = await PickDriversNotInHeatAsync(Requests);

        var url = $"/api/heats/{HeatId}/registrations";
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var posts = driverIds.Select(async id =>
        {
            await gate.Task;
            return await client.PostAsJsonAsync(url, new { driverId = id });
        }).ToList();
        gate.SetResult();
        var responses = await Task.WhenAll(posts);

        var after = await client.GetFromJsonAsync<HeatSummary>($"/api/heats/{HeatId}");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(responses.Count(r => r.StatusCode == HttpStatusCode.Created), Is.EqualTo(1));
            Assert.That(responses.Count(r => r.StatusCode == HttpStatusCode.Conflict), Is.EqualTo(Requests - 1));
            Assert.That(after!.RegisteredCount, Is.EqualTo(after.Capacity));
        }
    }

    private static async Task<List<int>> PickDriversNotInHeatAsync(int count)
    {
        using var scope = TestEnvironment.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LeagueDbContext>();
        return await db.Drivers
            .Where(d => d.Team.Active && !db.Registrations.Any(r => r.HeatId == HeatId && r.DriverId == d.Id))
            .OrderBy(d => d.Id)
            .Select(d => d.Id)
            .Take(count)
            .ToListAsync();
    }
}
