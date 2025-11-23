namespace AlertService.Application.DTOs;

/// <summary>
/// Data Transfer Object for CA mention records.
/// Represents a single mention of a cryptocurrency contract address on Twitter.
/// </summary>
public class CaMentionRecordDto
{
    public Guid Id { get; set; }
    public string CoinMintAddress { get; set; } = string.Empty;
    public string TweetId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string TweetUrl { get; set; } = string.Empty;
    public string? TweetContent { get; set; }
    public int Followers { get; set; }
    public bool IsVerified { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsOriginalPost { get; set; }
    public bool IsReply { get; set; }
    public bool IsQuote { get; set; }
    public bool IsRetweet { get; set; }
}
