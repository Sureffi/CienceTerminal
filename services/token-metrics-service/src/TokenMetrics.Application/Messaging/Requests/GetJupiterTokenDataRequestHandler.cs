using MediatR;
using TokenMetrics.Domain.Common;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Domain.Interfaces;

namespace TokenMetrics.Application.Messaging.Requests;

public class GetJupiterTokenDataRequestHandler : IRequestHandler<GetJupiterTokenDataRequest, Result<JupiterTokenData>>
{
    private readonly IJupiterClient _jupiterClient;

    public GetJupiterTokenDataRequestHandler(IJupiterClient jupiterClient)
    {
        _jupiterClient = jupiterClient;
    }

    public async Task<Result<JupiterTokenData>> Handle(GetJupiterTokenDataRequest request, CancellationToken cancellationToken)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(request.CoinMintAddress))
        {
            return Result<JupiterTokenData>.ValidationError("Coin mint address cannot be empty");
        }

        // Fetch token metadata from Jupiter API
        var result = await _jupiterClient.GetTokenMetadataAsync(request.CoinMintAddress, cancellationToken);
        return result;
    }
}
