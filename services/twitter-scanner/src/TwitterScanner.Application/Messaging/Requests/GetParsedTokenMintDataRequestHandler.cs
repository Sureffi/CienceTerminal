using MediatR;
using Solnet.Rpc.Models;
using TwitterScanner.Application.Interfaces;

namespace TwitterScanner.Application.Messaging.Requests;

public class GetParsedTokenMintDataRequestHandler : IRequestHandler<GetParsedTokenMintDataRequest, ParsedTokenMintData?>
{
    private readonly ISolanaRpcService _solanaRpcService;

    public GetParsedTokenMintDataRequestHandler(ISolanaRpcService solanaRpcService)
    {
        _solanaRpcService = solanaRpcService;
    }

    public async Task<ParsedTokenMintData?> Handle(GetParsedTokenMintDataRequest request, CancellationToken cancellationToken)
    {
        return await _solanaRpcService.GetTokenMintInfoAsync(request.Mint);
    }
}
