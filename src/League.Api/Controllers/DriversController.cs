using League.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace League.Api.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController(DriverService drivers) : ControllerBase
{
    [HttpGet]
    public Task<List<DriverSummary>> Search(string? search, CancellationToken ct) =>
        drivers.SearchAsync(search ?? "", ct);

    [HttpGet("on-course")]
    public Task<List<OnCourseEntry>> GetOnCourse(CancellationToken ct) =>
        drivers.GetOnCourseAsync(ct);
}
