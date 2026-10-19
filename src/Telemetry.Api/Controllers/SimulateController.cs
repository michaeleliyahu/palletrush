using Microsoft.AspNetCore.Mvc;
using Telemetry.Api.Simulation;

namespace Telemetry.Api.Controllers;

public record SimulateRunRequest(int HeatId, int DriverId, int? Seed);

[ApiController]
[Route("simulate")]
public class SimulateController(RunSimulator simulator) : ControllerBase
{
    [HttpPost("run")]
    public async Task<IActionResult> Run(SimulateRunRequest request)
    {
        var runId = await simulator.SimulateAsync(request.HeatId, request.DriverId, request.Seed);
        return Accepted(new { runId });
    }
}
