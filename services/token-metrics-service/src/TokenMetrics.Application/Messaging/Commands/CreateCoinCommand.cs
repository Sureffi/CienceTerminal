using MediatR;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Messaging.Commands;

/// <summary>
/// Command to create a new Coin record with optional Jupiter metadata enrichment.
/// </summary>
public record CreateCoinCommand(string CoinMintAddress) : IRequest<Coin>;
