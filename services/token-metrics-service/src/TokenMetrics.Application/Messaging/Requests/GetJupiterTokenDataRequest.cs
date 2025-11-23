using MediatR;
using TokenMetrics.Domain.Common;
using TokenMetrics.Domain.Entities;

namespace TokenMetrics.Application.Messaging.Requests;

public record GetJupiterTokenDataRequest(string CoinMintAddress) : IRequest<Result<JupiterTokenData>>;
