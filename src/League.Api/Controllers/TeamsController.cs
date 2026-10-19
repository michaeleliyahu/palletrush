using League.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace League.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController(TeamService teams) : ControllerBase
{
    [HttpGet]
    public Task<List<TeamSummary>> GetAll(CancellationToken ct) =>
        teams.GetAllAsync(ct);
}
