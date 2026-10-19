using League.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts;

namespace League.Api.Tests;

public class RunStartedHandlerTests
{
    private const int HeatId = 1;
    private const int DriverId = 3;

    [Test]
    public async Task Handling_the_same_RunStarted_twice_creates_one_run()
    {
        var startedAt = new DateTimeOffset(2026, 11, 7, 9, 30, 0, TimeSpan.Zero);
        var runStarted = new RunStarted(Guid.NewGuid(), Guid.NewGuid(), HeatId, DriverId, startedAt, "v2");

        await TestEnvironment.HandleAsync(runStarted);
        await TestEnvironment.HandleAsync(runStarted);

        using var scope = TestEnvironment.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LeagueDbContext>();
        Assert.That(await db.Runs.CountAsync(r => r.Id == runStarted.RunId), Is.EqualTo(1));
    }
}
