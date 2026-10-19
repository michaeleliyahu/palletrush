using Shared.Contracts;

namespace League.Api.Events;

public class ConeHitHandler : ICourseEventHandler<ConeHit>
{
    public Task HandleAsync(ConeHit courseEvent, CancellationToken ct) =>
        throw new NotImplementedException();
}
