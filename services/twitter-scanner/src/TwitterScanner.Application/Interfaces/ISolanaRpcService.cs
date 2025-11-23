using Solnet.Rpc.Models;

namespace TwitterScanner.Application.Interfaces;

public interface ISolanaRpcService
{
    Task<ParsedTokenMintData?> GetTokenMintInfoAsync(string mint);
}
