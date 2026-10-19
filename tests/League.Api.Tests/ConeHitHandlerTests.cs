using System.Net.Http.Json;
using Shared.Contracts;

namespace League.Api.Tests;

public class ConeHitHandlerTests
{
    private const int HeatId = 1;
    private const int DriverId = 2;

    [Test]
    public async Task Handling_the_same_ConeHit_twice_adds_one_penalty()
    {
        var runId = Guid.NewGuid();
        var t0 = new DateTimeOffset(2026, 11, 7, 9, 30, 0, TimeSpan.Zero);
        await TestEnvironment.HandleAsync(new RunStarted(Guid.NewGuid(), runId, HeatId, DriverId, t0, "v2"));
        await TestEnvironment.HandleAsync(new RunFinished(Guid.NewGuid(), runId, t0.AddSeconds(90), "v2"));

        var coneHit = new ConeHit(Guid.NewGuid(), runId, "C3", t0.AddSeconds(30), "v2");
        await TestEnvironment.HandleAsync(coneHit);
        await TestEnvironment.HandleAsync(coneHit);

        using var client = TestEnvironment.Factory.CreateClient();
        var leaderboard = await client.GetFromJsonAsync<List<LeaderboardRow>>($"/api/heats/{HeatId}/leaderboard");
        var row = leaderboard!.Single(r => r.RunId == runId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(row.PenaltyCount, Is.EqualTo(1));
            Assert.That(row.TotalTimeMs, Is.EqualTo(row.RawTimeMs + 5000));
        }
    }

    private record LeaderboardRow(Guid RunId, int RawTimeMs, int PenaltyCount, int TotalTimeMs);
}
