using MediatR;

namespace TokenMetrics.Application.Messaging.Commands;

/// <summary>
/// Command to publish TokenMetricsUpdatedEvent for a specific coin.
/// Handled in Infrastructure layer where AWS dependencies are available.
/// </summary>
public record PublishTokenMetricsUpdatedCommand(string CoinMintAddress) : IRequest;
