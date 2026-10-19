namespace League.Api.Events;

public interface ICourseEventHandler<in T>
{
    Task HandleAsync(T courseEvent, CancellationToken ct);
}
