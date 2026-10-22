using System.Net;
using System.Net.Http.Json;
using League.Api.Data;
using League.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace League.Api.Tests;

public class RegistrationTests
{
    [Test]
    public async Task Ineligible_driver_is_rejected_without_taking_a_slot()
    {
        int heatId, driverId;
        using (var scope = TestEnvironment.Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LeagueDbContext>();
            var heat = new Heat { Name = "Ineligible test heat", StartsAt = DateTimeOffset.UtcNow, Capacity = 8, RegisteredCount = 0 };
            var driver = new Driver
            {
                Name = "Inactive Driver",
                LicenseNo = "FL-INACTIVE",
                Team = new Team { Name = "Retired Team", Active = false },
            };
            db.AddRange(heat, driver);
            await db.SaveChangesAsync();
            (heatId, driverId) = (heat.Id, driver.Id);
        }

        using var client = TestEnvironment.Factory.CreateClient();
        var url = $"/api/heats/{heatId}/registrations";
        var response = await client.PostAsJsonAsync(url, new { driverId });

        var after = await client.GetFromJsonAsync<HeatSummary>($"/api/heats/{heatId}");
        var registrations = await client.GetFromJsonAsync<List<RegistrationEntry>>(url);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
            Assert.That(after!.RegisteredCount, Is.Zero);
            Assert.That(registrations, Is.Empty);
        }
    }

    [Test]
    public async Task Registering_for_an_unknown_heat_returns_not_found()
    {
        using var client = TestEnvironment.Factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/heats/999999/registrations", new { driverId = 1 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
