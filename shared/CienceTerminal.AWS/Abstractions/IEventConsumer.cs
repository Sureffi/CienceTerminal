namespace CienceTerminal.AWS.Abstractions;

public interface IEventConsumer<T> where T : class
{
    Task HandleEventAsync(T @event, CancellationToken cancellationToken = default);
}