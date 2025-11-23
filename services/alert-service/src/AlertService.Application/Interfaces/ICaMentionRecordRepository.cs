namespace AlertService.Application.Interfaces;

/// <summary>
/// Repository for querying CA mention records from the shared database.
/// Alert Service has read-only access to this data (owned by Token Metrics Service).
/// </summary>
public interface ICaMentionRecordRepository
{
    /// <summary>
    /// Gets the top mentioners for a specific coin based on follower count.
    /// Returns profile picture URLs of the most influential mentioners.
    /// </summary>
    /// <param name="coinMintAddress">The coin's mint address</param>
    /// <param name="count">Number of top mentioners to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of profile picture URLs from top mentioners</returns>
    Task<List<string>> GetTopMentionerProfilePicturesAsync(
        string coinMintAddress,
        int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// Batch query to get top mentioners for multiple coins.
    /// </summary>
    /// <param name="coinMintAddresses">Collection of coin mint addresses</param>
    /// <param name="count">Number of top mentioners per coin</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary mapping coin addresses to lists of profile picture URLs</returns>
    Task<Dictionary<string, List<string>>> GetTopMentionerProfilePicturesBatchAsync(
        IEnumerable<string> coinMintAddresses,
        int count,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets mention records for a specific coin address.
    /// Returns full mention details ordered by verified status and follower count.
    /// </summary>
    /// <param name="coinMintAddress">The coin's mint address</param>
    /// <param name="hours">Number of hours to look back (default 24)</param>
    /// <param name="limit">Maximum number of mentions to return (default 9)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of CA mention records</returns>
    Task<List<AlertService.Domain.Entities.CaMentionRecord>> GetMentionsByCoinAsync(
        string coinMintAddress,
        int hours = 24,
        int limit = 9,
        CancellationToken cancellationToken = default);
}
