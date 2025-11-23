using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using TwitterScanner.Application.Interfaces;

namespace TwitterScanner.Infrastructure.ExternalServices;

public class SolanaRpcService : ISolanaRpcService
{
    private readonly IRpcClient _client;
    private readonly ILogger<SolanaRpcService> _logger;
    private readonly IMemoryCache _cache;

    public SolanaRpcService(IRpcClient client, ILogger<SolanaRpcService> logger, IMemoryCache cache)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    // TODO: Proper error handling and rate limiting
    public async Task<ParsedTokenMintData?> GetTokenMintInfoAsync(string mint)
    {
        if (string.IsNullOrWhiteSpace(mint))
        {
            _logger.LogDebug("Received null or whitespace string in GetTokenMintInfoAsync");
            return null;
        }

        string cacheKey = $"token_mint_{mint}";

        // Check cache first
        if (_cache.TryGetValue(cacheKey, out ParsedTokenMintData? cachedResult))
        {
            _logger.LogDebug("Retrieved token mint data from cache for mint: {Mint}", mint);
            return cachedResult;
        }

        var result = await _client.GetTokenMintInfoAsync(mint);
        if (result.Result?.Value?.Data?.Parsed is null)
        {
            return null;
        }

        var tokenMintData = result.Result.Value.Data.Parsed;

        // Cache the result for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        _cache.Set(cacheKey, tokenMintData, cacheOptions);
        _logger.LogDebug("Cached token mint data for mint: {Mint}", mint);

        return tokenMintData;
    }
}
