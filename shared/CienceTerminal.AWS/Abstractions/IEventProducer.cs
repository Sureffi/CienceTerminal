namespace CienceTerminal.AWS.Abstractions;

public interface IEventProducer
{
    Task PublishAsync<T>(string topicArn, T @event, CancellationToken cancellationToken = default) where T : class;
}