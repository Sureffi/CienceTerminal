using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TokenMetrics.Domain.Common;
using TokenMetrics.Domain.Entities;
using TokenMetrics.Domain.Interfaces;

namespace TokenMetrics.Infrastructure.ExternalServices;

public class JupiterClient : IJupiterClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JupiterClient> _logger;
    private const string JupiterApiBaseUrl = "https://lite-api.jup.ag/ultra/v1";

    public JupiterClient(HttpClient httpClient, ILogger<JupiterClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<JupiterTokenData>> GetTokenMetadataAsync(string mintAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            string uri = $"{JupiterApiBaseUrl}/search?query={mintAddress}";

            var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);

            // Handle HTTP status codes
            if (!httpResponse.IsSuccessStatusCode)
            {
                return httpResponse.StatusCode switch
                {
                    HttpStatusCode.NotFound =>
                        Result<JupiterTokenData>.NotFound($"Token not found for mint address: {mintAddress}"),
                    HttpStatusCode.TooManyRequests =>
                        Result<JupiterTokenData>.RateLimited("Jupiter API rate limit exceeded"),
                    HttpStatusCode.ServiceUnavailable or HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout =>
                        Result<JupiterTokenData>.ServerError($"Jupiter API is currently unavailable: {httpResponse.StatusCode}"),
                    _ =>
                        Result<JupiterTokenData>.Failure($"Jupiter API returned {httpResponse.StatusCode}")
                };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<List<JupiterTokenData>>(cancellationToken: cancellationToken);

            if (response is null || response.Count == 0)
            {
                _logger.LogWarning("No token found for mint address: {MintAddress}", mintAddress);
                return Result<JupiterTokenData>.NotFound($"Token not found for mint address: {mintAddress}");
            }

            // Find exact match by ID (mint address)
            var token = response.FirstOrDefault(t =>
                t.Id.Equals(mintAddress, StringComparison.OrdinalIgnoreCase));

            if (token == null)
            {
                _logger.LogWarning("Exact match not found for mint address: {MintAddress}", mintAddress);
                return Result<JupiterTokenData>.NotFound($"Exact match not found for mint address: {mintAddress}");
            }

            _logger.LogInformation("Successfully fetched metadata for token {Symbol} ({MintAddress})",
                token.Symbol, mintAddress);

            return Result<JupiterTokenData>.Success(token);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while fetching Jupiter metadata for {MintAddress}", mintAddress);
            return Result<JupiterTokenData>.NetworkError($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout while fetching Jupiter metadata for {MintAddress}", mintAddress);
            return Result<JupiterTokenData>.NetworkError("Request timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching Jupiter metadata for {MintAddress}", mintAddress);
            return Result<JupiterTokenData>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Dictionary<string, JupiterTokenData>> GetBatchTokenMetadataAsync(IEnumerable<string> mintAddresses, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, JupiterTokenData>(StringComparer.OrdinalIgnoreCase);

        var addressList = mintAddresses.ToList();
        if (addressList.Count == 0)
        {
            _logger.LogWarning("No mint addresses provided for batch fetch");
            return result;
        }

        try
        {
            // Jupiter API supports up to 100 mint addresses per request
            const int batchSize = 100;
            var batches = addressList
                .Select((address, index) => new { address, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.address).ToList())
                .ToList();

            _logger.LogInformation("Fetching {TotalCount} tokens in {BatchCount} batch(es)", addressList.Count, batches.Count);

            foreach (var batch in batches)
            {
                // Create comma-separated query string
                var query = string.Join(",", batch);
                var uri = $"{JupiterApiBaseUrl}/search?query={Uri.EscapeDataString(query)}";

                var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Jupiter API returned {StatusCode} for batch of {Count} tokens",
                        httpResponse.StatusCode, batch.Count);
                    continue;
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<List<JupiterTokenData>>(cancellationToken: cancellationToken);

                if (response == null || response.Count == 0)
                {
                    _logger.LogWarning("No tokens found in batch response for {Count} addresses", batch.Count);
                    continue;
                }

                // Map tokens by their mint address (ID)
                foreach (var token in response)
                {
                    if (!string.IsNullOrEmpty(token.Id))
                    {
                        result[token.Id] = token;
                    }
                }

                _logger.LogDebug("Successfully fetched {Count} tokens from batch", response.Count);
            }

            _logger.LogInformation("Batch fetch completed: {FoundCount}/{RequestedCount} tokens found",
                result.Count, addressList.Count);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while batch fetching Jupiter metadata");
            return result;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout while batch fetching Jupiter metadata");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while batch fetching Jupiter metadata");
            return result;
        }
    }
}
