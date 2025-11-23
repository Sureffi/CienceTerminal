using TwitterScanner.Domain.Entities;

namespace TwitterScanner.Application.Interfaces;

public interface ITwitterStreamingClient
{
    Task ConnectAsync(Action<Tweet> onMessage, CancellationToken cancellationToken);
}
