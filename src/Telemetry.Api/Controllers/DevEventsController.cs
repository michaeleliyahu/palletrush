using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Telemetry.Api.Events;

namespace Telemetry.Api.Controllers;

[ApiController]
[Route("dev/events")]
public class DevEventsController(CourseEventPublisher publisher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publish(CourseEventEnvelope envelope)
    {
        if (envelope.Payload.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            ModelState.AddModelError(nameof(envelope.Payload), "The Payload field is required.");
            return ValidationProblem();
        }

        await publisher.PublishRawAsync(envelope.Type, envelope.Payload.GetRawText());
        return Accepted();
    }
}
